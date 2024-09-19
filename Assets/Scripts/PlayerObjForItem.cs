using UnityEngine;

public class PlayerObjForItem : ItemEffectInitializer
{
	[SerializeField] GameObject _objForPlayer;
	[SerializeField] Vector3 _localPosition;
	[SerializeField] Vector3 _localRotation;

	private GameObject _currentAddedLight;

	public override void Initialize()
	{
		if (!(Camera.main == null))
		{
			_currentAddedLight = Object.Instantiate(_objForPlayer);
			_currentAddedLight.transform.parent = Camera.main.transform;
			_currentAddedLight.transform.localPosition = _localPosition;
			_currentAddedLight.transform.localRotation = Quaternion.Euler(_localRotation);
			_currentAddedLight.transform.localScale = Vector3.one;
		}
	}

	private void OnDestroy()
	{
		if (_currentAddedLight != null)
		{
			Object.Destroy(_currentAddedLight);
		}
	}
}
