#if UNITY_4_0||UNITY_4_1||UNITY_4_2||UNITY_4_3||UNITY_4_4||UNITY_4_5||UNITY_4_6||UNITY_4_7||UNITY_4_8||UNITY_4_9
#define UNITY_4
#endif

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if !UNITY_4
using UnityStandardAssets.ImageEffects;
#endif

/// <summary>
/// Class for the Weather System Editor GUI
/// </summary>
[CustomEditor(typeof(WeatherSystem))]
public class WeatherSystemEditor : Editor
{
    // Instance of Weather class
    protected WeatherSystem self;

    // Instance reference for editor
    public virtual void OnEnable()
    {
        self = (WeatherSystem)target;
        self.Initialize();
    }

    public override void OnInspectorGUI()
    {
        if (!self.isTODPresent)
        {
            EditorGUILayout.HelpBox("Time of Day asset not present in hierarchy!", MessageType.Error);
            return;
        }

        GUI.color = new Color(0.75f, 0.75f, 0.75f);

        // General Settings
        EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        for (int i = 0; i < self.weatherTypes.Length; i++)
        {
            self.weatherPercentages[i] = EditorGUILayout.Slider(self.weatherTypes[i] + " %:", self.weatherPercentages[i], 0.0f, 100.0f);
            if (self.weatherPercentages[i].ToString().Contains("E"))  // Any value below 0, make 0
                self.weatherPercentages[i] = 0.0f;
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset All % Zero"))
        {
            for (int i = 0; i < self.weatherTypes.Length; i++)
            {
                self.weatherPercentages[i] = 0.0f;
            }
            GUI.changed = true;
        }

        if (GUILayout.Button("Reset All % Even"))
        {
            for (int i = 0; i < self.weatherTypes.Length; i++)
            {
                self.weatherPercentages[i] = 100.0f / self.weatherTypes.Length;
            }
            GUI.changed = true;
        }

        EditorGUILayout.EndHorizontal();

        self.CheckTotal(ref self.weatherPercentages, 100.0f, true);

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        for (int j = 0; j < self.weatherTypes.Length; j++)
        {
            self.cloudTypes[j] = (TOD_CloudType)EditorGUILayout.EnumPopup(self.weatherTypes[j] + " Clouds", self.cloudTypes[j]);
        }

        EditorGUILayout.Space();

        // Current weather
        EditorGUILayout.TextField("Current Weather", self.currentWeather.ToString());

        // Current clouds
        EditorGUILayout.TextField("Current Clouds", self.currentCloudType.ToString());

        // Weather checks per day
        self.checksPerDay = EditorGUILayout.IntSlider("Checks/Day:", self.checksPerDay, 1, 10);
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUI.color = new Color(1.0f, 0.75f, 0.75f);

        // Thunder/Lightning Settings
        EditorGUILayout.LabelField("Thunder/Lightning Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Thunder and lightning parameters
        self.enableThunderAndLightning = EditorGUILayout.Toggle("Enable T/L", self.enableThunderAndLightning);
        if (self.enableThunderAndLightning)
        {
            self.lightningColor = EditorGUILayout.ColorField("Lightning Color:", self.lightningColor);
            self.doLightningGlow = EditorGUILayout.Toggle("Lightning Glow:", self.doLightningGlow);
            if (self.doLightningGlow)
            {
                self.lightningGlowColor = EditorGUILayout.ColorField("Glow Color:", self.lightningGlowColor);
                self.lightningGlowWidth = EditorGUILayout.Slider("Glow Width:", self.lightningGlowWidth, 1.0f, 100.0f);
                self.lightningOriginGlowColor = EditorGUILayout.ColorField("Origin Glow Color:", self.lightningOriginGlowColor);
                self.lightningOriginGlowWidth = EditorGUILayout.Slider("Origin Glow Width:", self.lightningOriginGlowWidth, 1.0f, 200.0f);
            }
            self.lightningFlashIntensity = EditorGUILayout.Slider("Flash Intensity:", self.lightningFlashIntensity, 0, 1);
            self.minTimeBetweenStrikes = EditorGUILayout.Slider("Min Time/Strikes:", self.minTimeBetweenStrikes, 1.0f, 120.0f);
            self.maxTimeBetweenStrikes = EditorGUILayout.Slider("Max Time/Strikes:", self.maxTimeBetweenStrikes, 1.0f, 120.0f);
            self.lifeTime = EditorGUILayout.Slider("Strike Lifetime:", self.lifeTime, 0.5f, 2.0f);
            self.lightningWidth = EditorGUILayout.Slider("Lightning Width:", self.lightningWidth, 0.5f, 2.0f);
            self.lightningInnerConeAngle = EditorGUILayout.Slider("Inner Cone Angle:", self.lightningInnerConeAngle, 0, 90);
            self.maxLightningHitDistance = EditorGUILayout.Slider("Max Strike Distance:", self.maxLightningHitDistance, 1.0f, 10000.0f);
            self.numVertices = EditorGUILayout.IntSlider("Num Vertices:", self.numVertices, 10, 40);
            self.rangeMin = EditorGUILayout.Slider("Range Min", self.rangeMin, 0.0f, -100.0f);
            self.rangeMax = EditorGUILayout.Slider("Range Max", self.rangeMax, 0.0f, 100.0f);
            self.maxDeviation = EditorGUILayout.Slider("Max Deviation:", self.maxDeviation, 0.0f, 50.0f);
            self.nonStrikeLength = EditorGUILayout.Slider("Non-Strike Length:", self.nonStrikeLength, 1.0f, 10.0f);
            self.minNumBranchVerts = EditorGUILayout.IntSlider("Min Branch Verts:", self.minNumBranchVerts, 1, 20);
            self.maxNumBranchVerts = EditorGUILayout.IntSlider("Max Branch Verts:", self.maxNumBranchVerts, 1, 20);
            self.minBranchLength = EditorGUILayout.Slider("Min Branch Length:", self.minBranchLength, 1.0f, 50.0f);
            self.maxBranchLength = EditorGUILayout.Slider("Max Branch Length:", self.maxBranchLength, 1.0f, 50.0f);
            self.maxBranchDeviation = EditorGUILayout.Slider("Max Branch Deviation:", self.maxBranchDeviation, 1.0f, 50.0f);
            self.branchStartPercentage = EditorGUILayout.Slider("Branch Start %:", self.branchStartPercentage, 0, 1);
            self.branchSpacing = EditorGUILayout.IntSlider("Branch Spacing:", self.branchSpacing, 1, 10);
            self.maxTLAudioLevel = EditorGUILayout.Slider("Max Audio Level:", self.maxTLAudioLevel, 0, 1);

            serializedObject.Update();
            SerializedProperty arrayToCheck = serializedObject.FindProperty("thunderSounds");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(arrayToCheck, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            EditorGUIUtility.LookLikeControls();

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset Thunder/Lightning Defaults"))
            {
                self.SetThunderAndLightningDefaults();
                GUI.changed = true;
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUI.color = new Color(0.75f, 1.0f, 0.75f);

        // Rain Settings
        EditorGUILayout.LabelField("Rain Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Rain parameters
        self.enableRain = EditorGUILayout.Toggle("Enable Rain", self.enableRain);
        if (self.enableRain)
        {
            self.enableFogWithRain = EditorGUILayout.Toggle("Enable Fog/Rain", self.enableFogWithRain);
            self.rainIsEffectedByWind = EditorGUILayout.Toggle("Affected by Wind", self.rainIsEffectedByWind);
            self.rainHeightAbove = EditorGUILayout.Slider("Height Above Cam:", self.rainHeightAbove, 0.0f, 250.0f);
            self.minRainParticleSize = EditorGUILayout.Slider("Min Drop Size:", self.minRainParticleSize, 0.001f, 1.0f);
            self.maxRainParticleSize = EditorGUILayout.Slider("Max Drop Size:", self.maxRainParticleSize, 0.001f, 1.0f);
            self.minNumRainParticles = EditorGUILayout.IntSlider("Min Num Drops:", self.minNumRainParticles, 1, 5000);
            self.maxNumRainParticles = EditorGUILayout.IntSlider("Max Num Drops:", self.maxNumRainParticles, 1, 5000);
            self.splashEnergy = EditorGUILayout.Slider("Splash Energy:", self.splashEnergy, 1.0f, 20.0f);
            self.splashObject = (Transform)EditorGUILayout.ObjectField("Splash Object:", self.splashObject, typeof(Transform), true);
            self.rainColor = EditorGUILayout.ColorField("Rain Color:", self.rainColor);
            self.rainColorMin = EditorGUILayout.ColorField("Rain Color Min:", self.rainColorMin);
            self.maxRainAudioLevel = EditorGUILayout.Slider("Max Audio Level", self.maxRainAudioLevel, 0, 1);

            serializedObject.Update();
            SerializedProperty arrayToCheck = serializedObject.FindProperty("rainSounds");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(arrayToCheck, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            EditorGUIUtility.LookLikeControls();

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset Rain Defaults"))
            {
                self.SetRainDefaults();
                GUI.changed = true;
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUI.color = new Color(0.75f, 0.75f, 1.0f);

        // Fog Settings
        EditorGUILayout.LabelField("Fog Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Fog parameters
        self.enableFog = EditorGUILayout.Toggle("Enable Fog", self.enableFog);
        if (self.enableFog)
        {
            if (SystemInfo.supportsRenderTextures) // Check if user has Pro version of Unity
            {
                self.useGlobalFog = EditorGUILayout.Toggle("Use Global Fog", self.useGlobalFog);
            }
            else
            {
                self.useGlobalFog = false;
            }

            if (self.useGlobalFog) 
            {

#if UNITY_4
                if (!Camera.main.GetComponent<GlobalFog_U4>())
                {
                    Shader globalFogShader = Shader.Find("Hidden/GlobalFog_U4");
                    GlobalFog_U4 globalFogScript = Camera.main.gameObject.AddComponent<GlobalFog_U4>();
                    globalFogScript.fogShader = globalFogShader;
                }

                self.globalFogMode = (GlobalFog_U4.FogMode)EditorGUILayout.EnumPopup("Global Fog Mode", self.globalFogMode);
                self.globalFogHeightScale = EditorGUILayout.Slider("Fog Height Scale:", self.globalFogHeightScale, 1.0f, 100.0f);
#else
                if (!Camera.main.GetComponent<GlobalFog_U5>())
	            {
	                Shader globalFogShader = Shader.Find("Hidden/GlobalFog_U5");
	                GlobalFog_U5 globalFogScript = Camera.main.gameObject.AddComponent<GlobalFog_U5>();
	                globalFogScript.fogShader = globalFogShader;
	            }

                self.distanceFog = EditorGUILayout.Toggle("Use Distance Fog", self.distanceFog);
                self.useRadialDistance = EditorGUILayout.Toggle("Use Radial Distance", self.useRadialDistance);
                self.heightFog = EditorGUILayout.Toggle("Use Height Fog", self.heightFog);
                self.heightDensity = EditorGUILayout.Slider("Fog Height Density:", self.heightDensity, 0.0f, 1000.0f);
                self.fogStartOffset = EditorGUILayout.Slider("Fog Start Offset:", self.fogStartOffset, 0.0f, self.fogEndDistance - 0.001f);
#endif
                self.globalFogHeight = EditorGUILayout.Slider("Fog Height:", self.globalFogHeight, 1.0f, 1000.0f);
            }
            else
            {

#if UNITY_4
                if (Camera.main.GetComponent<GlobalFog_U4>())
                {
                    DestroyImmediate(Camera.main.gameObject.GetComponent<GlobalFog_U4>());
                }
#else
                if (Camera.main.GetComponent<GlobalFog_U5>())
	            {
                    DestroyImmediate(Camera.main.gameObject.GetComponent<GlobalFog_U5>());
                }
#endif

                self.fogModeToUse = (FogMode)EditorGUILayout.EnumPopup("Fog Mode", self.fogModeToUse);
            }

            self.fogStartDistance = EditorGUILayout.Slider("Fog Start Distance:", self.fogStartDistance, 0.0f, 1000.0f);
            self.fogEndDistance = EditorGUILayout.Slider("Fog End Distance:", self.fogEndDistance, 0.0f, 1000.0f);
            self.fogDensityMin = EditorGUILayout.Slider("Fog Density Min:", self.fogDensityMin, 0.0f, 1.0f);
            self.fogDensityMax = EditorGUILayout.Slider("Fog Density Max:", self.fogDensityMax, 0.0f, 1.0f);
            self.fogFadeScale = EditorGUILayout.Slider("Fog Fade Scale:", self.fogFadeScale, 0.0f, 1.0f);
            self.maxFogAudioLevel = EditorGUILayout.Slider("Max Audio Level", self.maxFogAudioLevel, 0, 1);

            serializedObject.Update();
            SerializedProperty arrayToCheck = serializedObject.FindProperty("fogSounds");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(arrayToCheck, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            EditorGUIUtility.LookLikeControls();

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset Fog Defaults"))
            {
                self.SetFogDefaults();
                GUI.changed = true;
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUI.color = new Color(0.75f, 0.75f, 0.0f);

        // Dust Settings
        EditorGUILayout.LabelField("Dust Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Dust parameters
        self.enableDust = EditorGUILayout.Toggle("Enable Dust", self.enableDust);
        if (self.enableDust)
        {
            self.dustIsAffectedByWind = EditorGUILayout.Toggle("Affected by Wind", self.dustIsAffectedByWind);
            self.dustHeightAbove = EditorGUILayout.Slider("Height Above Cam:", self.dustHeightAbove, 0.0f, 10.0f);
            self.minDustParticleSize = EditorGUILayout.Slider("Min Particle Size:", self.minDustParticleSize, 0.001f, 10.0f);
            self.maxDustParticleSize = EditorGUILayout.Slider("Max Particle Size:", self.maxDustParticleSize, 0.001f, 10.0f);
            self.minNumDustParticles = EditorGUILayout.IntSlider("Min Num Particles:", self.minNumDustParticles, 1, 5000);
            self.maxNumDustParticles = EditorGUILayout.IntSlider("Max Num Particles:", self.maxNumDustParticles, 1, 5000);
            self.dustColor = EditorGUILayout.ColorField("Dust Color", self.dustColor);
            self.maxDustAudioLevel = EditorGUILayout.Slider("Max Audio Level", self.maxDustAudioLevel, 0, 1);

            serializedObject.Update();
            SerializedProperty arrayToCheck = serializedObject.FindProperty("dustSounds");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(arrayToCheck, true);
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            EditorGUIUtility.LookLikeControls();

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset Dust Defaults"))
            {
                self.SetDustDefaults();
                GUI.changed = true;
            }
        }
        EditorGUILayout.Space();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(self);
            self.isDirty = true;
        }
    }
}
