using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract Class for all Weather Elements
/// </summary>
public abstract class WeatherElement : MonoBehaviour
{
    private TOD_Sky sky;
    private TOD_Weather weather;
    private TOD_Animation windAnimation;
    private WeatherSystem weatherSystem;
    private GameObject cameraObject;
    private GameObject atmosphere = null;
    private Light sunLight = null;

    private bool activateElement = false;
    private bool enableElement = false;
    private bool doFadeIn = false;
    private float timeScaleDivisor = 0.0f;
    private float fadeTime = 0.0f;
    private bool isFadedIn = false;
    private bool isInitialized = false;
    private bool isBaseInitialized = false;

    private AudioClip[] elementSounds;
    private AudioClip currentElementSound = null;
    private float maxAudioLevel = 1.0f;
    private bool setNewSound = true;

    private float heightAboveCamera = 0.0f;
    private Vector3 position = Vector3.zero;

    private bool hasSound = false;

    #region properties

    /// <summary>
    /// Gets or sets the sky.
    /// </summary>
    /// <value>
    /// The sky.
    /// </value>
    public TOD_Sky Sky
    {
        get { return sky; }
        set { sky = value; }
    }

    /// <summary>
    /// Gets or sets the weather.
    /// </summary>
    /// <value>
    /// The weather.
    /// </value>
    public TOD_Weather Weather
    {
        get { return weather; }
        set { weather = value; }
    }

    /// <summary>
    /// Gets or sets the weather system.
    /// </summary>
    /// <value>
    /// The weather system.
    /// </value>
    public WeatherSystem WeatherSystem
    {
        get { return weatherSystem; }
        set { weatherSystem = value; }
    }

    /// <summary>
    /// Gets or sets the wind animation.
    /// </summary>
    /// <value>
    /// The wind animation.
    /// </value>
    public TOD_Animation WindAnimation
    {
        get { return windAnimation; }
        set { windAnimation = value; }
    }

    /// <summary>
    /// Gets or sets the camera object.
    /// </summary>
    /// <value>
    /// The camera object.
    /// </value>
    public GameObject CameraObject
    {
        get { return cameraObject; }
        set { cameraObject = value; }
    }

    /// <summary>
    /// Gets or sets the atmosphere.
    /// </summary>
    /// <value>
    /// The atmosphere.
    /// </value>
    public GameObject Atmosphere
    {
        get { return atmosphere; }
        set { atmosphere = value; }
    }

    /// <summary>
    /// Gets or sets the sunlight.
    /// </summary>
    /// <value>
    /// The sunlight.
    /// </value>
    public Light SunLight
    {
        get { return sunLight; }
        set { sunLight = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [enable element].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable element]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableElement
    {
        get { return enableElement; }
        set { enableElement = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [activate element].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [activate element]; otherwise, <c>false</c>.
    /// </value>
    public bool ActivateElement
    {
        get { return activateElement; }
        set { activateElement = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [do fade in].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [do fade in]; otherwise, <c>false</c>.
    /// </value>
    public bool DoFadeIn
    {
        get { return doFadeIn; }
        set { doFadeIn = value; }
    }

    /// <summary>
    /// Gets or sets the fade time.
    /// </summary>
    /// <value>
    /// The fade time.
    /// </value>
    public float FadeTime
    {
        get { return fadeTime; }
        set { fadeTime = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is faded in.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is faded in; otherwise, <c>false</c>.
    /// </value>
    public bool IsFadedIn
    {
        get { return isFadedIn; }
        set { isFadedIn = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is initialized.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is initialized; otherwise, <c>false</c>.
    /// </value>
    public bool IsInitialized
    {
        get { return isInitialized; }
        set { isInitialized = value; }
    }

    /// <summary>
    /// Gets or sets the time scale divisor.
    /// </summary>
    /// <value>
    /// The time scale divisor.
    /// </value>
    public float TimeScaleDivisor
    {
        get { return timeScaleDivisor; }
        set { timeScaleDivisor = value; }
    }

    /// <summary>
    /// Gets or sets the height above camera.
    /// </summary>
    /// <value>
    /// The height above camera.
    /// </value>
    public float HeightAboveCamera
    {
        get { return heightAboveCamera; }
        set { heightAboveCamera = value; }
    }

    /// <summary>
    /// Gets or sets the element sounds.
    /// </summary>
    /// <value>
    /// The element sounds.
    /// </value>
    public AudioClip[] ElementSounds
    {
        get { return elementSounds; }
        set { elementSounds = value; }
    }

    /// <summary>
    /// Gets or sets the current element sound.
    /// </summary>
    /// <value>
    /// The current element sound.
    /// </value>
    public AudioClip CurrentElementSound
    {
        get { return currentElementSound; }
        set { currentElementSound = value; }
    }

    /// <summary>
    /// Gets or sets the maximum audio level.
    /// </summary>
    /// <value>
    /// The maximum audio level.
    /// </value>
    public float MaxAudioLevel
    {
        get { return maxAudioLevel; }
        set { maxAudioLevel = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [set new sound].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [set new sound]; otherwise, <c>false</c>.
    /// </value>
    public bool SetNewSound
    {
        get { return setNewSound; }
        set { setNewSound = true; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [has sound].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [has sound]; otherwise, <c>false</c>.
    /// </value>
    public bool HasSound
    {
        get { return hasSound; }
        set { hasSound = true; }
    }

    #endregion

    public virtual void Initialize()
    {
        weatherSystem = (WeatherSystem)FindObjectOfType(typeof(WeatherSystem));
        sky = (TOD_Sky)FindObjectOfType(typeof(TOD_Sky));
        sunLight = sky.GetComponentInChildren<Light>();
        weather = sky.GetComponent<TOD_Weather>();
        windAnimation = sky.GetComponent<TOD_Animation>();
        cameraObject = Camera.main.gameObject;

        if (cameraObject)
        {
            position = cameraObject.transform.position;
            position.y += heightAboveCamera;
            transform.position = position;
        }

        if (GetComponent<AudioSource>() != null)
            hasSound = true;

        isBaseInitialized = true;
    }

    protected virtual void Awake()
    {
    }
    
    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
    }

    protected virtual void LateUpdate()
    {
        if (cameraObject)
        {
            position    = CameraObject.transform.position;
            position.y += HeightAboveCamera;
            transform.position = position;
        }
    }

    public virtual void Transition(bool fadeIn)
    {
        if (!weatherSystem.isTODPresent || !isInitialized)
            return;

        if (!enableElement)
            return;

        activateElement = true;
        doFadeIn = fadeIn;
        if (doFadeIn && elementSounds.Length > 0)
        {
            currentElementSound = elementSounds[Random.Range(0, elementSounds.Length - 1)];
            setNewSound = true;
        }
    }

    public virtual void Reset()
    {
        if (hasSound)
        {
            GetComponent<AudioSource>().volume = 0.0f;
            GetComponent<AudioSource>().Stop();
        }

        activateElement = false;
        doFadeIn = false;
        fadeTime = 0.0f;
        isFadedIn = false;
    }
}
