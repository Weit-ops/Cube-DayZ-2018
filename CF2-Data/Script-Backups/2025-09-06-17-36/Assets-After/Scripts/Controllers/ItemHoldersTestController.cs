using System.Collections;
using UnityEngine;

public class ItemHoldersTestController : MonoBehaviour
{
	private const float HandAttackWaitTime   = 1f;
	private const float MuzzleFlashReduction = 6.5f;
	public  const float MuzzleLightDelay     = 0.1f;
	public  const float MuzzleLightReduction = 100f;

	[SerializeField] GameObject _item;
	[SerializeField] Transform _itemHolder;
	[SerializeField] ThirdPlayerModelController _modelController;
	[SerializeField] Animation _animation;
	[SerializeField] Animator _characterAnimator;
	[SerializeField] float _moveSpeed;

	private GameObject _currentItemInHand;
	private PositionInHandInfo _itemViewInfo;
	private float _startShootTime;

	private void Update()
	{
		if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.F))
		{
			if (_currentItemInHand != null)
			{
				Object.Destroy(_currentItemInHand);
			}

			_currentItemInHand = Object.Instantiate(_item);
			_itemViewInfo = _currentItemInHand.GetComponent<PositionInHandInfo>();
			_currentItemInHand.transform.parent = _itemHolder;
			_currentItemInHand.transform.localPosition = _itemViewInfo.ItemPositionOffset;
			_currentItemInHand.transform.localRotation = Quaternion.Euler(_itemViewInfo.ItemRotationOffset);
			_currentItemInHand.transform.localScale = _itemViewInfo.ItemScaleOffset;
			_modelController.EnableIk(_itemViewInfo.LeftHandPoint, _itemViewInfo.RightHandPoint);
		}

		if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.G))
		{
			_modelController.DisableIk();

			if (_currentItemInHand != null)
			{
				Object.Destroy(_currentItemInHand);
			}
		}

		if (ControlFreak2.CF2Input.GetMouseButtonDown(0))
		{
			if (_currentItemInHand != null)
			{
				if ((bool)_itemViewInfo.MuzzleFlash)
				{
					ShowMuzzleFlash();
					_animation.Play("RangeAttack");
				} else {
					_animation.Play("MeleeAttack");
				}
			} else {
				StopCoroutine("HandPunchProcess");
				_characterAnimator.SetBool("HandPunch", true);
				StartCoroutine("HandPunchProcess");
			}
		}

		if (ControlFreak2.CF2Input.GetKey(KeyCode.W))
		{
			_characterAnimator.SetFloat("Speed_f", _moveSpeed);
		}

		FadeMuzzleFlashProcess();
	}

	private void ShowMuzzleFlash()
	{
		if ((bool)_itemViewInfo && (bool)_itemViewInfo.MuzzleFlash)
		{
			if ((bool)_itemViewInfo.MuzzleFlashLight)
			{
				_itemViewInfo.MuzzleFlashLight.enabled = true;
				_itemViewInfo.MuzzleFlashLight.intensity = 8f;
			} 

			_itemViewInfo.MuzzleFlash.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, UnityEngine.Random.Range(0.4f, 0.5f)));
			_itemViewInfo.MuzzleFlash.localRotation = Quaternion.AngleAxis(UnityEngine.Random.value * 360f, Vector3.forward);
			_itemViewInfo.MuzzleFlash.GetComponent<Renderer>().enabled = true;
			_startShootTime = Time.time;
		}
	}

	private void FadeMuzzleFlashProcess()
	{
		if (!_itemViewInfo)
		{
			return;
		}

		if ((bool)_itemViewInfo.MuzzleFlash && _itemViewInfo.MuzzleFlash.GetComponent<Renderer>().enabled)
		{
			Color color = _itemViewInfo.MuzzleFlash.GetComponent<Renderer>().material.GetColor("_TintColor");
			if (color.a > 0f)
			{
				color.a -= 6.5f * Time.deltaTime;
				_itemViewInfo.MuzzleFlash.GetComponent<Renderer>().material.SetColor("_TintColor", color);
			} else {
				_itemViewInfo.MuzzleFlash.GetComponent<Renderer>().enabled = false;
			}
		}

		if (!_itemViewInfo.MuzzleFlashLight || !_itemViewInfo.MuzzleFlashLight.enabled)
		{
			return;
		}

		if (_itemViewInfo.MuzzleFlashLight.intensity > 0f) {
			if (_startShootTime + 0.1f < Time.time) {
				_itemViewInfo.MuzzleFlashLight.intensity -= 100f * Time.deltaTime;
			}
		} else {
			_itemViewInfo.MuzzleFlashLight.enabled = false;
		}
	}

	private IEnumerator HandPunchProcess() {
		yield return new WaitForSeconds(1f);
		_characterAnimator.SetBool("HandPunch", false);
	}
}
