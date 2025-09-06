using UnityEngine;

public class FullScreenController : MonoBehaviour
{
	[SerializeField] bool _enableFullScreenOnStart;

	private void Start()
	{
		
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.F12))
		{
			if (ChatGui.I != null && !ChatGui.I.chatIsFocused)
			{
				ScreenUtils.FullScreen(!Screen.fullScreen);
			}
			if (JsSpeeker.I.ReskinType != 0)
			{
				ScreenUtils.FullScreen(!Screen.fullScreen);
			}
		}
	}
}
