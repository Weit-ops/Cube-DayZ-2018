using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiendCamera : MonoBehaviour
{
    private float defaultFarClipPlane;
    private float defaultFogDensity;
    private Camera _cam;

    void Start()
    {
        _cam = GetComponent<Camera>();
        OnQualitySettings();
    }
    private void OnQualitySettings()
    {
        float farClipPlane = Mathf.Clamp((float)(128 * (QualitySettings.GetQualityLevel() + 1)), 128f, this.defaultFarClipPlane);
        this._cam.farClipPlane = farClipPlane;
        float[] array = new float[32];
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = Mathf.Clamp((float)(32 * (QualitySettings.GetQualityLevel() + 1)), 32f, this.defaultFarClipPlane);
        }
        array[0] = 0f;
        this._cam.layerCullDistances = array;
        RenderSettings.fogEndDistance = array[13];
        this.defaultFogDensity = array[13] / this.defaultFarClipPlane;
        RenderSettings.fogDensity = this.defaultFogDensity;
    }
    // Update is called once per frame
}
