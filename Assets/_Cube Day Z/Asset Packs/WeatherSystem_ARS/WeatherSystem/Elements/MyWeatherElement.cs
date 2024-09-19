using UnityEngine;

public class MyWeatherElement : MonoBehaviour
{
	private TOD_Sky sky;

	private TOD_Weather weather;

	private TOD_Animation windAnimation;

	private WeatherSystem weatherSystem;

	private GameObject cameraObject;

	private GameObject atmosphere;

	private bool activateElement;

	private bool enableElement;

	private bool doFadeIn;

	private float timeScaleDivisor;

	private float fadeTime;

	private bool isFadedIn;

	private bool isInitialized;

	private bool isBaseInitialized;

	private AudioClip[] elementSounds;

	private AudioClip currentElementSound;

	private float maxAudioLevel = 1f;

	private bool setNewSound = true;

	private float heightAboveCamera;

	private Vector3 position = Vector3.zero;

	public TOD_Sky Sky
	{
		get
		{
			return sky;
		}
		set
		{
			sky = value;
		}
	}

	public TOD_Weather Weather
	{
		get
		{
			return weather;
		}
		set
		{
			weather = value;
		}
	}

	public WeatherSystem WeatherSystem
	{
		get
		{
			return weatherSystem;
		}
		set
		{
			weatherSystem = value;
		}
	}

	public TOD_Animation WindAnimation
	{
		get
		{
			return windAnimation;
		}
		set
		{
			windAnimation = value;
		}
	}

	public GameObject CameraObject
	{
		get
		{
			return cameraObject;
		}
		set
		{
			cameraObject = value;
		}
	}

	public GameObject Atmosphere
	{
		get
		{
			return atmosphere;
		}
		set
		{
			atmosphere = value;
		}
	}

	public bool EnableElement
	{
		get
		{
			return enableElement;
		}
		set
		{
			enableElement = value;
		}
	}

	public bool ActivateElement
	{
		get
		{
			return activateElement;
		}
		set
		{
			activateElement = value;
		}
	}

	public bool DoFadeIn
	{
		get
		{
			return doFadeIn;
		}
		set
		{
			doFadeIn = value;
		}
	}

	public float FadeTime
	{
		get
		{
			return fadeTime;
		}
		set
		{
			fadeTime = value;
		}
	}

	public bool IsFadedIn
	{
		get
		{
			return isFadedIn;
		}
		set
		{
			isFadedIn = value;
		}
	}

	public bool IsInitialized
	{
		get
		{
			return isInitialized;
		}
		set
		{
			isInitialized = value;
		}
	}

	public float TimeScaleDivisor
	{
		get
		{
			return timeScaleDivisor;
		}
		set
		{
			timeScaleDivisor = value;
		}
	}

	public float HeightAboveCamera
	{
		get
		{
			return heightAboveCamera;
		}
		set
		{
			heightAboveCamera = value;
		}
	}

	public AudioClip[] ElementSounds
	{
		get
		{
			return elementSounds;
		}
		set
		{
			elementSounds = value;
		}
	}

	public AudioClip CurrentElementSound
	{
		get
		{
			return currentElementSound;
		}
		set
		{
			currentElementSound = value;
		}
	}

	public float MaxAudioLevel
	{
		get
		{
			return maxAudioLevel;
		}
		set
		{
			maxAudioLevel = value;
		}
	}

	public bool SetNewSound
	{
		get
		{
			return setNewSound;
		}
		set
		{
			setNewSound = true;
		}
	}

	public virtual void Initialize()
	{
		weatherSystem = (WeatherSystem)Object.FindObjectOfType(typeof(WeatherSystem));
		sky = (TOD_Sky)Object.FindObjectOfType(typeof(TOD_Sky));
		weather = sky.GetComponent<TOD_Weather>();
		windAnimation = sky.GetComponent<TOD_Animation>();
		cameraObject = Camera.main.gameObject;
		if ((bool)cameraObject)
		{
			position = cameraObject.transform.position;
			position.y += heightAboveCamera;
			base.transform.position = position;
		}
		isBaseInitialized = true;
	}

	protected virtual void Awake()
	{
	}

	protected virtual void Start()
	{
		if (!isBaseInitialized)
		{
			weatherSystem = (WeatherSystem)Object.FindObjectOfType(typeof(WeatherSystem));
			sky = (TOD_Sky)Object.FindObjectOfType(typeof(TOD_Sky));
			weather = sky.GetComponent<TOD_Weather>();
			cameraObject = Camera.main.gameObject;
		}
	}

	protected virtual void Update()
	{
		if ((bool)cameraObject)
		{
			position = CameraObject.transform.position;
			position.y += HeightAboveCamera;
			base.transform.position = position;
		}
	}

	protected virtual void LateUpdate()
	{
	}

	public virtual void Transition(bool fadeIn)
	{
		if (weatherSystem.isTODPresent && isInitialized && enableElement)
		{
			activateElement = true;
			doFadeIn = fadeIn;
			if (doFadeIn && elementSounds.Length > 0)
			{
				currentElementSound = elementSounds[UnityEngine.Random.Range(0, elementSounds.Length - 1)];
				setNewSound = true;
			}
		}
	}

	public virtual void Reset()
	{
		GetComponent<AudioSource>().volume = 0f;
		GetComponent<AudioSource>().Stop();
		activateElement = false;
		doFadeIn = false;
		fadeTime = 0f;
		isFadedIn = false;
	}
}
