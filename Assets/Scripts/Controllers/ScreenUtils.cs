using UnityEngine;

public static class ScreenUtils
{
	private static Vector2 _defaultResolution = new Vector2(Screen.width, Screen.height);

	public static void FullScreen(bool enable)
	{
		int width = ((!enable) ? ((int)_defaultResolution.x) : Screen.resolutions[Screen.resolutions.Length - 1].width);
		int height = ((!enable) ? ((int)_defaultResolution.y) : Screen.resolutions[Screen.resolutions.Length - 1].height);
		Screen.SetResolution(width, height, enable);
	}
}
