using UnityEngine;

public class UIVersion : MonoBehaviour
{
	public static string GetMinorVersion()
	{
		return DataKeeper.BuildVersion + " a023";
	}

	private void Start()
	{
		GetComponent<tk2dTextMesh>().text = GetMinorVersion();
	}

	private void Update()
	{
		CalculateCompasValue();
	}

	private void CalculateCompasValue()
	{
	}
}
