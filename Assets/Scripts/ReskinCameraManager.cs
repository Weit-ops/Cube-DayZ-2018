using UnityEngine;

public class ReskinCameraManager : MonoBehaviour
{
	public Camera MainCamera;
	public Camera HandCamera;
	public CameraQualityController qc;

	public TOD_CameraEffects TodCamEff;
	public TOD_Camera TodCam;

	private void Start()
	{
		if (JsSpeeker.I.ReskinType != 0)
		{
			MainCamera.clearFlags = CameraClearFlags.Skybox;

			Object.Destroy(TodCamEff);
			Object.Destroy(TodCam);
			Object.Destroy(qc);

			HandCamera.renderingPath = RenderingPath.VertexLit;
		}

		MainCamera.clearFlags = CameraClearFlags.Skybox;
	}
}
