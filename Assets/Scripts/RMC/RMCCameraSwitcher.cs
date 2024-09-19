using UnityEngine;

public class RMCCameraSwitcher : MonoBehaviour
{
	public GameObject[] cameras;

	public int actCamera;

	private void OnEnable()
	{
		actCamera = 0;
		cameras[3].GetComponent<Camera>().enabled = true;
	}

	private void OnDisable()
	{
		for (int i = 0; i < 3; i++)
		{
			cameras[i].GetComponent<Camera>().enabled = false;
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.O))
		{
			if (actCamera < cameras.Length - 1)
			{
				cameras[actCamera].GetComponent<Camera>().enabled = false;
				cameras[actCamera + 1].GetComponent<Camera>().enabled = true;
				actCamera++;
			}
			else
			{
				actCamera = 0;
				cameras[cameras.Length - 1].GetComponent<Camera>().enabled = false;
				cameras[0].GetComponent<Camera>().enabled = true;
			}
		}
	}
}
