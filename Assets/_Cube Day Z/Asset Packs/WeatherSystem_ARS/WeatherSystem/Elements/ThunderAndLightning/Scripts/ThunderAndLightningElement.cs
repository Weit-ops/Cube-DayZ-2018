#if UNITY_4_0||UNITY_4_1||UNITY_4_2||UNITY_4_3||UNITY_4_4||UNITY_4_5||UNITY_4_6||UNITY_4_7||UNITY_4_8||UNITY_4_9
#define UNITY_4
#endif

using UnityEngine;
using System.Collections;

/// <summary>
/// Class for Thunder and Lightning Elements
/// </summary>
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Light))]
[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(ParticleSystemRenderer))]
public class ThunderAndLightningElement : WeatherElement_Shuriken
{
    private Color lightningColor = Color.white;
    private bool doLightningGlow = true;
    private float lightningGlowWidth = 50.0f;
    private Color lightningGlowColor = Color.white;
    private float lightningOriginGlowWidth = 100.0f;
    private Color lightningOriginGlowColor = Color.white;
    private float lightningFlashIntensity = 1.0f;
    private float minTimeBetweenStrikes = 10.0f;
    private float maxTimeBetweenStrikes = 20.0f;
    private float lifeTime = 0.5f;
    private float lightningWidth = 1.0f;
    private float innerConeAngle = 15.0f;
    private float outerConeAngle = 90.0f;
    private float maxLightningHitDistance = 5000.0f;
    private int numVertices = 20;
    private float rangeMin = -20.0f;
    private float rangeMax = 20.0f;
    private float maxDeviation = 10.0f;
    private float nonStrikeLenth = 5.0f;

    private int minNumBranchVerts = 4;
    private int maxNumBranchVerts = 8;
    private float minBranchLength = 10.0f;
    private float maxBranchLength = 20.0f;
    private float maxBranchDeviation = 5.0f;
    private float branchStartPercentage = 0.3f;
    private int branchSpacing = 2;
    
    private LineRenderer lineRenderer;
    private Light lightFlash;
    private float currentTimer = 0.0f;

    private float sizeOfAtmosphere = 0.0f;

    private Material particleMat;

    public GameObject branch;

    #region properties

