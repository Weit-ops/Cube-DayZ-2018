#if UNITY_4_0||UNITY_4_1||UNITY_4_2||UNITY_4_3||UNITY_4_4||UNITY_4_5||UNITY_4_6||UNITY_4_7||UNITY_4_8||UNITY_4_9
#define UNITY_4
#endif

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if !UNITY_4
using UnityStandardAssets.ImageEffects;
#endif

/// <summary>
/// Class for Weather System
/// </summary>
public class WeatherSystem : MonoBehaviour
{
    // Predictions
    public TOD_WeatherType[] weatherTypes;
    public TOD_CloudType[] cloudTypes;
    public TOD_CloudType currentCloudType = TOD_CloudType.None;
    public float[] weatherPercentages;
    private TOD_WeatherType[] weatherTypesClone;
    private float[] weatherPercentagesClone;
    
    // TOD
    private GameObject skyDome = null;
    private TOD_Animation windAnimation;
    private float checkPeriodInSeconds = 0.0f;
    private float checkTimer = 0.0f;
    public int checksPerDay = 4;
    private bool isPredictionDone = false;
    private bool isFirstWeatherCheck = true;

    // Thunder and lightning
    public bool enableThunderAndLightning = true;
    public AudioClip[] thunderSounds;
    public Color lightningColor = Color.white;
    public bool doLightningGlow = true;
    public float lightningGlowWidth = 50.0f;
    public Color lightningGlowColor = new Color(1.0f, 1.0f, 1.0f, 0.059f);
    public float lightningOriginGlowWidth = 100.00f;
    public Color lightningOriginGlowColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    public float lightningFlashIntensity = 1.0f;
    public float maxTLAudioLevel = 1.0f;
    public float minTimeBetweenStrikes = 10.0f;
    public float maxTimeBetweenStrikes = 20.0f;
    public float lifeTime = 0.5f;
    public float lightningWidth = 1.0f;
    public float lightningInnerConeAngle = 15.0f;
    public float lightningOuterConeAngle = 90.0f;
    public float maxLightningHitDistance = 5000.0f;
    public int numVertices = 20;
    public float rangeMin = -20.0f;
    public float rangeMax = 20.0f;
    public float maxDeviation = 10.0f;
    public float nonStrikeLength = 5.0f;
    public int minNumBranchVerts = 4;
    public int maxNumBranchVerts = 8;
    public float minBranchLength = 10.0f;
    public float maxBranchLength = 20.0f;
    public float maxBranchDeviation = 5.0f;
    public float branchStartPercentage = 0.3f;
    public int branchSpacing = 2;		

    // Rain
    public bool enableRain = true;
    public bool enableFogWithRain = false;
    public bool rainIsEffectedByWind = true;
    public float rainHeightAbove = 10.0f;
    public float minRainParticleSize = 0.2f;
    public float maxRainParticleSize = 0.4f;
    public int minNumRainParticles = 500;
    public int maxNumRainParticles = 1000;
	public float rainDayAlpha = 50f;
	public float rainNightAlpha = 10f;
    public Color rainColor = new Color(0.175f, 0.175f, 0.175f, 0.2f);
    public Color rainColorMin = new Color(0.175f, 0.175f, 0.175f, 0.2f);
    public float splashEnergy = 10.0f;
    public Transform splashObject;
    public AudioClip[] rainSounds;
    public float maxRainAudioLevel = 1.0f;

    // Fog
    public bool enableFog = true;
    public bool useGlobalFog = false;

#if UNITY_4
    public GlobalFog_U4.FogMode globalFogMode = GlobalFog_U4.FogMode.Distance;
    public float globalFogHeightScale = 100.0f;
#else
    public bool distanceFog = true;
    public bool useRadialDistance = false;
    public bool heightFog = true;
    public float heightDensity = 2.0f;
    public float fogStartOffset = 1.0f;
#endif

    public float globalFogHeight = 100.0f;
    public Color fogColor = Color.grey;
    public FogMode fogModeToUse = FogMode.Linear;
    public float fogStartDistance = 0.0f;
    public float fogEndDistance = 300.0f;
    public float fogDensityMin = 0.0f;
    public float fogDensityMax = 0.05f;
    public float fogFadeScale = 1.0f;
    public AudioClip[] fogSounds;
    public float maxFogAudioLevel = 1.0f;

