using UnityEngine;

public class CameraQualityController : MonoBehaviour
{
	private void Start()
	{
		Set();
	}

	public void Set()
	{
		if (QualitySettings.GetQualityLevel() == 0)
		{
			GetComponent<Camera>().renderingPath = RenderingPath.VertexLit;
		}
		else
		{
			GetComponent<Camera>().renderingPath = RenderingPath.UsePlayerSettings;
		}
	}
}
