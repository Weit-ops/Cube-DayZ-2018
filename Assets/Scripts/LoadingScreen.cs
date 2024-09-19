using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
	public Text text;

	private void Update()
	{
		if ((bool)text)
		{
			text.text = "LOADING... " + AssetBundlesService.I.Progress + "%";
		}
	}
}