    // Dust
    public bool enableDust = true;
    public bool dustIsAffectedByWind = true;
    public float dustHeightAbove = 0.0f;
    public float minDustParticleSize = 4.0f;
    public float maxDustParticleSize = 5.0f;
    public int minNumDustParticles = 100;
    public int maxNumDustParticles = 200;
    public Color dustColor = Color.gray;
    public AudioClip[] dustSounds;
    public float maxDustAudioLevel = 1.0f;

    // Component references
    private ThunderAndLightningElement thunderAndLightning = null;
    private MyRainElement rain = null;
    private FogElement fog = null;
    private DustElement dust = null;
    private TOD_Weather weather = null;

    private GameObject thunderAndLightningElement = null;
    private GameObject rainElement = null;
    private GameObject fogElement = null;
    private GameObject dustElement = null;

    // Sanity check
    public bool isTODPresent = false;
    private bool isInitialized = false;

    public TOD_WeatherType currentWeather = TOD_WeatherType.Clear;
    public bool isDirty = false;

	private Action _afterInitialize;
	private bool _isStarted;

    /// <summary>
    /// Used to initialize Weather System
    /// </summary>
    public void Initialize()
    {
        if (!(TOD_Sky)FindObjectOfType(typeof(TOD_Sky)))
        {
            isTODPresent = false;
            return;
        }

        isTODPresent = true;

        int enumCount = System.Enum.GetValues(typeof(TOD_WeatherType)).Length;

        if (weatherTypes == null || weatherTypes.Length < enumCount)
        {
            weatherTypes = new TOD_WeatherType[enumCount];
            weatherPercentages = new float[enumCount];

            for (int i = 0; i < enumCount; i++)
            {
                TOD_WeatherType wt = GetEnum<TOD_WeatherType>(i);
                weatherTypes[i] = wt;
                weatherPercentages[i] = 20.0f;
            }
        }

        if (cloudTypes == null || cloudTypes.Length < enumCount)
        {
            cloudTypes = new TOD_CloudType[enumCount];

            for (int i = 0; i < enumCount; i++)
            {
                TOD_CloudType ct = GetEnum<TOD_CloudType>(i);
                cloudTypes[i] = ct;
            }
        }

        isInitialized = true;
    }

    void Start()
    {
        if (!isTODPresent)
            return;

        if (!isInitialized)
            Initialize();

        GetWeatherComponents();

        UpdateWeatherElements();

        InitializeWeatherElements();

        if (checksPerDay < 1)
            checksPerDay = 1;
        checkPeriodInSeconds = (skyDome.GetComponent<TOD_Time>().DayLengthInMinutes * 60) / checksPerDay;

        // Copy weather weather types and percentages
        weatherTypesClone = (TOD_WeatherType[])weatherTypes.Clone();
        weatherPercentagesClone = (float[])weatherPercentages.Clone();

        // Sort arrays in descending order
        Array.Sort(weatherPercentagesClone, weatherTypesClone);
        Array.Reverse(weatherTypesClone);
        Array.Reverse(weatherPercentagesClone);

		if (_afterInitialize != null)
		{
			_afterInitialize();
			_afterInitialize = null;
		}

		_isStarted = true;
    }

    void Update()
    {
        if (!isTODPresent || !isInitialized)
            return;

        if (isDirty)
        {
            UpdateWeatherElements();
            isDirty = false;
			isFirstWeatherCheck = true;
			checkTimer = 0.0f;
        }

        checkTimer -= Time.deltaTime;

        if (checkTimer > 0.0f)
            return;

        if (!isPredictionDone)
        {
            currentWeather = ChooseWeather(weatherPercentagesClone);
            currentCloudType = cloudTypes[(int)currentWeather];
            isPredictionDone = true;
        }
        
        if(currentWeather != weather.Weather || isFirstWeatherCheck)
        {
            weather.Weather = currentWeather;

            ChangeWeather(weather.Weather);
        }

        checkTimer = checkPeriodInSeconds;
        isPredictionDone = false;
		isFirstWeatherCheck = false;
    }

