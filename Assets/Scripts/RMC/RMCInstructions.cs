using UnityEngine;
using UnityEngine.SceneManagement;

public class RMCInstructions : MonoBehaviour
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
	}

	private void OnGUI()
	{
		GUILayout.BeginArea(new Rect(50f, 50f, 1000f, 100f));
		GUILayout.Label("W, A, S, D for controls, CTRL for lean back, SPACE for rear wheel brake");
		GUILayout.Label("G for siren, and L for headlight");
		GUILayout.Label("Press ''C'' for change camera");
		GUILayout.Label("Press ''R'' for reset scene");
		GUILayout.EndArea();
	}
}
