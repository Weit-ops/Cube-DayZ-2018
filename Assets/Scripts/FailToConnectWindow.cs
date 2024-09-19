using UnityEngine;

public class FailToConnectWindow : MonoBehaviour
{
	private void OnClickOk()
	{
		base.gameObject.SetActive(false);
	}
}