	public void SyncWeather(byte weatherType)
	{
		if (!_isStarted)
		{
			_afterInitialize = delegate
			{
				currentWeather = (TOD_WeatherType)weatherType;
				currentCloudType = cloudTypes[(uint)currentWeather];
				if (currentWeather != weather.Weather)
				{
					weather.Weather = currentWeather;
					ChangeWeather(weather.Weather);
				}
			};
		}
		else
		{
			currentWeather = (TOD_WeatherType)weatherType;
			currentCloudType = cloudTypes[(uint)currentWeather];
			if (currentWeather != weather.Weather)
			{
				weather.Weather = currentWeather;
				ChangeWeather(weather.Weather);
			}
		}
	}
	
	/// <summary>
    /// Changes the weather.
    /// </summary>
    /// <param name="currentWeather">The current weather.</param>
    public void ChangeWeather(TOD_WeatherType currentWeather)
    {
        weather.Clouds = cloudTypes[(int)currentWeather];

        if (isFirstWeatherCheck)
        {
            rain.Reset();
            thunderAndLightning.Reset();
            fog.Reset();
            dust.Reset();
        }

        switch (currentWeather)
        {
            case TOD_WeatherType.Clear:
                if (fog)
                    fog.Transition(false);
                if (rain)
                    rain.Transition(false);
                if (thunderAndLightning)
                    thunderAndLightning.EnableElement = false;
                if (dust)
                    dust.Transition(false);
                break;

            case TOD_WeatherType.Storm:
                rain.Transition(true);
                if (enableThunderAndLightning)
                    thunderAndLightning.EnableElement = true;
                else
                    thunderAndLightning.EnableElement = false;
                dust.Transition(false);
                if (enableFogWithRain)
                {
                    fog.Transition(true);
                }
                else
                    fog.Transition(false);
                break;

            case TOD_WeatherType.Dust:
                fog.Transition(false);
                rain.Transition(false);
                thunderAndLightning.EnableElement = false;
                dust.Transition(true);
                break;

            case TOD_WeatherType.Fog:
                rain.Transition(false);
                thunderAndLightning.EnableElement = false;
                dust.Transition(false);
                fog.Transition(true);
                break;

            default:
                fog.Transition(false);
                rain.Transition(false);
                thunderAndLightning.EnableElement = false;
                dust.Transition(false);
                break;
        }
    }

    /// <summary>
    /// Gets the weather components.
    /// </summary>
    public void GetWeatherComponents()
    {
		TOD_Sky skyObject = (TOD_Sky)FindObjectOfType(typeof(TOD_Sky));
        skyDome = skyObject.gameObject;
        weather = skyDome.GetComponent<TOD_Weather>();
        windAnimation = skyDome.GetComponent<TOD_Animation>();
        if (GetComponentInChildren<ThunderAndLightningElement>())
        {
            thunderAndLightningElement = GetComponentInChildren<ThunderAndLightningElement>().gameObject;
            thunderAndLightning = thunderAndLightningElement.GetComponent<ThunderAndLightningElement>();
        }
        if (GetComponentInChildren<MyRainElement>())
        {
            rainElement = GetComponentInChildren<MyRainElement>().gameObject;
            rain = rainElement.GetComponent<MyRainElement>();
        }
        if (GetComponentInChildren<FogElement>())
        {
            fogElement = GetComponentInChildren<FogElement>().gameObject;
            fog = fogElement.GetComponent<FogElement>();
        }
        if (GetComponentInChildren<DustElement>())
        {
            dustElement = GetComponentInChildren<DustElement>().gameObject;
            dust = dustElement.GetComponent<DustElement>();
        }
    }

