using System.Collections;
using UnityEngine;

public class RMCPoliceLights : MonoBehaviour
{
	public GameObject dummyIllumin;

	private Shader illumin;

	private RMCRealisticMotorcycleController motorScript;

	public GameObject headLight;

	public GameObject brakeLight;

	public GameObject[] lights;

	public GameObject[] rightSignals;

	public GameObject[] leftSignals;

	public GameObject[] miscLights;

	private bool headLightsOn;

	public bool sirenOn;

	private Shader defaultShader;

	private Light[] lightSources;

	private Light[] rightSignalsLightSources;

	private Light[] leftSignalsLightSources;

	private Light[] miscLightSources;

	private Light headLightLightSource;

	public bool GetHeadLightsOn()
	{
		return headLightsOn;
	}

	private void Start()
	{
		if ((bool)dummyIllumin)
		{
			illumin = dummyIllumin.GetComponent<MeshRenderer>().material.shader;
		}
		else
		{
			illumin = Shader.Find("Self-Illumin/Specular");
		}
		defaultShader = Shader.Find("Bumped Specular");
		motorScript = GetComponent<RMCRealisticMotorcycleController>();
		lightSources = new Light[lights.Length];
		rightSignalsLightSources = new Light[rightSignals.Length];
		leftSignalsLightSources = new Light[leftSignals.Length];
		miscLightSources = new Light[miscLights.Length];
		headLightLightSource = new Light();
		headLightLightSource = headLight.GetComponentInChildren<Light>();
		for (int i = 0; i < lights.Length; i++)
		{
			lightSources[i] = lights[i].GetComponentInChildren<Light>();
		}
		for (int j = 0; j < rightSignals.Length; j++)
		{
			rightSignalsLightSources[j] = rightSignals[j].GetComponentInChildren<Light>();
		}
		for (int k = 0; k < leftSignals.Length; k++)
		{
			leftSignalsLightSources[k] = leftSignals[k].GetComponentInChildren<Light>();
		}
		for (int l = 0; l < miscLights.Length; l++)
		{
			miscLightSources[l] = miscLights[l].GetComponentInChildren<Light>();
		}
		StartCoroutine(FlashLights());
		StartCoroutine(SignalLights());
		StartCoroutine(MiscLights());
	}

	private IEnumerator FlashLights()
	{
		yield return new WaitForSeconds(1f);
		if (sirenOn)
		{
			for (int i = 0; i < lights.Length; i++)
			{
				lights[i].GetComponent<MeshRenderer>().material.shader = illumin;
				lightSources[i].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				lights[i].GetComponent<MeshRenderer>().material.shader = defaultShader;
				lightSources[i].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
				lights[i].GetComponent<MeshRenderer>().material.shader = illumin;
				lightSources[i].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				lights[i].GetComponent<MeshRenderer>().material.shader = defaultShader;
				lightSources[i].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
				lights[i].GetComponent<MeshRenderer>().material.shader = illumin;
				lightSources[i].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				lights[i].GetComponent<MeshRenderer>().material.shader = defaultShader;
				lightSources[i].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
				lights[i].GetComponent<MeshRenderer>().material.shader = illumin;
				lightSources[i].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				lights[i].GetComponent<MeshRenderer>().material.shader = defaultShader;
				lightSources[i].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
			}
		}
		StartCoroutine(FlashLights());
	}

