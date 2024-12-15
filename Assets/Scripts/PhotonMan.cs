using System;
using System.Collections;
using System.Collections.Generic;
using BattleRoyale;
using CodeStage.AntiCheat.ObscuredTypes;
using Photon;
using SkyWars;
using UnityEngine;

public enum ManState : byte
{
	HealthPoint = 0,
	Hunger = 1,
	Thirst = 2,
	Sickness = 3,
	Stamina = 4
}

public class PhotonManInfo
{
	public ObscuredShort HealthPoints;
	public Dictionary<string, string> Equipment;
}

public class PhotonMan : Photon.MonoBehaviour, IPunObservable
{
	internal struct State
	{
		internal double timestamp;
		internal Vector3 pos;
		internal Vector3 velocity;
		internal Quaternion rot;
		internal Vector3 angularVelocity;
	}

	private const int OnDieMenuWaitTime = 2;

	[SerializeField] PlayerPhotonSectorController _mySectorController;
	[SerializeField] GameObject _model;
	[SerializeField] GameObject _ragdoll;
	[SerializeField] Animator _animator;
	[SerializeField] ModelClothingController _clothingController;
	[SerializeField] ThirdPersonAttackController _thirdPersonAttackController;
	[SerializeField] AudioSource _weaponAudioSource;
	[SerializeField] GameObject _rainCoatManager;

	private ObscuredInt _hunger;
	private ObscuredInt _thirst;
	private ObscuredInt _sickness;

	private string _id;
	private string _name;
	private int _animatorSpeedId;
	private bool _isAnimatorHashInitialized;

	public Rigidbody rigBody;
	public CapsuleCollider capsCollider;

	private LayerMask _ragdollRaymask = 512;
	private double _interpolationBackTime = 0.1;
	private double _extrapolationLimit = 0.5;
	private State[] _bufferedState = new State[20];
	private int _timestampCount;

	[SerializeField] GameObject _parachute;

	public PhotonManInfo ManInfo { get; private set; }

	public ObscuredInt CurrentHunger
	{
		get
		{
			return _hunger;
		}
		 set
		{
			_hunger = value;
			DataKeeper.PlayerGameInfoBackup.Hunger = value;
		}
	}

	public ObscuredInt CurrentThirst
	{
		get
		{
			return _thirst;
		}
		 set
		{
			_thirst = value;
			DataKeeper.PlayerGameInfoBackup.Thirst = value;
		}
	}

	public ObscuredInt CurrentSickness
	{
		get
		{
			return _sickness;
		}
		 set
		{
			_sickness = value;
			DataKeeper.PlayerGameInfoBackup.Sickness = value;
		}
	}

	public HallucinogenType CurrentHallucinogen { get; private set; }

	public PlayerPhotonSectorController SectorController
	{
		get
		{
			return _mySectorController;
		}
	}

	public string Name
	{
		get
		{
			return _name;
		}
	}

	public double InstTime { get; private set; }

	public bool IsDead
	{
		get
		{
			return ManInfo == null || (short)ManInfo.HealthPoints <= 0;
		}
	}

	public void ToggleAnimator(bool flag)
	{
		_animator.enabled = flag;
	}

	private void Start()
	{
		SetRainCoat();
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			if (GameControls.I != null && GameControls.I.Player != null)
			{
				stream.SendNext(GameControls.I.Player.transform.position);
				stream.SendNext(GameControls.I.Player.transform.rotation);
				stream.SendNext(GameControls.I.Player.GetComponent<Rigidbody>().velocity);
				stream.SendNext(GameControls.I.Player.GetComponent<Rigidbody>().angularVelocity);
			}
			return;
		}

		for (int num = _bufferedState.Length - 1; num >= 1; num--)
		{
			_bufferedState[num] = _bufferedState[num - 1];
		}

		State state           = default(State);
		state.timestamp       = info.timestamp;
		state.pos             = (Vector3)stream.ReceiveNext();
		state.rot             = (Quaternion)stream.ReceiveNext();
		state.velocity        = (Vector3)stream.ReceiveNext();
		state.angularVelocity = (Vector3)stream.ReceiveNext();
		State state2 = state;

		_bufferedState[0] = state2;
		_timestampCount = Mathf.Min(_timestampCount + 1, _bufferedState.Length);

