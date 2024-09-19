#if UNITY_4_0||UNITY_4_1||UNITY_4_2||UNITY_4_3||UNITY_4_4||UNITY_4_5||UNITY_4_6||UNITY_4_7||UNITY_4_8||UNITY_4_9
#define UNITY_4
#endif

using UnityEngine;
using System.Collections;

#if !UNITY_4
using UnityStandardAssets.ImageEffects;
#endif

/// <summary>
/// Class for Fog Element
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class FogElement : WeatherElement
{
    
#if UNITY_4
    private GlobalFog_U4 globalFogScript = null;
    private GlobalFog_U4.FogMode globalFogMode = GlobalFog_U4.FogMode.Distance;
    private float globalFogHeightScale = 100.0f;
#else
    private GlobalFog_U5 globalFogScript = null;
    private bool distanceFog = true;
    private bool useRadialDistance = false;
    private bool heightFog = true;
    private float heightDensity = 2.0f;
    private float fogStartOffset = 1.0f;
#endif

    private float globalFogHeight = 100.0f;

    private Color fogColor = Color.grey;
    private bool useGlobalFog = false;
    private float fogFadeScale = 1.0f;

    // Linear Fog
    private float fogStartDistance = 0.0f;
    private float fogEndDistance = 300.0f;

    // Fog density
    private float fogDensityMin = 0.0f;
    private float fogDensityMax = 0.05f;

    private FogMode fogModeUsed = FogMode.ExponentialSquared;
    private bool isPro = false;

    #region properties

#if UNITY_4

    /// <summary>
    /// Gets or sets the global fog script.
    /// </summary>
    /// <value>
    /// The global fog script.
    /// </value>
    public GlobalFog_U4 GlobalFogScript
    {
        get { return globalFogScript; }
        set { globalFogScript = value; }
    }

    /// <summary>
    /// Gets or sets the global fog mode.
    /// </summary>
    /// <value>
    /// The global fog mode.
    /// </value>
    public GlobalFog_U4.FogMode GlobalFogMode
    {
        get { return globalFogMode; }
        set { globalFogMode = value; }
    }

    /// <summary>
    /// Gets or sets the global fog height scale.
    /// </summary>
    /// <value>
    /// The global fog height scale.
    /// </value>
    public float GlobalFogHeightScale
    {
        get { return globalFogHeightScale; }
        set { globalFogHeightScale = value; }
    }

#else

    /// <summary>
    /// Gets or sets the global fog script.
    /// </summary>
    /// <value>
    /// The global fog script.
    /// </value>
    public GlobalFog_U5 GlobalFogScript
    {
        get { return globalFogScript; }
        set { globalFogScript = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use distance fog.
    /// </summary>
    /// <value>
    ///   <c>true</c> if use distance fog; otherwise, <c>false</c>.
    /// </value>
    public bool DistanceFog
    {
        get { return distanceFog; }
        set { distanceFog = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use radial distance fog.
    /// </summary>
    /// <value>
    ///   <c>true</c> if use radial distance fog; otherwise, <c>false</c>.
    /// </value>
    public bool UseRadialDistance
    {
        get { return useRadialDistance; }
        set { useRadialDistance = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use height fog.
    /// </summary>
    /// <value>
    ///   <c>true</c> if use height fog; otherwise, <c>false</c>.
    /// </value>
    public bool HeightFog
    {
        get { return heightFog; }
        set { heightFog = value; }
    }

    /// <summary>
    /// Gets or sets the global fog height density.
    /// </summary>
    /// <value>
    /// The global fog height density.
    /// </value>
    public float HeightDensity
    {
        get { return heightDensity; }
        set { heightDensity = value; }
    }

    /// <summary>
    /// Gets or sets the start offset of the fog.
    /// </summary>
    /// <value>
    /// The fog start offset.
    /// </value>
    public float FogStartOffset
    {
        get { return fogStartOffset; }
        set { fogStartOffset = value; }
    }

    
#endif

    /// <summary>
    /// Gets or sets the height of the global fog.
    /// </summary>
    /// <value>
    /// The height of the global fog.
    /// </value>
    public float GlobalFogHeight
    {
        get { return globalFogHeight; }
        set { globalFogHeight = value; }
    }

    /// <summary>
    /// Gets or sets the color of the global fog.
    /// </summary>
    /// <value>
    /// The color of the global fog.
    /// </value>
    public Color FogColor
    {
        get { return fogColor; }
        set { fogColor = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use global fog.
    /// </summary>
    /// <value>
    ///   <c>true</c> if use global fog; otherwise, <c>false</c>.
    /// </value>
    public bool UseGlobalFog
    {
        get { return useGlobalFog; }
        set { useGlobalFog = value; }
    }

    /// <summary>
    /// Gets or sets the fog fade scale.
    /// </summary>
    /// <value>
    /// The fog fade scale.
    /// </value>
    public float FogFadeScale
    {
        get { return fogFadeScale; }
        set { fogFadeScale = value; }
    }

    /// <summary>
    /// Gets or sets the fog start distance.
    /// </summary>
    /// <value>
    /// The fog start distance.
    /// </value>
    public float FogStartDistance
    {
        get { return fogStartDistance; }
        set { fogStartDistance = value; }
    }

    /// <summary>
    /// Gets or sets the fog end distance.
    /// </summary>
    /// <value>
    /// The fog end distance.
    /// </value>
    public float FogEndDistance
    {
        get { return fogEndDistance; }
        set { fogEndDistance = value; }
    }

    /// <summary>
    /// Gets or sets the fog density minimum.
    /// </summary>
    /// <value>
    /// The fog density minimum.
    /// </value>
    public float FogDensityMin
    {
        get { return fogDensityMin; }
        set { fogDensityMin = value; }
    }

    /// <summary>
    /// Gets or sets the fog density maximum.
    /// </summary>
    /// <value>
    /// The fog density maximum.
    /// </value>
    public float FogDensityMax
    {
        get { return fogDensityMax; }
        set { fogDensityMax = value; }
    }

    /// <summary>
    /// Gets or sets the fog mode used.
    /// </summary>
    /// <value>
    /// The fog mode used.
    /// </value>
    public FogMode FogModeUsed
    {
        get { return fogModeUsed; }
        set { fogModeUsed = value; }
    }

    #endregion

    /// <summary>
    /// Used to initialize Fog element
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

		if (EnableElement)
        {
            if (GetComponent<AudioSource>() != null && ElementSounds.Length > 0)
            {
                GetComponent<AudioSource>().loop = true;
                GetComponent<AudioSource>().volume = 0.0f;
                CurrentElementSound = ElementSounds[Random.Range(0, ElementSounds.Length - 1)];
            }
	
	        // Check if Pro version of Unity and Global Fog on main camera
	        // Add Global Fog image effect if needed
	        if (SystemInfo.supportsImageEffects)
	            isPro = true;
	
	        if (useGlobalFog && isPro)
	        {
                RenderSettings.fog = false;

#if UNITY_4
                globalFogScript = Camera.main.GetComponent<GlobalFog_U4>();
#else
                globalFogScript = Camera.main.GetComponent<GlobalFog_U5>();
#endif
                globalFogScript.enabled = EnableElement;

#if UNITY_4
                globalFogScript.fogMode = globalFogMode;
	            globalFogScript.globalDensity = 0.0f;
	            globalFogScript.heightScale = globalFogHeightScale;
                globalFogScript.globalFogColor = fogColor;
#else
                globalFogScript.distanceFog = distanceFog;
                globalFogScript.useRadialDistance = useRadialDistance;
                globalFogScript.heightFog = heightFog;
                globalFogScript.heightDensity = 0.0f;
#endif

                globalFogScript.startDistance = fogEndDistance;
                globalFogScript.height = globalFogHeight;
	            
	        }
	        else
	        {
	            RenderSettings.fog = EnableElement;
	            RenderSettings.fogDensity = 0.0f;
	            RenderSettings.fogStartDistance = fogEndDistance;
	            RenderSettings.fogEndDistance = fogEndDistance;
	            RenderSettings.fogMode = fogModeUsed;
	        }
	
            fogColor = RenderSettings.fogColor;
	
	        IsFadedIn = false;
	
	        if (fogFadeScale <= 0.0f)
	            fogFadeScale = 0.001f;
		}

        IsInitialized = true;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (!WeatherSystem.isTODPresent || !IsInitialized)
            return;

        if (!EnableElement)
            return;

        if (!ActivateElement)
            return;

        if (DoFadeIn && IsFadedIn)
            return;

        if (!DoFadeIn && !IsFadedIn)
            return;

        FadeTime += Time.deltaTime / TimeScaleDivisor / fogFadeScale;

        if (SetNewSound && HasSound)
        {
            GetComponent<AudioSource>().clip = CurrentElementSound;

            if (!GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().Play();

            SetNewSound = false;
        }

        if (FadeTime <= 1.0f)
        {
            if (DoFadeIn)
            {
                FadeFogDistance(DoFadeIn, FadeTime);
                if (ElementSounds.Length > 0 && HasSound)
                    GetComponent<AudioSource>().volume = Mathf.Lerp(0.0f, MaxAudioLevel, FadeTime);
            }
            else
            {
                FadeFogDensity(DoFadeIn, FadeTime);
                if (ElementSounds.Length > 0 && HasSound)
                    GetComponent<AudioSource>().volume = Mathf.Lerp(MaxAudioLevel, 0.0f, FadeTime);
            }
        }
        else
        {
            if (DoFadeIn)
                FadeFogDensity(DoFadeIn, FadeTime - 1.0f);
            else
                FadeFogDistance(DoFadeIn, FadeTime);

            if (FadeTime > 2.0f)
            {
                IsFadedIn = DoFadeIn;
                ActivateElement = false;
                FadeTime = 0.0f;

                if (HasSound)
                {
                    if (!DoFadeIn && GetComponent<AudioSource>().isPlaying)
                        GetComponent<AudioSource>().Stop();
                }
            }
        }
    }

    /// <summary>
    /// Fades the fog distance.
    /// </summary>
    /// <param name="fadingIn">if set to <c>true</c> [fading in].</param>
    /// <param name="fadeAmount">The fade amount.</param>
    private void FadeFogDistance(bool fadingIn, float fadeAmount)
    {
        if (fadingIn)
        {
            if (useGlobalFog && globalFogScript != null)
            {

#if UNITY_4
                globalFogScript.globalFogColor = RenderSettings.fogColor;
                globalFogScript.startDistance = Mathf.Lerp(fogEndDistance, fogStartDistance, fadeAmount);
#else
                globalFogScript.startDistance = Mathf.Lerp(fogEndDistance - fogStartOffset, fogStartDistance, fadeAmount);
#endif

            }
            else
            {

#if UNITY_4
                RenderSettings.fogStartDistance = Mathf.Lerp(fogEndDistance, fogStartDistance, fadeAmount);
#else
                RenderSettings.fogStartDistance = Mathf.Lerp(fogEndDistance - fogStartOffset, fogStartDistance, fadeAmount);
#endif
            }
        }
        else if (!fadingIn)
        {
            if (useGlobalFog && globalFogScript != null)
            {

#if UNITY_4
                globalFogScript.startDistance = Mathf.Lerp(fogStartDistance, fogEndDistance, fadeAmount);
#else
                globalFogScript.startDistance = Mathf.Lerp(fogStartDistance, fogEndDistance - fogStartOffset, fadeAmount);
#endif

            }
            else
            {

#if UNITY_4
                RenderSettings.fogStartDistance = Mathf.Lerp(fogStartDistance, fogEndDistance, fadeAmount);
#else
                RenderSettings.fogStartDistance = Mathf.Lerp(fogStartDistance, fogEndDistance - fogStartOffset, fadeAmount);
#endif

            }
        }
    }

    /// <summary>
    /// Fades the fog density.
    /// </summary>
    /// <param name="fadingIn">if set to <c>true</c> [fading in].</param>
    /// <param name="fadeAmount">The fade amount.</param>
    private void FadeFogDensity(bool fadingIn, float fadeAmount)
    {
        if (fadingIn)
        {
            if (useGlobalFog && globalFogScript != null)
            {

#if UNITY_4
                globalFogScript.globalDensity = Mathf.Lerp(fogDensityMin, fogDensityMax, fadeAmount);
                globalFogScript.globalFogColor = RenderSettings.fogColor;
#else
                globalFogScript.heightDensity = Mathf.Lerp(0.0f, heightDensity, fadeAmount);
                RenderSettings.fogDensity = Mathf.Lerp(fogDensityMin, fogDensityMax, fadeAmount);
#endif

            }
            else
            {
                RenderSettings.fogDensity = Mathf.Lerp(fogDensityMin, fogDensityMax, fadeAmount);
            }
        }
        else if (!fadingIn)
        {
            if (useGlobalFog && globalFogScript != null)
            {

#if UNITY_4
                globalFogScript.globalDensity = Mathf.Lerp(fogDensityMax, fogDensityMin, fadeAmount);
#else
                globalFogScript.heightDensity = Mathf.Lerp(heightDensity, 0.0f, fadeAmount);
                RenderSettings.fogDensity = Mathf.Lerp(fogDensityMax, fogDensityMin, fadeAmount);
#endif

            }
            else
            {
                RenderSettings.fogDensity = Mathf.Lerp(fogDensityMax, fogDensityMin, fadeAmount);
            }
        }
    }

    /// <summary>
    /// Used to transitionally fade Fog element
    /// </summary>
    public override void Transition(bool fadeIn)
    {
        if (!WeatherSystem.isTODPresent || !IsInitialized)
            return;

        ActivateElement = true;
        DoFadeIn = fadeIn;
    }

    public override void Reset()
    {
        base.Reset();

        if (globalFogScript != null)
        {
            globalFogScript.startDistance = fogEndDistance;

#if UNITY_4
            globalFogScript.globalDensity = fogDensityMin;
#else
            globalFogScript.heightDensity = heightDensity;
#endif

        }

        RenderSettings.fogStartDistance = fogEndDistance;

        if (RenderSettings.fogMode == FogMode.Linear)
            RenderSettings.fogStartDistance -= 0.001f;

        RenderSettings.fogDensity = fogDensityMin;
    }
}
