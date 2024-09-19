using UnityEngine;
using UnityEngine.UI;

public class UIVersionUGUI : MonoBehaviour
{
	public Text text;

	private void Start()
	{
		if ((bool)text)
		{
			text.text += UIVersion.GetMinorVersion();
		}
	}
}
