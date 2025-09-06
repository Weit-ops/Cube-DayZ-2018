using UnityEngine;

[AddComponentMenu("Common/Full Screen Option")]
public class FullScreenOption : MonoBehaviour
{
	void Update ()
	{
		if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.F5))
		{
			if (ControlFreak2.CFScreen.fullScreen)
			{
				ControlFreak2.CFScreen.SetResolution(1280, 720, false);
			}
			else
			{
				ControlFreak2.CFScreen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
			}
		}
	}
}