	private IEnumerator SignalLights()
	{
		yield return new WaitForEndOfFrame();
		if (sirenOn)
		{
			for (int j = 0; j < rightSignals.Length; j++)
			{
				rightSignals[j].GetComponent<MeshRenderer>().material.shader = illumin;
				rightSignalsLightSources[j].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				rightSignals[j].GetComponent<MeshRenderer>().material.shader = defaultShader;
				rightSignalsLightSources[j].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
				rightSignals[j].GetComponent<MeshRenderer>().material.shader = illumin;
				rightSignalsLightSources[j].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				rightSignals[j].GetComponent<MeshRenderer>().material.shader = defaultShader;
				rightSignalsLightSources[j].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
				rightSignals[j].GetComponent<MeshRenderer>().material.shader = illumin;
				rightSignalsLightSources[j].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				rightSignals[j].GetComponent<MeshRenderer>().material.shader = defaultShader;
				rightSignalsLightSources[j].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
				rightSignals[j].GetComponent<MeshRenderer>().material.shader = illumin;
				rightSignalsLightSources[j].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				rightSignals[j].GetComponent<MeshRenderer>().material.shader = defaultShader;
				rightSignalsLightSources[j].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
			}
			for (int i = 0; i < leftSignals.Length; i++)
			{
				leftSignals[i].GetComponent<MeshRenderer>().material.shader = illumin;
				leftSignalsLightSources[i].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				leftSignals[i].GetComponent<MeshRenderer>().material.shader = defaultShader;
				leftSignalsLightSources[i].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
				leftSignals[i].GetComponent<MeshRenderer>().material.shader = illumin;
				leftSignalsLightSources[i].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				leftSignals[i].GetComponent<MeshRenderer>().material.shader = defaultShader;
				leftSignalsLightSources[i].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
				leftSignals[i].GetComponent<MeshRenderer>().material.shader = illumin;
				leftSignalsLightSources[i].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				leftSignals[i].GetComponent<MeshRenderer>().material.shader = defaultShader;
				leftSignalsLightSources[i].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
				leftSignals[i].GetComponent<MeshRenderer>().material.shader = illumin;
				leftSignalsLightSources[i].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				leftSignals[i].GetComponent<MeshRenderer>().material.shader = defaultShader;
				leftSignalsLightSources[i].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
			}
		}
		StartCoroutine(SignalLights());
	}

	private IEnumerator MiscLights()
	{
		yield return new WaitForEndOfFrame();
		if (sirenOn)
		{
			for (int i = 0; i < miscLights.Length; i++)
			{
				miscLights[i].GetComponent<MeshRenderer>().material.shader = illumin;
				miscLightSources[i].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				miscLights[i].GetComponent<MeshRenderer>().material.shader = defaultShader;
				miscLightSources[i].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
				miscLights[i].GetComponent<MeshRenderer>().material.shader = illumin;
				miscLightSources[i].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				miscLights[i].GetComponent<MeshRenderer>().material.shader = defaultShader;
				miscLightSources[i].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
				miscLights[i].GetComponent<MeshRenderer>().material.shader = illumin;
				miscLightSources[i].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				miscLights[i].GetComponent<MeshRenderer>().material.shader = defaultShader;
				miscLightSources[i].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
				miscLights[i].GetComponent<MeshRenderer>().material.shader = illumin;
				miscLightSources[i].GetComponent<Light>().enabled = true;
				yield return new WaitForSeconds(0.05f);
				miscLights[i].GetComponent<MeshRenderer>().material.shader = defaultShader;
				miscLightSources[i].GetComponent<Light>().enabled = false;
				yield return new WaitForSeconds(0.05f);
			}
		}
		StartCoroutine(MiscLights());
	}

	private void Update()
	{
		if (!motorScript.canControl)
		{
			return;
		}
		if (motorScript.brakingNow)
		{
			brakeLight.GetComponent<MeshRenderer>().material.shader = illumin;
			brakeLight.GetComponentInChildren<Light>().intensity = Mathf.Lerp(brakeLight.GetComponentInChildren<Light>().intensity, 1f, Time.deltaTime * 10f);
		}
		else
		{
			brakeLight.GetComponent<MeshRenderer>().material.shader = defaultShader;
			brakeLight.GetComponentInChildren<Light>().intensity = Mathf.Lerp(brakeLight.GetComponentInChildren<Light>().intensity, 0f, Time.deltaTime * 10f);
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			if (!headLightsOn)
			{
				headLight.GetComponent<MeshRenderer>().material.shader = illumin;
				headLightLightSource.GetComponent<Light>().enabled = true;
				headLightsOn = true;
			}
			else
			{
				headLight.GetComponent<MeshRenderer>().material.shader = defaultShader;
				headLightLightSource.GetComponent<Light>().enabled = false;
				headLightsOn = false;
			}
		}
		if (Input.GetKeyDown(KeyCode.G))
		{
			if (!sirenOn)
			{
				sirenOn = true;
			}
			else
			{
				sirenOn = false;
			}
		}
	}
}
