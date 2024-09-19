using UnityEngine;

public class DisableOnFacebook : MonoBehaviour
{
	private void OnEnable()
	{
		if (Application.absoluteURL.Contains("play_fb") || Application.absoluteURL.Contains("facebook"))
		{
			base.gameObject.SetActive(false);
		}
	}
}