		if (!Application.isEditor)
		{
			return;
		}
		for (int i = 0; i < _timestampCount - 1; i++)
		{
			if (_bufferedState[i].timestamp < _bufferedState[i + 1].timestamp)
			{
				Debug.Log("State inconsistent");
			}
		}
	}

	private void Update()
	{
		if (!base.photonView.isMine)
		{
			double num = PhotonNetwork.time - _interpolationBackTime;
			if (_bufferedState[0].timestamp > num)
			{
				for (int i = 0; i < _timestampCount; i++)
				{
					if (_bufferedState[i].timestamp <= num || i == _timestampCount - 1)
					{
						State state = _bufferedState[Mathf.Max(i - 1, 0)];
						State state2 = _bufferedState[i];
						double num2 = state.timestamp - state2.timestamp;
						float t = 0f;
						if (num2 > 0.0001)
						{
							t = (float)((num - state2.timestamp) / num2);
						}
						base.transform.localPosition = Vector3.Lerp(state2.pos, state.pos, t);
						base.transform.localRotation = Quaternion.Slerp(state2.rot, state.rot, t);
						return;
					}
				}
			}
			else
			{
				State state3 = _bufferedState[0];
				float num3 = (float)(num - state3.timestamp);
				if ((double)num3 < _extrapolationLimit)
				{
					float angle = num3 * state3.angularVelocity.magnitude * 57.29578f;
					Quaternion quaternion = Quaternion.AngleAxis(angle, state3.angularVelocity);
					base.transform.position = state3.pos + state3.velocity * num3;
					base.transform.rotation = quaternion * state3.rot;
					GetComponent<Rigidbody>().velocity = state3.velocity;
					GetComponent<Rigidbody>().angularVelocity = state3.angularVelocity;
				}
			}
		}
		if (base.photonView.isMine && GameControls.I != null && GameControls.I.Player != null)
		{
			DataKeeper.PlayerGameInfoBackup.Position = GameControls.I.Player.transform.position;
		}
		if (!_model.activeSelf || !_isAnimatorHashInitialized)
		{
			return;
		}
		float value = 0f;
		if (base.photonView.isMine)
		{
			if (GameControls.I != null && GameControls.I.Player != null)
			{
				value = GameControls.I.Player.GetComponent<Rigidbody>().velocity.magnitude;
			}
		}
		else
		{
			value = GetComponent<Rigidbody>().velocity.magnitude;
		}
		_animator.SetFloat(_animatorSpeedId, value);
	}

	private void InitializeAnimatorHash()
	{
		_animatorSpeedId = Animator.StringToHash("Speed_f");
		_isAnimatorHashInitialized = true;
	}

	private void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		InitializeAnimatorHash();
		InstTime = info.timestamp;
		_id = info.sender.NickName;
		_name = (string)base.photonView.instantiationData[0];
		_clothingController.SetSkin((Sex)(byte)base.photonView.instantiationData[1], (byte)base.photonView.instantiationData[2], (byte)base.photonView.instantiationData[3]);
		ManInfo = new PhotonManInfo
		{
			HealthPoints = (short)100,
			Equipment = new Dictionary<string, string>()
		};
		if (!WorldController.I.WorldPlayers.ContainsKey(info.sender.NickName))
		{
			WorldController.I.WorldPlayers.Add(info.sender.NickName, this);
			if (base.photonView.isMine)
			{
				PlayerNoiseController component = GetComponent<PlayerNoiseController>();
				component.Initialize();
			}
		}
		else if (WorldController.I.WorldPlayers[info.sender.NickName] == null || WorldController.I.WorldPlayers[info.sender.NickName].InstTime < InstTime)
		{
			WorldController.I.WorldPlayers[info.sender.NickName] = this;
			if (base.photonView.isMine)
			{
				PlayerNoiseController component2 = GetComponent<PlayerNoiseController>();
				component2.Initialize();
			}
		}
		if (base.photonView.isMine && WorldController.I.NeedRestoreInfo)
		{
			ManInfo.HealthPoints = WorldController.I.MyInfo.HealthPoints;
			foreach (KeyValuePair<string, string> item in WorldController.I.MyInfo.Equipment)
			{
				ManInfo.Equipment.Add(item.Key, item.Value);
			}
			InventoryController instance = InventoryController.Instance;
			string equipedId = instance.GetEquipedId("Hand");
			string equipedId2 = instance.GetEquipedId(ClothingBodyPart.Vest.ToString());
			string equipedId3 = instance.GetEquipedId(ClothingBodyPart.Backpack.ToString());
			string equipedId4 = instance.GetEquipedId(ClothingBodyPart.Headwear.ToString());
			string equipedId5 = instance.GetEquipedId(ClothingBodyPart.Bodywear.ToString());
			string equipedId6 = instance.GetEquipedId(ClothingBodyPart.Legwear.ToString());
			base.photonView.RPC("SyncInfo", PhotonTargets.Others, (short)ManInfo.HealthPoints, equipedId, equipedId2, equipedId3, equipedId4, equipedId5, equipedId6);
			OnPlayerRespawn(true);
			WorldController.I.NeedRestoreInfo = false;
		}
		if (base.photonView.isMine)
		{
			GetComponent<Collider>().enabled = false;
			_model.SetActive(false);
			GetComponent<Rigidbody>().isKinematic = true;
		}
		if (base.photonView.isMine)
		{
			DataKeeper.PlayerGameInfoBackup.HitPoints = (short)ManInfo.HealthPoints;
		}
	}

	private void OnDestroy()
	{
		if (WorldController.I.WorldPlayers.ContainsKey(_id) && WorldController.I.WorldPlayers[_id] == this)
		{
			WorldController.I.WorldPlayers.Remove(_id);
		}
	}

	public void OnTextureLoad(List<string> ids)
	{
		_clothingController.OnTextureLoad(ids);
	}

	public void EuqipItem(Clothing info)
	{
		string text = string.Empty;
		switch (info.BodyPart)
		{
		case ClothingBodyPart.Bodywear:
		case ClothingBodyPart.Legwear:
			text = info.Id;
			break;
		case ClothingBodyPart.Backpack:
		case ClothingBodyPart.Headwear:
		case ClothingBodyPart.Vest:
			text = info.Prefab;
			break;
		}
		base.photonView.RPC("EquipClothingSync", PhotonTargets.All, info.BodyPart.ToString(), text);
	}

	public void TakeOffItem(ClothingBodyPart bodyPart)
	{
		base.photonView.RPC("EquipClothingSync", PhotonTargets.All, bodyPart.ToString(), string.Empty);
	}

	private void EnableStateEffects(bool returnToDefault)
	{
		if (returnToDefault)
		{
			ResetStateEffects();
		}
		StartStateEffects();
	}

	private void ResetStateEffects()
	{
		CurrentHunger = 0;
		CurrentThirst = 0;
		CurrentSickness = 0;
		GameControls.I.Walker.staminaForSprintAmt = GameControls.I.Walker.staminaForSprint;
		GameControls.I.Player.hitPoints = 100f;
		if (StatesView.I != null)
		{
			StatesView.I.UpdateState(ManState.HealthPoint, (short)ManInfo.HealthPoints);
			StatesView.I.UpdateState(ManState.Hunger, (int)CurrentHunger);
			StatesView.I.UpdateState(ManState.Thirst, (int)CurrentThirst);
			StatesView.I.UpdateState(ManState.Sickness, (int)CurrentSickness);
		}
	}

	public void StartStateEffects()
	{
		StartCoroutine("Hunger");
		StartCoroutine("Thirst");
		StartCoroutine("StatesEffects");
	}

	private void DisableStateEffects()
	{
		Debug.Log("DisableStateEffects");
		StopAllCoroutines();
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (base.photonView.isMine)
		{
			InventoryController instance = InventoryController.Instance;
			string equipedId = instance.GetEquipedId("Hand");
			string equipedId2 = instance.GetEquipedId(ClothingBodyPart.Vest.ToString());
			string equipedId3 = instance.GetEquipedId(ClothingBodyPart.Backpack.ToString());
			string equipedId4 = instance.GetEquipedId(ClothingBodyPart.Headwear.ToString());
			string equipedId5 = instance.GetEquipedId(ClothingBodyPart.Bodywear.ToString());
			string equipedId6 = instance.GetEquipedId(ClothingBodyPart.Legwear.ToString());
			base.photonView.RPC("SyncInfo", player, (short)ManInfo.HealthPoints, equipedId, equipedId2, equipedId3, equipedId4, equipedId5, equipedId6);
		}
	}

	public void UseHallucinogen(HallucinogenType type)
	{
		if (type != 0)
		{
			StopCoroutine("Hallucination");
			StartCoroutine("Hallucination", type);
		}
	}

	private IEnumerator StatesEffects()
	{
		int damage3 = (((int)CurrentHunger >= 100) ? 10 : 0);
		damage3 += (((int)CurrentThirst >= 100) ? 10 : 0);
		damage3 += (((int)CurrentSickness >= 100) ? 10 : 0);
		yield return new WaitForSeconds(2f);
		if (damage3 > 0)
		{
			base.photonView.RPC("Hit", PhotonTargets.All, (short)damage3, (byte)0, null);
		}
		StartCoroutine("StatesEffects");
	}

	private IEnumerator Hallucination(HallucinogenType type)
	{
		CurrentHallucinogen = type;
		HallucinogenController.I.EnableHallucinogen(CurrentHallucinogen);
		yield return new WaitForSeconds(ItemsRegistry.I.HallucinationTime);
		HallucinogenController.I.DisableHallucinogen();
		CurrentHallucinogen = HallucinogenType.None;
	}

	public void AddSickness(ObscuredByte value)
	{
		CurrentSickness = GetChangedState(CurrentSickness, (byte)value);
	}

	private IEnumerator Hunger()
	{
		yield return new WaitForSeconds(ItemsRegistry.I.HungerStepTime);
		ChangeState(ManState.Hunger, 1);
		StartCoroutine("Hunger");
	}

	private IEnumerator Thirst()
	{
		yield return new WaitForSeconds(ItemsRegistry.I.ThirstStepTime);
		ChangeState(ManState.Thirst, 1);
		StartCoroutine("Thirst");
	}

	private ObscuredInt GetChangedState(ObscuredInt value, short additionalValue)
	{
		return Mathf.Max(0, Mathf.Min(100, (int)value + additionalValue));
	}

	public void ChangeState(ManState state, short value)
	{
		switch (state)
		{
		case ManState.Hunger:
			CurrentHunger = GetChangedState(CurrentHunger, value);
			StatesView.I.UpdateState(state, (int)CurrentHunger);
			break;
		case ManState.Thirst:
			CurrentThirst = GetChangedState(CurrentThirst, value);
			StatesView.I.UpdateState(state, (int)CurrentThirst);
			break;
		case ManState.Sickness:
			CurrentSickness = GetChangedState(CurrentSickness, value);
			StatesView.I.UpdateState(state, (int)CurrentSickness);
			break;
		case ManState.HealthPoint:
			if (value > 0)
			{
				base.photonView.RPC("Heal", PhotonTargets.All, (byte)value);
			}
			else if (value < 0)
			{
				base.photonView.RPC("Hit", PhotonTargets.All, Math.Abs(value), (byte)0, (string)WorldController.I.MyId);
			}
			break;
		case ManState.Stamina:
		{
			float num = (float)value * (float)GameControls.I.Walker.staminaForSprint / 100f;
			GameControls.I.Walker.staminaForSprintAmt = Mathf.Min(GameControls.I.Walker.staminaForSprint, GameControls.I.Walker.staminaForSprintAmt + num);
			break;
		}
		}
	}

	public void HitPlayer(ObscuredShort damage, ObscuredByte radiation, ObscuredString attackerSocialId)
	{
		if (!IsDead)
		{
			base.photonView.RPC("Hit", PhotonTargets.All, (short)damage, (byte)radiation, (string)attackerSocialId);
		}
	}

	public void Suicide()
	{
		if (GameControls.I.Player != null && GameControls.I.Player.inTheCar && GameControls.I.Player.myCurentCarWrapper != null && GameControls.I.Player.myCurrentDoor != null)
		{
			GameControls.I.Player.myCurentCarWrapper.ExitFromCar(GameControls.I.Player.myCurrentDoor.DoorId);
		}
		if (OrbitCameraController.I != null)
		{
			OrbitCameraController.I.SwitchCameras(false);
		}
		base.photonView.RPC("Hit", PhotonTargets.All, (short)1000, (byte)0, (string)WorldController.I.MyId);
	}

	private IEnumerator OnDie(ObscuredString attackerSocialId, PhotonPlayer killer)
	{
		GameControls.I.Player.canUseExit = false;

		if (GameControls.I.Player.inTheCar && GameControls.I.Player.myCurentCarWrapper != null && GameControls.I.Player.myCurrentDoor != null)
		{
			GameControls.I.Player.myCurentCarWrapper.ExitFromCar(GameControls.I.Player.myCurrentDoor.DoorId);
		}

		if (OrbitCameraController.I != null)
		{
			OrbitCameraController.I.SwitchCameras(false);
			OrbitCameraController.I.canUse = false;
		}

		bool killedByPlayer = false;
		string killerName = string.Empty;
		string weaponId = string.Empty;
		PlayerNoiseController.I.ResetNoise();

		if (attackerSocialId == WorldController.I.MyId)
		{
			WorldController.I.PlayerStatistic.Suicide++;
			killedByPlayer = true;
		}
		else
		{
			if (!string.IsNullOrEmpty(attackerSocialId))
			{
				PhotonMan playerKiller = ((!WorldController.I.WorldPlayers.ContainsKey(attackerSocialId)) ? null : WorldController.I.WorldPlayers[attackerSocialId]);
				if ((bool)playerKiller)
				{
					WorldController.I.WorldPlayers[attackerSocialId].photonView.RPC("AddPlayerFrag", killer);
					weaponId = WorldController.I.WorldPlayers[attackerSocialId]._clothingController.ItemInHandId;
					killedByPlayer = true;
					killerName = WorldController.I.WorldPlayers[attackerSocialId].Name;
				}
			}

			WorldController.I.PlayerStatistic.Die++;
		}
		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars)
		{
			BattleRoyaleGameManager.I.CalculateMyFinalePlace();
			if (attackerSocialId != (ObscuredString)JsSpeeker.viewer_id)
			{
				BattleRoyaleGameManager.I.StartShowKillLog(killerName, JsSpeeker.vk_name, weaponId, attackerSocialId);
			}
		}

		HallucinogenController.I.DisableHallucinogen();
		CurrentHallucinogen = HallucinogenType.None;
		InventoryController.Instance.OnDie(killedByPlayer);
		GameUIController.I.ShowCharacterMenu(false, CharacterMenuType.Menu, false);

		yield return new WaitForSeconds(2f);

		DisableStateEffects();
		Debug.Log ("die");
		GameUIController.I.ShowRespawnMenu(true, killerName, weaponId);
		PlayerPrefs.SetString("Die", "true");

		if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
		{
			PlayerPrefs.Save();
		}
	}

	public void Respawn(bool firstSpawn)
	{
		base.photonView.RPC("RespawnPlayer", PhotonTargets.All, firstSpawn);
	}

	public void OnPlayerRespawn(bool afterReconnect)
	{
		base.transform.parent = GameControls.I.Player.transform;
		base.transform.localRotation = Quaternion.identity;
		base.transform.localPosition = Vector3.zero;
		base.transform.localScale = Vector3.one;

		if (base.photonView.isMine && !afterReconnect)
		{
			base.photonView.RPC("OnRespawn", PhotonTargets.Others);
			WorldController.I.CanEnterTheCar = true;
			if (OrbitCameraController.I != null)
			{
				OrbitCameraController.I.canUse = true;
			}
			if (WorldController.I.PlayerSaveHelper.HasSaveInfo && !WorldController.IsDieNotCorrect)
			{
				if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
				{
					WorldController.I.AddItemsFromLoad();
					byte[] playerStates = WorldController.I.PlayerSaveHelper.GetPlayerStates();
					if (playerStates != null)
					{
						SetStates(playerStates[0], playerStates[1], playerStates[2], playerStates[3]);
					}
					WorldController.I.PlayerSaveHelper.ClearStatesInfo();
				}
			}
			else if (DataKeeper.GameType != GameType.Tutorial && DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
			{
				WorldController.I.AddItemsFromPack();
			}
		}

		ResetStateEffects();
		WorldController.I.SavePlayer(false);
	}

	public void SetStates(ObscuredByte hp, ObscuredByte hunger, ObscuredByte thirst, ObscuredByte sickness)
	{
		if ((byte)hp > 0)
		{
			ObscuredByte obscuredByte = (byte)((byte)hp - 100);
			ChangeState(ManState.HealthPoint, (byte)obscuredByte);
		}
		if ((byte)hunger > 0)
		{
			ChangeState(ManState.Hunger, (byte)hunger);
		}
		if ((byte)thirst > 0)
		{
			ChangeState(ManState.Thirst, (byte)thirst);
		}
		if ((byte)sickness > 0)
		{
			ChangeState(ManState.Sickness, (byte)sickness);
		}
	}

	public void PlayerDropItem(ObscuredVector3 position, ObscuredString id, ObscuredInt count, ObscuredByte additionalCount)
	{
		base.photonView.RPC("DropItem", PhotonTargets.MasterClient, (Vector3)position, (string)id, (int)count, (byte)additionalCount);
	}

	public void PlayerPlaceItem(ObscuredVector3 position, ObscuredQuaternion rotation, ObscuredString id)
	{
		short num = (short)WorldController.I.Info.DestructibleObjects.FindIndex((DestructibleObject obj) => (ObscuredString)obj.Id == id);
		base.photonView.RPC("PlaceItem", PhotonTargets.MasterClient, DataKeeper.BackendInfo.user.playerId, (Vector3)position, (Quaternion)rotation, num);
	}

	public void ChangeItemInHand(ObscuredString itemId)
	{
		base.photonView.RPC("ChangeItemInHandView", PhotonTargets.All, (string)itemId);
	}

	public void ShowPlayerAttack(ObscuredVector3? targetPoint)
	{
		if (!(WorldController.I != null) || !(WorldController.I.Player != null))
		{
			return;
		}
		PhotonPlayer[] nearPhotonPlayers = WorldController.I.Player.SectorController.GetNearPhotonPlayers();
		if (nearPhotonPlayers != null)
		{
			List<PhotonPlayer> list = new List<PhotonPlayer>(nearPhotonPlayers);
			if (list.Contains(PhotonNetwork.player))
			{
				list.Remove(PhotonNetwork.player);
			}
			if (targetPoint.HasValue)
			{
				base.photonView.RPC("ShowAttackAtPoint", list[0], (short)targetPoint.Value.x, (short)targetPoint.Value.y, (short)targetPoint.Value.z);
			}
			else
			{
				base.photonView.RPC("ShowAttack", list[0]);
			}
		}
	}

	private void CopyTransformsRecurse(Transform src, Transform dst)
	{
		dst.position = src.position;
		dst.rotation = src.rotation;
		foreach (Transform item in dst)
		{
			Transform transform2 = src.Find(item.name);
			if ((bool)transform2)
			{
				CopyTransformsRecurse(transform2, item);
			}
		}
	}

	private void SetRagdollPositions(GameObject ragdoll, ObscuredString killerSocId)
	{
		CopyTransformsRecurse(_model.transform, ragdoll.transform);
		if (killerSocId != null && killerSocId != WorldController.I.MyId)
		{
			PhotonMan photonMan = ((!WorldController.I.WorldPlayers.ContainsKey(killerSocId)) ? null : WorldController.I.WorldPlayers[killerSocId]);
			if ((bool)photonMan)
			{
				Vector3 vector = photonMan.transform.position - base.transform.position;
				RaycastHit hitInfo;
				if (Physics.SphereCast(photonMan.transform.position, 0.2f, vector, out hitInfo, 750f, _ragdollRaymask) && (bool)hitInfo.rigidbody)
				{
					hitInfo.rigidbody.AddForce(vector * 50f, ForceMode.Impulse);
				}
				else
				{
					Rigidbody[] componentsInChildren = ragdoll.GetComponentsInChildren<Rigidbody>();
					if (componentsInChildren != null)
					{
						Rigidbody[] array = componentsInChildren;
						foreach (Rigidbody rigidbody in array)
						{
							if (rigidbody.transform.name == "Body_jnt" || rigidbody.transform.name == "Spine_jnt")
							{
								rigidbody.AddForce((base.transform.position - photonMan.transform.position).normalized * 30f, ForceMode.Impulse);
								break;
							}
						}
					}
				}
			}
		}
		RemoveBody component = ragdoll.GetComponent<RemoveBody>();
		if ((bool)component)
		{
			component.bodyStayTime = 15f;
			component.enabled = true;
		}
	}

	private void SetRainCoat()
	{
		if (!string.IsNullOrEmpty(Name) && Name.Contains("CFFC300FF VIP"))
		{
			_rainCoatManager.SetActive(true);
		}
	}

	[PunRPC]
	private void ShowAttackAtPoint(short targetX, short targetY, short targetZ)
	{
		if (WorldController.I == null || WorldController.I.Player == null)
		{
			return;
		}
		if (_clothingController == null)
		{
			Debug.LogError("_clothingController == null");
		}
		if (WorldController.I.Info == null)
		{
			Debug.LogError("WorldController.I.Info == null");
		}
		if (ItemsRegistry.I == null)
		{
			Debug.LogError("ItemsRegistry.I == null");
		}
		if (GameControls.I == null)
		{
			Debug.LogError("GameControls.I == null");
		}
		Weapon weapon = null;
		int num = 1;
		float maxDistance = 3f;
		if (!string.IsNullOrEmpty(_clothingController.ItemInHandId))
		{
			weapon = WorldController.I.Info.GetWeaponInfo(_clothingController.ItemInHandId);
			if (weapon != null)
			{
				num = ItemsRegistry.I.GetWeaponIndex(weapon.Id);
				maxDistance = weapon.Range;
			}
		}
		if (num < GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count)
		{
			WeaponBehavior weaponBehavior = GameControls.I.PlayerWeapons.WeaponsBehavioursList[num];
			if (WorldController.I.Player != null && Vector3.Distance(WorldController.I.Player.transform.position, base.transform.position) < 100f)
			{
				_weaponAudioSource.clip = weaponBehavior.fireSnd;
				_weaponAudioSource.pitch = UnityEngine.Random.Range(0.96f * Time.timeScale, 1f * Time.timeScale);
				_weaponAudioSource.PlayOneShot(_weaponAudioSource.clip, 0.9f / _weaponAudioSource.volume);
				_thirdPersonAttackController.ShowAttackAnimation(weapon);
			}
			Vector3 vector = new Vector3(targetX, targetY, targetZ);
			Vector3 direction = vector - base.transform.position;
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.position, direction, out hitInfo, maxDistance, weaponBehavior.bulletMask) && WorldController.I.Player != null && Vector3.Distance(WorldController.I.Player.transform.position, hitInfo.point) < 40f)
			{
				weaponBehavior.Effects.ImpactEffects(hitInfo, weaponBehavior);
				weaponBehavior.Effects.BulletMarks(hitInfo, weaponBehavior);
			}
		}
	}

	[PunRPC]
	private void ShowAttack()
	{
		if (WorldController.I == null || WorldController.I.Player == null)
		{
			return;
		}
		if (_clothingController == null)
		{
			Debug.LogError("_clothingController == null");
		}
		if (WorldController.I.Info == null)
		{
			Debug.LogError("WorldController.I.Info == null");
		}
		if (ItemsRegistry.I == null)
		{
			Debug.LogError("ItemsRegistry.I == null");
		}
		if (GameControls.I == null)
		{
			Debug.LogError("GameControls.I == null");
		}
		if (WorldController.I.Player == null)
		{
			Debug.LogError("WorldController.I.Player == null");
		}
		Weapon weapon = null;
		int num = 1;
		if (!string.IsNullOrEmpty(_clothingController.ItemInHandId))
		{
			weapon = WorldController.I.Info.GetWeaponInfo(_clothingController.ItemInHandId);
			if (weapon != null)
			{
				num = ItemsRegistry.I.GetWeaponIndex(weapon.Id);
			}
		}
		if (num < GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count)
		{
			WeaponBehavior weaponBehavior = GameControls.I.PlayerWeapons.WeaponsBehavioursList[num];
			if (WorldController.I.Player != null && Vector3.Distance(WorldController.I.Player.transform.position, base.transform.position) < 100f)
			{
				_weaponAudioSource.clip = weaponBehavior.fireSnd;
				_weaponAudioSource.pitch = UnityEngine.Random.Range(0.96f * Time.timeScale, 1f * Time.timeScale);
				_weaponAudioSource.PlayOneShot(_weaponAudioSource.clip, 0.9f / _weaponAudioSource.volume);
				_thirdPersonAttackController.ShowAttackAnimation(weapon);
			}
		}
	}

	[PunRPC]
	private void AddPlayerFrag()
	{
		WorldController.I.PlayerStatistic.PlayerKills++;
	}

	[PunRPC]
	private void AddZombieFrag()
	{
		WorldController.I.PlayerStatistic.ZombieKills++;
		if (WorldController.I.PlayerStatistic.ZombieKills > 0 && WorldController.I.PlayerStatistic.ZombieKills % 10 == 0)
		{
			int num = WorldController.I.PlayerStatistic.ZombieKills / 10;
			Debug.Log("setting level: " + num + " (user_id=" + JsSpeeker.viewer_id + ")");
		}
	}

	[PunRPC]
	private void PlaceItem(int playerUId, Vector3 position, Quaternion rotation, short itemIndex)
	{
		if (PhotonNetwork.isMasterClient && itemIndex >= 0 && itemIndex < WorldController.I.Info.DestructibleObjects.Count)
		{
			DestructibleObject destructibleObject = WorldController.I.Info.DestructibleObjects[itemIndex];
			if (WorldController.I.Info.DestructibleObjects.Count > itemIndex)
			{
				PhotonNetwork.InstantiateSceneObject("WorldObjects/" + destructibleObject.Prefab, position, rotation, 0, new object[5] { playerUId, itemIndex, null, null, null });
			}
		}
	}

	[PunRPC]
	private void DropItem(Vector3 position, string id, int count, byte additionalCount)
	{
		if (PhotonNetwork.isMasterClient)
		{
			Item itemInfo = WorldController.I.Info.GetItemInfo(id);
			RaycastHit hitInfo;
			if (itemInfo != null && Physics.Raycast(position, -Vector3.up, out hitInfo, 100f, ItemsRegistry.I.ItemsCollisionsMask))
			{
				Quaternion rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, hitInfo.normal), Vector3.Cross(Vector3.up, hitInfo.normal));
				PhotonNetwork.InstantiateSceneObject("PhotonItem", hitInfo.point, rotation, 0, new object[4]
				{
					itemInfo.Id,
					(byte)count,
					true,
					additionalCount
				});
			}
		}
	}

	[PunRPC]
	private void OnRespawn()
	{
		_model.SetActive(true);
	}

	[PunRPC]
	private void RespawnPlayer(bool firstSpawn)
	{
		ManInfo.HealthPoints = (short)100;
		if (base.photonView.isMine)
		{
			DataKeeper.PlayerGameInfoBackup.HitPoints = (short)ManInfo.HealthPoints;
			if (DataKeeper.GameType != GameType.Tutorial && DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
			{
				EnableStateEffects(!firstSpawn);
			}
			if (!firstSpawn)
			{
				GameUIController.I.ShowRespawnMenu(false);
			}
			PlayerSpawnsController.I.OnRespawn();
			GameControls.I.Walker.climbing = false;
			GameControls.I.Walker.noClimbingSfx = false;
		}
	}

	[PunRPC]
	private void SyncInfo(short hp, string hand, string vest, string backpack, string head, string body, string legs)
	{
		ManInfo.HealthPoints = hp;
		if (base.photonView.isMine)
		{
			DataKeeper.PlayerGameInfoBackup.HitPoints = (short)ManInfo.HealthPoints;
		}
		Clothing clothingInfo = WorldController.I.Info.GetClothingInfo(vest);
		if (clothingInfo != null)
		{
			_clothingController.Equip(ClothingBodyPart.Vest, clothingInfo.Prefab, base.photonView.isMine);
		}
		Clothing clothingInfo2 = WorldController.I.Info.GetClothingInfo(backpack);
		if (clothingInfo2 != null)
		{
			_clothingController.Equip(ClothingBodyPart.Backpack, clothingInfo2.Prefab, base.photonView.isMine);
		}
		Clothing clothingInfo3 = WorldController.I.Info.GetClothingInfo(head);
		if (clothingInfo3 != null)
		{
			_clothingController.Equip(ClothingBodyPart.Headwear, clothingInfo3.Prefab, base.photonView.isMine);
		}
		_clothingController.Equip(ClothingBodyPart.Bodywear, body, base.photonView.isMine);
		_clothingController.Equip(ClothingBodyPart.Legwear, legs, base.photonView.isMine);
		if (!base.photonView.isMine)
		{
			_clothingController.AddInHand(hand);
		}
		_model.SetActive(!base.photonView.isMine);
		if ((short)ManInfo.HealthPoints <= 0 && _model.activeSelf)
		{
			_model.SetActive(false);
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(_ragdoll, base.transform.position, base.transform.rotation);
			_clothingController.SetRagdollView(gameObject);
			SetRagdollPositions(gameObject, null);
		}
	}

	[PunRPC]
	private void Heal(byte value)
	{
		if ((short)ManInfo.HealthPoints > 0)
		{
			ManInfo.HealthPoints = (short)Mathf.Min(100, (short)ManInfo.HealthPoints + value);
			if (base.photonView.isMine)
			{
				DataKeeper.PlayerGameInfoBackup.HitPoints = (short)ManInfo.HealthPoints;
				StatesView.I.UpdateState(ManState.HealthPoint, (short)ManInfo.HealthPoints);
				GameControls.I.Player.HealPlayer((int)value);
			}
		}
	}

	[PunRPC]
	private void Hit(short damage, byte radiation, string attackerSocialId, PhotonMessageInfo info)
	{
		if (((DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars) && BattleRoyaleGameManager.I != null && BattleRoyaleGameManager.I.IsLobby()) || IsDead)
		{
			return;
		}

		if (!string.IsNullOrEmpty(attackerSocialId) && _clothingController.IsEquiped(ClothingBodyPart.Vest) && info.photonView.ownerId != base.photonView.ownerId)
		{
			damage = (short)Mathf.Min(1, (short)Math.Ceiling((float)damage * 0.8f));
		}

		ManInfo.HealthPoints = (short)Mathf.Max(0, (short)ManInfo.HealthPoints - damage);

		if (base.photonView.isMine)
		{
			DataKeeper.PlayerGameInfoBackup.HitPoints = (short)ManInfo.HealthPoints;
		}

		if (IsDead)
		{
			_model.SetActive(false);
			GameObject ragdoll = (GameObject)Instantiate(_ragdoll, base.transform.position, base.transform.rotation);

			_clothingController.SetRagdollView(ragdoll);
			SetRagdollPositions(ragdoll, attackerSocialId);
		}

		if (!base.photonView.isMine)
		{
			return;
		}

		StatesView.I.UpdateState(ManState.HealthPoint, (short)ManInfo.HealthPoints);

		if (damage != 0)
		{
			GameControls.I.Player.ApplyDamage(damage, delegate
			{
				StartCoroutine(OnDie(attackerSocialId, info.sender));
			});
		}

		if (radiation > 0)
		{
			ChangeState(ManState.Sickness, radiation);
		}

		if ((short)ManInfo.HealthPoints != (short)(float)GameControls.I.Player.hitPoints)
		{
			Debug.Log(string.Concat("HEALTH IN PHOTON MAN ", ManInfo.HealthPoints, " IN REALISTIC ", GameControls.I.Player.hitPoints));
		}

		PushPlayer(attackerSocialId, damage);
	}

	private void PushPlayer(string attackerSocialId, short damage)
	{
		if (DataKeeper.GameType == GameType.SkyWars && attackerSocialId != null && attackerSocialId != "fogdamage")
		{
			Vector3 vector = (GameControls.I.Player.transform.position - WorldController.I.WorldPlayers[attackerSocialId].transform.position).normalized;
			if (damage == 0)
			{
				vector = -vector;
			}
			GameControls.I.Player.GetComponent<Rigidbody>().AddForce(vector * SkyWarsSetupOptions.HitForce, ForceMode.Force);
		}
	}

	[PunRPC]
	private void ChangeItemInHandView(string info)
	{
		if (!string.IsNullOrEmpty(info))
		{
			_clothingController.AddInHand(info);
		}
		else
		{
			_clothingController.RemoveFromHand();
		}
	}

	[PunRPC]
	private void EquipClothingSync(string bodyPart, string info)
	{
		ClothingBodyPart bodyPart2 = (ClothingBodyPart)(byte)Enum.Parse(typeof(ClothingBodyPart), bodyPart);
		if (string.IsNullOrEmpty(info))
		{
			_clothingController.TakeOff(bodyPart2);
		}
		else
		{
			_clothingController.Equip(bodyPart2, info, base.photonView.isMine);
		}
	}

	public void EnterTheCar(bool flag, byte carId, byte doorId)
	{
		base.photonView.RPC("EnterTheCarRpc", PhotonTargets.Others, flag, carId, doorId);
	}

	[PunRPC]
	public void EnterTheCarRpc(bool flag, byte carId, byte doorId)
	{
		if (flag)
		{
			rigBody.isKinematic = true;
			capsCollider.isTrigger = true;
			CarWrapper carWrapperById = GlobalCarManager.I.GetCarWrapperById(carId);

			if (carWrapperById != null)
			{
				base.transform.parent = carWrapperById.seats[doorId].seat_transform;
				StartCoroutine("WaitAndSetDriverPos");
			}
		}
		else
		{
			PhotonManDeparentFromCar();
		}
	}

	public void PhotonManDeparentFromCar()
	{
		rigBody.isKinematic = false;
		capsCollider.isTrigger = false;
		base.transform.parent = null;
	}

	private IEnumerator WaitAndSetDriverPos()
	{
		yield return new WaitForSeconds(0.5f);
		base.transform.localPosition = new Vector3(0f, 0f, 0f);
		base.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
	}

	public void SeatInCarOnStart(byte carId, byte doorId)
	{
		rigBody.isKinematic = true;
		CarWrapper carWrapperById = GlobalCarManager.I.GetCarWrapperById(carId);
		if (carWrapperById != null)
		{
			Debug.LogError("Enter the car");
			base.transform.parent = carWrapperById.seats[doorId].seat_transform;
			base.transform.localPosition = new Vector3(0f, 0f, 0f);
			base.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
		}
		capsCollider.isTrigger = true;
	}

	public void CallShowParachute(bool flag)
	{
		base.photonView.RPC("ShowParachute", PhotonTargets.Others, flag);
	}

	[PunRPC]
	public void ShowParachute(bool flag)
	{
		if (_parachute != null)
		{
			_parachute.gameObject.SetActive(flag);
		}
	}
}