    /// <summary>
    /// Gets or sets the color of the lightning.
    /// </summary>
    /// <value>
    /// The color of the lightning.
    /// </value>
    public Color LightningColor
    {
        get { return lightningColor; }
        set { lightningColor = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [do lightning glow].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [do lightning glow]; otherwise, <c>false</c>.
    /// </value>
    public bool DoLightningGlow
    {
        get { return doLightningGlow; }
        set { doLightningGlow = value; }
    }

    /// <summary>
    /// Gets or sets the width of the lightning glow.
    /// </summary>
    /// <value>
    /// The width of the lightning glow.
    /// </value>
    public float LightningGlowWidth
    {
        get { return lightningGlowWidth; }
        set { lightningGlowWidth = value; }
    }

    /// <summary>
    /// Gets or sets the color of the lightning glow.
    /// </summary>
    /// <value>
    /// The color of the lightning glow.
    /// </value>
    public Color LightningGlowColor
    {
        get { return lightningGlowColor; }
        set { lightningGlowColor = value; }
    }

    /// <summary>
    /// Gets or sets the width of the lightning origin glow.
    /// </summary>
    /// <value>
    /// The width of the lightning origin glow.
    /// </value>
    public float LightningOriginGlowWidth
    {
        get { return lightningOriginGlowWidth; }
        set { lightningOriginGlowWidth = value; }
    }

    /// <summary>
    /// Gets or sets the color of the lightning origin glow.
    /// </summary>
    /// <value>
    /// The color of the lightning origin glow.
    /// </value>
    public Color LightningOriginGlowColor
    {
        get { return lightningOriginGlowColor; }
        set { lightningOriginGlowColor = value; }
    }

    /// <summary>
    /// Gets or sets the lightning flash intensity.
    /// </summary>
    /// <value>
    /// The lightning flash intensity.
    /// </value>
    public float LightningFlashIntensity
    {
        get { return lightningFlashIntensity; }
        set { lightningFlashIntensity = value; }
    }
    
    /// <summary>
    /// Gets or sets the minimum time between strikes.
    /// </summary>
    /// <value>
    /// The minimum time between strikes.
    /// </value>
    public float MinTimeBetweenStrikes
    {
        get { return minTimeBetweenStrikes; }
        set { minTimeBetweenStrikes = value; }
    }

    /// <summary>
    /// Gets or sets the maximum time between strikes.
    /// </summary>
    /// <value>
    /// The maximum time between strikes.
    /// </value>
    public float MaxTimeBetweenStrikes
    {
        get { return maxTimeBetweenStrikes; }
        set { maxTimeBetweenStrikes = value; }
    }

    /// <summary>
    /// Gets or sets the width of the lightning.
    /// </summary>
    /// <value>
    /// The width of the lightning.
    /// </value>
    public float LightningWidth
    {
        get { return lightningWidth; }
        set { lightningWidth = value; }
    }

    /// <summary>
    /// Gets or sets the life time of the lightning.
    /// </summary>
    /// <value>
    /// The life time of the lightning.
    /// </value>
    public float LifeTime
    {
        get { return lifeTime; }
        set { lifeTime = value; }
    }

    /// <summary>
    /// Gets or sets the inner cone angle.
    /// </summary>
    /// <value>
    /// The inner cone angle.
    /// </value>
    public float InnerConeAngle
    {
        get { return innerConeAngle; }
        set { innerConeAngle = value; }
    }

    /// <summary>
    /// Gets or sets the outer cone angle.
    /// </summary>
    /// <value>
    /// The outer cone angle.
    /// </value>
    public float OuterConeAngle
    {
        get { return outerConeAngle; }
        set { outerConeAngle = value; }
    }

    /// <summary>
    /// Gets or sets the maximum lightning hit distance.
    /// </summary>
    /// <value>
    /// The maximum lightning hit distance.
    /// </value>
    public float MaxLightningHitDistance
    {
        get { return maxLightningHitDistance; }
        set { maxLightningHitDistance = value; }
    }

    /// <summary>
    /// Gets or sets the number of vertices in the lightning bolt.
    /// </summary>
    /// <value>
    /// The number of vertices in the lightning bolt.
    /// </value>
    public int NumVertices
    {
        get { return numVertices; }
        set { numVertices = value; }
    }

    /// <summary>
    /// Gets or sets the minimum XZ range of the lightning end point
    /// from the start point.
    /// </summary>
    /// <value>
    /// The minimum range for XZ.
    /// </value>
    public float RangeMin
    {
        get { return rangeMin; }
        set { rangeMin = value; }
    }

    /// <summary>
    /// Gets or sets the maximum XZ range of the lightning end point
    /// from the start point.
    /// </summary>
    /// <value>
    /// The range maximum.
    /// </value>
    public float RangeMax
    {
        get { return rangeMax; }
        set { rangeMax = value; }
    }

    /// <summary>
    /// Gets or sets the maximum deviation of the lightning vertices.
    /// </summary>
    /// <value>
    /// The maximum deviation of the lightning vertices.
    /// </value>
    public float MaxDeviation
    {
        get { return maxDeviation; }
        set { maxDeviation = value; }
    }


    /// <summary>
    /// Gets or sets the length of a non strike.
    /// </summary>
    /// <value>
    /// The length of the non strike.
    /// </value>
    public float NonStrikeLength
    {
        get { return nonStrikeLenth; }
        set { nonStrikeLenth = value; }
    }

    /// <summary>
    /// Gets or sets the minimum number branch verts.
    /// </summary>
    /// <value>
    /// The minimum number branch verts.
    /// </value>
    public int MinNumBranchVerts
    {
        get { return minNumBranchVerts; }
        set { minNumBranchVerts = value; }
    }

    /// <summary>
    /// Gets or sets the maximum number branch verts.
    /// </summary>
    /// <value>
    /// The maximum number branch verts.
    /// </value>
    public int MaxNumBranchVerts
    {
        get { return maxNumBranchVerts; }
        set { maxNumBranchVerts = value; }
    }

    /// <summary>
    /// Gets or sets the minimum length of the branch.
    /// </summary>
    /// <value>
    /// The minimum length of the branch.
    /// </value>
    public float MinBranchLength
    {
        get { return minBranchLength; }
        set { minBranchLength = value; }
    }

    /// <summary>
    /// Gets or sets the maximum length of the branch.
    /// </summary>
    /// <value>
    /// The maximum length of the branch.
    /// </value>
    public float MaxBranchLength
    {
        get { return maxBranchLength; }
        set { maxBranchLength = value; }
    }

    /// <summary>
    /// Gets or sets the maximum branch deviation.
    /// </summary>
    /// <value>
    /// The maximum branch deviation.
    /// </value>
    public float MaxBranchDeviation
    {
        get { return maxBranchDeviation; }
        set { maxBranchDeviation = value; }
    }

    /// <summary>
    /// Gets or sets the branch start percentage.
    /// </summary>
    /// <value>
    /// The branch start percentage.
    /// </value>
    public float BranchStartPercentage
    {
        get { return branchStartPercentage; }
        set { branchStartPercentage = value; }
    }

    /// <summary>
    /// Gets or sets the branch spacing.
    /// </summary>
    /// <value>
    /// The branch spacing.
    /// </value>
    public int BranchSpacing
    {
        get { return branchSpacing; }
        set { branchSpacing = value; }
    }

    #endregion

    /// <summary>
    /// Used to initialize Thunder and Lightning elements
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        currentTimer = Random.Range(minTimeBetweenStrikes, maxTimeBetweenStrikes);
        lineRenderer = GetComponent<LineRenderer>();
        lightFlash = GetComponent<Light>();

        lightFlash.range = maxLightningHitDistance;

        lightFlash.intensity = 0.0f;

        if (lifeTime > maxTimeBetweenStrikes)
            lifeTime = maxTimeBetweenStrikes;

        transform.position = CameraObject.transform.position;

        IsInitialized = true;
    }

    protected override void Start()
    {
        base.Start();

        particleMat = GetComponent<ParticleSystemRenderer>().sharedMaterial;
    }

    /// <summary>
    /// Clears the line renderer.
    /// </summary>
    /// <param name="lr">The LineRenderer.</param>
    /// <param name="delay">The delay.</param>
    /// <returns></returns>
    private IEnumerator ClearLineRenderer(LineRenderer lr, float delay)
    {
        yield return new WaitForSeconds(delay);
        lr.positionCount = 0;
    }

#if UNITY_4
    /// <summary>
    /// Clears the particles for Unity 4.
    /// </summary>
    /// <param name="pe">The ParticleEmitter.</param>
    /// <param name="delay">The delay.</param>
    /// <returns></returns>
    private IEnumerator ClearParticles_U4(ParticleEmitter pe, float delay)
    {
        yield return new WaitForSeconds(delay);
        pe.ClearParticles();
    }

#else
    /// <summary>
    /// Clears the particles for Unity 5.
    /// </summary>
    /// <param name="pe">The EllipsoidParticleEmitter.</param>
    /// <param name="delay">The delay.</param>
    /// <returns></returns>
    private IEnumerator ClearParticles_U5(ParticleSystem ps, float delay)
    {
        yield return new WaitForSeconds(delay);
        ps.Clear();
    }
#endif

    /// <summary>
    /// Generates the lightning bolt.
    /// </summary>
    private void GenerateLightning()
    {
        if (Atmosphere == null)
            return;

        Vector3 startPoint = UnitSphere.GetPointOnRingY(innerConeAngle, outerConeAngle, transform, sizeOfAtmosphere);
        startPoint.y = Mathf.Abs(startPoint.y);
        
        Vector3 randomPoint = Vector3.Normalize(new Vector3(startPoint.x + Random.Range(rangeMin, rangeMax),
                                                            startPoint.y - maxLightningHitDistance,
                                                            startPoint.z + Random.Range(rangeMin, rangeMax)));

        RaycastHit hit;
        float distance = 0.0f;
        Vector3 endPoint = Vector3.zero;

        if (Physics.Raycast(startPoint, randomPoint, out hit, maxLightningHitDistance))
        {
            distance = transform.position.y - hit.point.y;
            endPoint = hit.point;
        }
        else
        {
            distance = transform.position.y - nonStrikeLenth;
            endPoint = new Vector3(0.0f, distance, 0.0f);
        }

        endPoint.x = Random.Range(startPoint.x + rangeMin, startPoint.x + rangeMax);
        endPoint.z = Random.Range(startPoint.z + rangeMin, startPoint.z + rangeMax);

#if UNITY_4
        lineRenderer.castShadows = false;
#else
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
#endif

        lineRenderer.receiveShadows = false;
		lineRenderer.startWidth = lightningWidth;
		lineRenderer.endWidth = 0.0f;
        lineRenderer.positionCount = numVertices;
        lineRenderer.GetComponent<Renderer>().material.SetColor("_TintColor", lightningColor);

        Vector3 tempStart = startPoint;
        Vector3 tempEnd = new Vector3(0.0f, 0.0f, 0.0f);

        float tempBiasX = (endPoint.x - startPoint.x) / numVertices;
        float tempBiasZ = (endPoint.z - startPoint.z) / numVertices;
        float tempBiasY = (endPoint.y - startPoint.y) / numVertices;

        lineRenderer.SetPosition(0, startPoint);

        ParticleSystem.Particle[] particles = null;
        if (doLightningGlow)
        {

/*#if UNITY_4
            GetComponent<ParticleEmitter>().Emit(numVertices);
            particles = GetComponent<ParticleEmitter>().particles;
#else
            GetComponent<EllipsoidParticleEmitter>().Emit(numVertices);
            particles = GetComponent<EllipsoidParticleEmitter>().particles;
#endif*/
			GetComponent<ParticleSystem> ().Emit (numVertices);
			particles = new ParticleSystem.Particle[GetComponent<ParticleSystem> ().main.maxParticles];

            particles[0].position = startPoint;
            particles[0].startColor = lightningOriginGlowColor;
            particles[0].startSize = lightningOriginGlowWidth;
        }

        for (int i = 1; i < numVertices; i++)
        {
            tempEnd.y = tempStart.y + tempBiasY;
            tempEnd.x = Random.Range(tempStart.x - maxDeviation + tempBiasX, tempStart.x + maxDeviation + tempBiasX);
            tempEnd.z = Random.Range(tempStart.z - maxDeviation + tempBiasZ, tempStart.z + maxDeviation + tempBiasZ);
            if (i > (numVertices * branchStartPercentage))
            {
                if((i % branchSpacing) == 0)
                    GenerateLightningBranch(tempEnd, lifeTime);
            }
            lineRenderer.SetPosition(i, tempEnd);
            if (doLightningGlow)
            {
                particles[i].position = tempEnd;
                particles[i].startColor = lightningGlowColor;
                particles[i].startSize = lightningGlowWidth;
            }
            tempStart = tempEnd;
        }

        if (doLightningGlow)
        {
            particles[numVertices - 1].position = endPoint;
            particles[numVertices - 1].startColor = lightningGlowColor;
            particles[numVertices - 1].startSize = lightningGlowWidth;

/*#if UNITY_4
            GetComponent<ParticleEmitter>().particles = particles;
#else
            GetComponent<EllipsoidParticleEmitter>().particles = particles;
#endif*/
			GetComponent<ParticleSystem> ().SetParticles (particles, particles.Length);
        }

        lineRenderer.SetPosition(numVertices - 1, endPoint);

        GenerateThunder(endPoint);

        GenerateLightningFlash(lifeTime, endPoint);

        StartCoroutine(ClearLineRenderer(lineRenderer, lifeTime));

        if (doLightningGlow)
        {

#if UNITY_4
            StartCoroutine(ClearParticles_U4(GetComponent<ParticleEmitter>(), lifeTime));
#else
            StartCoroutine(ClearParticles_U5(GetComponent<ParticleSystem>(), lifeTime));
#endif
        }
    }

    /// <summary>
    /// Generates the lightning branch.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="destroyDelay">The destroy delay.</param>
    private void GenerateLightningBranch(Vector3 position, float destroyDelay)
    {
        GameObject go = Transform.Instantiate(branch, position, Quaternion.identity) as GameObject;
        LineRenderer lr = go.AddComponent<LineRenderer>();
        
/*#if UNITY_4
        ParticleEmitter pe = null;
#else
        EllipsoidParticleEmitter pe = null;
#endif*/
		ParticleSystem ps = null;
        ParticleSystemRenderer pr = null;

        if (doLightningGlow)
        {

/*#if UNITY_4
            pe = go.GetComponent<ParticleEmitter>();
#else
            pe = go.GetComponent<EllipsoidParticleEmitter>();
#endif*/
			ps = go.GetComponent<ParticleSystem>();
            pr = go.GetComponent<ParticleSystemRenderer>();

#if UNITY_4
            pr.castShadows = false;
#else
            pr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
#endif

            pr.receiveShadows = false;
            pr.renderMode = ParticleSystemRenderMode.Billboard;
            pr.material = particleMat;
        }
        
        int numVerts = Random.Range(minNumBranchVerts, maxNumBranchVerts);

        Vector3 startPoint = go.transform.position;
        Vector3 endPoint = Vector3.zero;

        endPoint.x = Random.Range(startPoint.x + minBranchLength, startPoint.x + maxBranchLength);
        endPoint.y = Random.Range(startPoint.y - minBranchLength, startPoint.y - maxBranchLength);
        endPoint.z = Random.Range(startPoint.z + minBranchLength, startPoint.z + maxBranchLength);

#if UNITY_4
        lr.castShadows = false;
#else
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
#endif
        
        lr.receiveShadows = false;
        lr.sharedMaterial = lineRenderer.sharedMaterial;
        lr.startWidth = lightningWidth;
		lr.endWidth = 0.0f;
        lr.positionCount = numVerts;
        lr.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", lightningColor);

        Vector3 tempStart = startPoint;
        Vector3 tempEnd = new Vector3(0.0f, 0.0f, 0.0f);

        float tempBiasX = (endPoint.x - startPoint.x) / numVerts;
        float tempBiasZ = (endPoint.z - startPoint.z) / numVerts;
        float tempBiasY = (endPoint.y - startPoint.y) / numVerts;

        lr.SetPosition(0, startPoint);

        ParticleSystem.Particle[] particles = null;
        if (doLightningGlow)
        {
            ps.Emit(numVerts);
            //particles = pe.particles;
			particles = new ParticleSystem.Particle[ps.main.maxParticles];
			//ps.GetParticles (particles);
            particles[0].position = startPoint;
            particles[0].startColor = lightningGlowColor;
            particles[0].startSize = lightningGlowWidth;
        }

        for (int i = 1; i < numVerts; i++)
        {
            tempEnd.y = tempStart.y + tempBiasY;
            tempEnd.x = Random.Range(tempStart.x - maxBranchDeviation + tempBiasX, tempStart.x + maxBranchDeviation + tempBiasX);
            tempEnd.z = Random.Range(tempStart.z - maxBranchDeviation + tempBiasZ, tempStart.z + maxBranchDeviation + tempBiasZ);

            lr.SetPosition(i, tempEnd);
            if (doLightningGlow)
            {
                particles[i].position = tempEnd;
                particles[i].startColor = lightningGlowColor;
                particles[i].startSize = lightningGlowWidth;
            }
            tempStart = tempEnd;
        }

        if(doLightningGlow)
        {
            particles[numVerts - 1].position = endPoint;
            particles[numVerts - 1].startColor = lightningGlowColor;
            particles[numVerts - 1].startSize = lightningGlowWidth;
            //pe.particles = particles;
			ps.SetParticles (particles, particles.Length);
        }

        lr.SetPosition(numVerts - 1, endPoint);

        Destroy(go, destroyDelay);
    }

    /// <summary>
    /// Generates the thunder sound based on distance from a position.
    /// </summary>
    /// <param name="position">The position.</param>
    private void GenerateThunder(Vector3 position)
    {
        if (ElementSounds.Length < 1)
            return;

        float volume = 1.0f;

        if (CameraObject)
        {
            float distance = Vector3.Distance(position, CameraObject.transform.position);
            volume = ((sizeOfAtmosphere / 2.0f) / distance);
        }

        AudioSource.PlayClipAtPoint(ElementSounds[Random.Range(0, ElementSounds.Length - 1)], 
                                                    CameraObject.transform.position, volume * MaxAudioLevel);
    }


    /// <summary>
    /// Generates the lightning flash based on duration and 
    /// distance from position.
    /// </summary>
    /// <param name="duration">The duration.</param>
    /// <param name="position">The position.</param>
    private void GenerateLightningFlash(float duration, Vector3 position)
    {
        float intensity = 1.0f;

        if (CameraObject)
        {
            float distance = Vector3.Distance(position, CameraObject.transform.position);
            intensity = ((sizeOfAtmosphere / 2.0f) / distance);
        }

        lightFlash.intensity = intensity * lightningFlashIntensity;
        Invoke("StopLightningFlash", duration);
    }

    /// <summary>
    /// Stops the lightning flash.
    /// </summary>
    private void StopLightningFlash()
    {
        lightFlash.intensity = 0.0f;
    }

    protected override void Update()
    {
        if (!WeatherSystem.isTODPresent || !IsInitialized)
            return;

        transform.position = CameraObject.transform.position;
        sizeOfAtmosphere = (Atmosphere.transform.lossyScale.y / 2.0f) - 0.1f;
    }

    protected override void FixedUpdate()
    {
        if (!WeatherSystem.isTODPresent || !IsInitialized)
            return;

        if (!EnableElement)
            return;
        
        currentTimer -= Time.deltaTime;
        if (currentTimer > 0.0f)
            return;
        else
            currentTimer = Random.Range(minTimeBetweenStrikes, maxTimeBetweenStrikes);

        GenerateLightning();
    }
}
