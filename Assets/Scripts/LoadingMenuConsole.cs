using UnityEngine;

public class LoadingMenuConsole : MonoBehaviour
{
	private void OnDisable()
	{
		if (DebugConsoleController.I != null)
		{
			DebugConsoleController.I.ShowConsole(false);
		}
	}
}
