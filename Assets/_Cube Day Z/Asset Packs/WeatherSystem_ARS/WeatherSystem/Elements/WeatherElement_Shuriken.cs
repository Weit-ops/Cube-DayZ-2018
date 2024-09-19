using UnityEngine;

public class WeatherElement_Shuriken : WeatherElement
{
	private Vector3 originalElementPosition = Vector3.zero;

	public int ParticlesCount { get; set; }

	public override void Initialize()
	{
		base.Initialize();
		GetComponent<AudioSource>().loop = true;
		GetComponent<AudioSource>().volume = 0f;
		base.CurrentElementSound = base.ElementSounds[UnityEngine.Random.Range(0, base.ElementSounds.Length - 1)];
		GetComponent<ParticleSystem>().Stop();
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		if (!base.WeatherSystem.isTODPresent || !base.IsInitialized || !base.EnableElement)
		{
			return;
		}
		base.Update();
		if (!base.ActivateElement || (base.DoFadeIn && base.IsFadedIn) || (!base.DoFadeIn && !base.IsFadedIn))
		{
			return;
		}
		if (!GetComponent<ParticleSystem>().isPlaying)
		{
			GetComponent<ParticleSystem>().Play();
		}
		base.FadeTime += Time.deltaTime / base.TimeScaleDivisor;
		if (base.SetNewSound)
		{
			GetComponent<AudioSource>().clip = base.CurrentElementSound;
			if (!GetComponent<AudioSource>().isPlaying)
			{
				GetComponent<AudioSource>().Play();
			}
			base.SetNewSound = false;
		}
		if (base.DoFadeIn)
		{
			if (base.ElementSounds.Length > 0)
			{
				GetComponent<AudioSource>().volume = Mathf.Lerp(0f, base.MaxAudioLevel, base.FadeTime);
			}
		}
		else if (!base.DoFadeIn && base.ElementSounds.Length > 0)
		{
			GetComponent<AudioSource>().volume = Mathf.Lerp(base.MaxAudioLevel, 0f, base.FadeTime);
		}
		if (base.FadeTime > 1f)
		{
			base.IsFadedIn = base.DoFadeIn;
			base.ActivateElement = false;
			base.FadeTime = 0f;
			if (!base.DoFadeIn)
			{
				GetComponent<ParticleSystem>().Stop();
			}
			if (!base.DoFadeIn && GetComponent<AudioSource>().isPlaying)
			{
				GetComponent<AudioSource>().Stop();
			}
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
	}

	public override void Transition(bool fadeIn)
	{
		base.Transition(fadeIn);
		if (fadeIn && originalElementPosition == Vector3.zero)
		{
			originalElementPosition = base.transform.position;
		}
	}

	public override void Reset()
	{
		base.Reset();
		GetComponent<AudioSource>().Stop();
		GetComponent<ParticleSystem>().Stop();
	}
}
