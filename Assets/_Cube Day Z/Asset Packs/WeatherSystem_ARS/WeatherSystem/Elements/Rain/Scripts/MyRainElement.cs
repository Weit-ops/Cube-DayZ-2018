using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MyRainElement : WeatherElement_Shuriken
{
	private Color _startColor;

	private float dayAlpha = 50f;

	private float nightAlpha = 10f;

	private float lerpTime;

	public float DayAlpha
	{
		get
		{
			return dayAlpha;
		}
		set
		{
			dayAlpha = value;
		}
	}

	public float NightAlpha
	{
		get
		{
			return nightAlpha;
		}
		set
		{
			nightAlpha = value;
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		_startColor = GetComponent<ParticleSystem>().main.startColor.color;
		base.IsInitialized = true;
	}

	protected override void Start()
	{
		base.Start();
	}

	protected override void Update()
	{
		base.Update();
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (base.WeatherSystem.isTODPresent && base.IsInitialized && base.EnableElement)
		{
			float num = 0f;
			if (base.Sky.Cycle.Hour > 6f && base.Sky.Cycle.Hour < 18f)
			{
				lerpTime = Mathf.Clamp01(lerpTime + Time.deltaTime);
				num = Mathf.Lerp(nightAlpha / 255f, dayAlpha / 255f, lerpTime);
				GetComponent<ParticleSystem>().GetComponent<Renderer>().material.SetColor("_TintColor", new Color(_startColor.r, _startColor.g, _startColor.b, num));
			}
			else
			{
				lerpTime = Mathf.Clamp01(lerpTime - Time.deltaTime);
				num = Mathf.Lerp(nightAlpha / 255f, dayAlpha / 255f, lerpTime);
				GetComponent<ParticleSystem>().GetComponent<Renderer>().material.SetColor("_TintColor", new Color(_startColor.r, _startColor.g, _startColor.b, num));
			}
		}
	}

	public override void Reset()
	{
		base.Reset();
	}
}