    /// <summary>
    /// Updates the weather elements.
    /// </summary>
    public void UpdateWeatherElements()
    {
        if (thunderAndLightning)
        {
            thunderAndLightning.LightningColor = lightningColor;
            thunderAndLightning.DoLightningGlow = doLightningGlow;
            thunderAndLightning.LightningGlowColor = lightningGlowColor;
            thunderAndLightning.LightningGlowWidth = lightningGlowWidth;
            thunderAndLightning.LightningOriginGlowColor = lightningOriginGlowColor;
            thunderAndLightning.LightningOriginGlowWidth = lightningOriginGlowWidth;
            thunderAndLightning.LightningFlashIntensity = lightningFlashIntensity;
            thunderAndLightning.MinTimeBetweenStrikes = minTimeBetweenStrikes;
            thunderAndLightning.MaxTimeBetweenStrikes = maxTimeBetweenStrikes;
            thunderAndLightning.LifeTime = lifeTime;
            thunderAndLightning.LightningWidth = lightningWidth;
            thunderAndLightning.InnerConeAngle = lightningInnerConeAngle;
            thunderAndLightning.OuterConeAngle = lightningOuterConeAngle;
            thunderAndLightning.MaxLightningHitDistance = maxLightningHitDistance;
            thunderAndLightning.NumVertices = numVertices;
            thunderAndLightning.RangeMin = rangeMin;
            thunderAndLightning.RangeMax = rangeMax;
            thunderAndLightning.MaxDeviation = maxDeviation;
            thunderAndLightning.NonStrikeLength = nonStrikeLength;
            thunderAndLightning.MinNumBranchVerts = minNumBranchVerts;
            thunderAndLightning.MaxNumBranchVerts = maxNumBranchVerts;
            thunderAndLightning.MinBranchLength = minBranchLength;
            thunderAndLightning.MaxBranchLength = maxBranchLength;
            thunderAndLightning.MaxBranchDeviation = maxBranchDeviation;
            thunderAndLightning.BranchStartPercentage = branchStartPercentage;
            thunderAndLightning.BranchSpacing = branchSpacing;
            thunderAndLightning.MaxAudioLevel = maxTLAudioLevel;
            thunderAndLightning.Atmosphere = skyDome.GetComponent<TOD_Components>().Atmosphere;
            thunderAndLightning.ElementSounds = thunderSounds;
        }

        if (rain)
        {

            rain.TimeScaleDivisor = weather.FadeTime;
            rain.HeightAboveCamera = rainHeightAbove;
			rain.DayAlpha = rainDayAlpha;
			rain.NightAlpha = rainNightAlpha;
            rain.ElementSounds = rainSounds;
            rain.MaxAudioLevel = maxRainAudioLevel;
            rain.EnableElement = enableRain;
        }

        if (fog)
        {
            fog.EnableElement = enableFog;
            fog.UseGlobalFog = useGlobalFog;

            fog.TimeScaleDivisor = weather.FadeTime;
            fog.FogDensityMin = fogDensityMin;
            fog.FogDensityMax = fogDensityMax;
            fog.FogStartDistance = fogStartDistance;
            fog.FogEndDistance = fogEndDistance;
            fog.FogModeUsed = fogModeToUse;

#if UNITY_4
            fog.GlobalFogMode = globalFogMode;
            fog.GlobalFogHeightScale = globalFogHeightScale;
#else
            fog.DistanceFog = distanceFog;
            fog.UseRadialDistance = useRadialDistance;
            fog.HeightFog = heightFog;
            fog.HeightDensity = heightDensity;
            fog.FogStartOffset = fogStartOffset;
#endif
            fog.GlobalFogHeight = globalFogHeight;
            fog.FogColor = fogColor;

            fog.FogFadeScale = fogFadeScale;

            fog.ElementSounds = fogSounds;
            fog.MaxAudioLevel = maxFogAudioLevel;
        }

        if (dust)
        {
            dust.TimeScaleDivisor = weather.FadeTime;
            dust.HeightAboveCamera = dustHeightAbove;

            dust.DustColor = dustColor;
            dust.ElementSounds = dustSounds;
            dust.MaxAudioLevel = maxDustAudioLevel;
            dust.EnableElement = enableDust;
        }
    }

    /// <summary>
    /// Initializes the weather elements.
    /// </summary>
    public void InitializeWeatherElements()
    {
        if(thunderAndLightning)
            thunderAndLightning.Initialize();
        if(rain)
            rain.Initialize();
        if(fog)
            fog.Initialize();
        if(dust)
            dust.Initialize();
    }

    public void SetThunderAndLightningDefaults()
    {
        enableThunderAndLightning = true;
        lightningColor = Color.white;
        doLightningGlow = true;
        lightningGlowWidth = 50.0f;
        lightningGlowColor = new Color(1.0f, 1.0f, 1.0f, 0.059f);
        lightningOriginGlowWidth = 100.00f;
        lightningOriginGlowColor = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        lightningFlashIntensity = 1.0f;
        maxTLAudioLevel = 1.0f;
        minTimeBetweenStrikes = 10.0f;
        maxTimeBetweenStrikes = 20.0f;
        lifeTime = 0.5f;
        lightningWidth = 1.0f;
        lightningInnerConeAngle = 15.0f;
        lightningOuterConeAngle = 90.0f;
        maxLightningHitDistance = 5000.0f;
        numVertices = 20;
        rangeMin = -20.0f;
        rangeMax = 20.0f;
        maxDeviation = 10.0f;
        nonStrikeLength = 5.0f;
        minNumBranchVerts = 4;
        maxNumBranchVerts = 8;
        minBranchLength = 10.0f;
        maxBranchLength = 20.0f;
        maxBranchDeviation = 5.0f;
        branchStartPercentage = 0.3f;
        branchSpacing = 2;		
    }

