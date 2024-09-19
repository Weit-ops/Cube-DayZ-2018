using Photon;
using UnityEngine;

public enum NoiseType
{
	Shot = 0,
	Run = 1,
	HitZombie = 2
}

public class PlayerNoiseController : Photon.MonoBehaviour, IPunObservable
{
	public static PlayerNoiseController I;

	[SerializeField] float _noiseFadeSpeed;
	[SerializeField] int _runNoise;
	[SerializeField] int _shotNoise;
	[SerializeField] int _hitZombieNoise;
	[SerializeField] int _runMaxNoise;
	[SerializeField] int _shotMaxNoise;
	[SerializeField] int _hitZombieMaxNoise;
	[SerializeField] SphereCollider _noise;

	private short _defaultNoise;
	private float _currentRunNoise;
	private float _currentShotNoise;
	private float _currentHitZombieNoise;
	private bool _needUpdateNoise;

	public void Initialize()
	{
		I = this;
		_defaultNoise = (short)_noise.radius;
	}

	public void AddNoise(NoiseType type)
	{
		switch (type)
		{
		case NoiseType.Shot:
			_currentShotNoise = Mathf.Min(_currentShotNoise + (float)_shotNoise, _shotMaxNoise);
			break;
		case NoiseType.Run:
			_currentRunNoise = Mathf.Min(_currentRunNoise + (float)_runNoise, _runMaxNoise);
			break;
		case NoiseType.HitZombie:
			_currentHitZombieNoise = Mathf.Min(_currentHitZombieNoise + (float)_hitZombieNoise, _hitZombieMaxNoise);
			break;
		}
		_needUpdateNoise = true;
	}

	public void ResetNoise()
	{
		_currentRunNoise = 0f;
		_currentShotNoise = 0f;
		_currentHitZombieNoise = 0f;
		_noise.radius = _defaultNoise;
	}

	private void FadeNoise(ref float noise)
	{
		if (noise > 0f)
		{
			noise = Mathf.Lerp(noise, 0f, _noiseFadeSpeed * Time.deltaTime);
			_needUpdateNoise = true;
		}
	}

	private void Update()
	{
		if (base.photonView.isMine)
		{
			FadeNoise(ref _currentRunNoise);
			FadeNoise(ref _currentShotNoise);
			FadeNoise(ref _currentHitZombieNoise);
			if (_needUpdateNoise)
			{
				_noise.radius = (short)((float)_defaultNoise + _currentRunNoise + _currentShotNoise + _currentHitZombieNoise);
				_needUpdateNoise = false;
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(_noise.radius);
		}
		else
		{
			_noise.radius = (float)stream.ReceiveNext();
		}
	}
}
