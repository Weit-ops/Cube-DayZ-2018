using UnityEngine;
using System.Collections;

/// <summary>
/// Class for Dust Storm
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DustElement : WeatherElement_Shuriken
{
    private Color dustColor = Color.gray;
    private Color dustColorCurrent = Color.gray;

    #region properties

    /// <summary>
    /// Gets or sets the color of the dust.
    /// </summary>
    /// <value>
    /// The color of the dust.
    /// </value>
    public Color DustColor
    {
        get { return dustColor; }
        set { dustColor = value; }
    }

    #endregion

    /// <summary>
    /// Used to initialize Dust element
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        IsInitialized = true;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        dustColorCurrent.r = dustColor.r /** SunLight.color.r * SunLight.intensity*/;
        dustColorCurrent.g = dustColor.g /** SunLight.color.g * SunLight.intensity*/;
        dustColorCurrent.b = dustColor.b /** SunLight.color.b * SunLight.intensity*/;
        dustColorCurrent.a = dustColor.a;

        GetComponent<Renderer>().material.SetColor("_TintColor", dustColorCurrent);

    }

    public override void Transition(bool fadeIn)
    {
        base.Transition(fadeIn);
    }

    public override void Reset()
    {
        base.Reset();
    }
}