    public void SetRainDefaults()
    {
        enableRain = true;
        enableFogWithRain = false;
        rainIsEffectedByWind = true;
        rainHeightAbove = 10.0f;
        minRainParticleSize = 0.2f;
        maxRainParticleSize = 0.4f;
        minNumRainParticles = 500;
        maxNumRainParticles = 1000;
        rainColor = new Color(0.175f, 0.175f, 0.175f, 0.2f);
        rainColorMin = new Color(0.175f, 0.175f, 0.175f, 0.2f);
        splashEnergy = 10.0f;
        maxRainAudioLevel = 1.0f;
    }

    public void SetFogDefaults()
    {
        enableFog = true;
        useGlobalFog = false;

    #if UNITY_4
        globalFogMode = GlobalFog_U4.FogMode.Distance;
        globalFogHeightScale = 100.0f;
    #else
        distanceFog = true;
        useRadialDistance = false;
        heightFog = true;
        heightDensity = 2.0f;
        fogStartOffset = 1.0f;
    #endif

        globalFogHeight = 100.0f;
        fogColor = Color.grey;
        fogModeToUse = FogMode.Linear;
        fogStartDistance = 0.0f;
        fogEndDistance = 300.0f;
        fogDensityMin = 0.0f;
        fogDensityMax = 0.05f;
        fogFadeScale = 4.0f;
        maxFogAudioLevel = 1.0f;
    }

    public void SetDustDefaults()
    {
        enableDust = true;
        dustIsAffectedByWind = true;
        dustHeightAbove = 0.0f;
        minDustParticleSize = 4.0f;
        maxDustParticleSize = 5.0f;
        minNumDustParticles = 100;
        maxNumDustParticles = 200;
        dustColor = Color.gray;
        maxDustAudioLevel = 1.0f;
    }

    /// <summary>
    /// Get random enumerator
    /// </summary>
    static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(0,A.Length));
        return V;
    }

    /// <summary>
    /// Get enumerator at index
    /// </summary>
    static T GetEnum<T>(int index)
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(index);
        return V;
    }

    /// <summary>
    /// Checks the total.
    /// </summary>
    /// <param name="numbers">The numbers.</param>
    /// <param name="desiredTotal">The desired total.</param>
    /// <param name="keepZeros">if set to <c>true</c> [keep zeros].</param>
    public void CheckTotal(ref float[] numbers, float desiredTotal, bool keepZeros)
    {
        float total = 0.0f;
        float totalDifference = 0.0f;
        for (int i = 0; i < numbers.Length; i++)
            total += numbers[i];

        if (total != desiredTotal)
            totalDifference = desiredTotal - total;

        if (totalDifference != 0.0f)
        {
            float difference = totalDifference / numbers.Length;
            for (int j = 0; j < numbers.Length; j++)
            {
                if (keepZeros && numbers[j] == 0.0f)
                    continue;
                numbers[j] += difference;
            }
        }
    }

    /// <summary>
    /// Chooses a random weather pattern based on percentages
    /// </summary>
    TOD_WeatherType ChooseWeather(float[] percentages)
    {
        float total = 0.0f;

        for (int i = 0; i < percentages.Length; i++)
        {
            total += percentages[i];
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int j = 0; j < percentages.Length; j++)
        {
            if (randomPoint < percentages[j])
                return weatherTypesClone[j];
            else
                randomPoint -= percentages[j];
        }

        return weatherTypesClone[percentages.Length - 1];
    }

    /// <summary>
    /// Gets the wind effect.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWindEffect()
    {
        Vector2 windDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * windAnimation.WindDegrees),
                                 Mathf.Sin(Mathf.Deg2Rad * windAnimation.WindDegrees));
        Vector3 wind = windAnimation.WindSpeed * new Vector3(windDirection.y, 0.0f, -windDirection.x);
        return wind;
    }
}
