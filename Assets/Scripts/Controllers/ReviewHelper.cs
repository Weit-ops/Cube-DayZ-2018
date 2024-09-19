using UnityEngine;

public class ReviewHelper : MonoBehaviour
{
	private const string prefs_is_application_rated = "prefs_is_application_rated";

	public static bool IsApplcationRated
	{
		get
		{
			return PlayerPrefs.GetInt("prefs_is_application_rated", 0) != 0;
		}
		set
		{
			PlayerPrefs.SetInt("prefs_is_application_rated", value ? 1 : 0);
		}
	}

	public static void RateApplication()
	{
		Debug.LogError("Not implemented");
	}

	private static void runRateApplication(string rateURL)
	{
		Debug.Log("Rate URL:" + rateURL);
		Application.OpenURL(rateURL);
	}
}
