using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrapType
{
	Hit = 0,
	Explosion = 1
}

public class WorldObjectTrap : MonoBehaviour
{
	[SerializeField] PhotonWorldObject _photonWorldObject;
	[SerializeField] TrapType _trapType;
	[SerializeField] bool _corruptible;
	[SerializeField] float _explosionArea;
	[SerializeField] LayerMask _explosionLayerMask;
	[SerializeField] List<DestructibleObjectType> _destructibleObjTypes;
	[SerializeField] int _destructibleObjDamage;
	[SerializeField] float _hitInterval;
	[SerializeField] int _damage;
	[SerializeField] int _damageToTrap;
	[SerializeField] bool _pushMob;
	[SerializeField] float _pushForce;
	[SerializeField] bool _activateByTime;
	[SerializeField] float _activationTime;
	[SerializeField] GameObject _explosionEffect;
	[SerializeField] ElectricEffectController _electricEffect;

	private float _lastHitTime;
	private bool _canActivate;
	private bool _showEffect;

	public void OnInstantiate(double time)
	{
		_canActivate = true;
		_showEffect = true;
		if (_activateByTime)
		{
			StartCoroutine(ActivateByTime(time));
		}
	}

	private void OnDestroy()
	{
		if (_showEffect && _trapType == TrapType.Explosion && (bool)_explosionEffect)
		{
			ExplosionController component = ((GameObject)Object.Instantiate(_explosionEffect, base.transform.position, base.transform.rotation)).GetComponent<ExplosionController>();
			component.Initialize(_explosionArea);
		}
	}

	private IEnumerator ActivateByTime(double landTime)
	{
		float lifetime = (float)(PhotonNetwork.time - landTime);
		if (lifetime < _activationTime)
		{
			yield return new WaitForSeconds(_activationTime - lifetime);
		}
		if (_canActivate)
		{
			if (PhotonNetwork.isMasterClient)
			{
				ApplyAreaDamage();
				DestroyByMaster();
			}
			_canActivate = false;
		}
	}

	private void PushMob(Collider target)
	{
		switch (target.tag)
		{
		case "Zombie":
			PhotonMob component = target.GetComponent<PhotonMob>();
			if (_pushMob)
			{
				component.Push(-component.transform.forward, _pushForce);
			}
			break;
		}
	}

	public void ApplyDamage(Collider target)
	{
		switch (target.tag)
		{
		case "Player":
			WorldController.I.Player.HitPlayer ((short)_damage, (byte)0, null);
			if (_corruptible) {
				_photonWorldObject.Hit (_damageToTrap, new List<DestructibleObjectType> { _photonWorldObject.DestructibleType });
			}
			Debug.Log ("damage!");
			break;
		case "Zombie":
		{
			PhotonMob component = target.GetComponent<PhotonMob>();
			if ((bool)component)
			{
				component.Hit((short)_damage, -component.transform.forward, component.transform.position, false, null);
				if (_corruptible)
				{
					_photonWorldObject.Hit(_damageToTrap, new List<DestructibleObjectType> { _photonWorldObject.DestructibleType });
				}
			}
			break;
		}
		}
	}

	private void ApplyAreaDamage()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, _explosionArea, _explosionLayerMask);

		foreach (Collider collider in array)
		{
			switch (collider.tag)
			{
			case "Zombie":
			{
				PhotonMob component = collider.GetComponent<PhotonMob>();
				if ((bool)component)
				{
					component.Hit((short)_damage, -component.transform.forward, component.transform.position, false, null);
				}
				continue;
			}
			case "Player":
				WorldController.I.Player.HitPlayer((short)_damage, (byte)0, null);
				continue;
			}
			if (collider.gameObject.layer == 25)
			{
				PhotonMan component2 = collider.GetComponent<PhotonMan>();
				if ((bool)component2)
				{
					component2.HitPlayer((short)_damage, (byte)0, null);
				}
			}
			else
			{
				HitWorldObject(collider);
			}
		}
	}

	private void HitWorldObject(Collider target)
	{
		if (target.gameObject.layer != 28 && target.gameObject.layer != 21)
		{
			return;
		}
		PhotonWorldObject component = target.transform.parent.transform.parent.GetComponent<PhotonWorldObject>();
		if ((bool)component && component != _photonWorldObject)
		{
			component.Hit(_destructibleObjDamage, _destructibleObjTypes);
			return;
		}
		PhotonObject component2 = target.transform.parent.transform.parent.GetComponent<PhotonObject>();
		if ((bool)component2)
		{
			component2.Hit((short)_destructibleObjDamage, _destructibleObjTypes);
		}
	}

	private void OnTriggerStay(Collider target)
	{
		if (!_canActivate || target.name == "NoiseArea" || target.name == "Sensor")
		{
			return;
		}

		Debug.Log (target);
		switch (_trapType)
		{
		case TrapType.Hit:
			if (Time.time - _lastHitTime >= _hitInterval)
			{
				if (_electricEffect != null)
				{
					_electricEffect.Play();
				}
					
				ApplyDamage(target);
				_lastHitTime = Time.time;
			}
			PushMob(target);
			break;
		case TrapType.Explosion:
			switch (target.tag)
			{
			case "Player":
				ApplyAreaDamage();
				_canActivate = false;
				DestroyByMaster();
				break;
			case "Zombie":
				ApplyAreaDamage();
				_canActivate = false;
				DestroyByMaster();
				break;
			}
			break;
		}
	}

	private void DestroyByMaster()
	{
		_photonWorldObject.photonView.RPC("DestroyByMaster", PhotonTargets.All);
	}
}
