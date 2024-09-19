using UnityEngine;

public class TOD_CameraEffects : MonoBehaviour
{
	[SerializeField]
	private TOD_Sky _todSky;

	[SerializeField]
	private float bloomFadeTime = 10f;

	private float lerpTime;

	private FastBloom _fastBloomEffect;

	private bool _isInitialized;

	private void Initialize()
	{
		_fastBloomEffect = GetComponent<FastBloom>();
		_isInitialized = true;
	}

	private void Update()
	{
		if (!_isInitialized)
		{
			Initialize();
		}
		if (_fastBloomEffect != null)
		{
			int num = ((!_todSky.IsDay) ? 1 : (-1));
			lerpTime = Mathf.Clamp01(lerpTime + (float)num * Time.deltaTime / bloomFadeTime);
			_fastBloomEffect.intensity = Mathf.Lerp(0f, 2.5f, lerpTime);
		}
	}
}
