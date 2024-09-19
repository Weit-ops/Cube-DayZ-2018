using UnityEngine;

public class MapLoadingIndicator : MonoBehaviour
{
	public tk2dTextMesh text;

	private void Update()
	{
		text.text = AssetBundlesService.I.Progress + "%";
	}
}
