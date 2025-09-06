using UnityEngine;

public class FullScreenController : MonoBehaviour
{
	[SerializeField] bool _enableFullScreenOnStart;

	private void Start()
	{
		
	}

	private void Update()
	{
		if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.P) || ControlFreak2.CF2Input.GetKeyDown(KeyCode.F12))
		{
			if (ChatGui.I != null && !ChatGui.I.chatIsFocused)
			{
				ScreenUtils.FullScreen(!ControlFreak2.CFScreen.fullScreen);
			}
			if (JsSpeeker.I.ReskinType != 0)
			{
				ScreenUtils.FullScreen(!ControlFreak2.CFScreen.fullScreen);
			}
		}
	}
}
