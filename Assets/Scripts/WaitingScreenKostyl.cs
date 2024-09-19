using UnityEngine;

public class WaitingScreenKostyl : MonoBehaviour
{
	public GameObject go1;

	public GameObject battleroyal;

	public GameObject skywars;

	private void Start()
	{
		ShowScreen();
	}

	private void OnEnable()
	{
		ShowScreen();
	}

	private void ShowScreen()
	{
		if (DataKeeper.GameType == GameType.BattleRoyale)
		{
			if (skywars != null)
			{
				skywars.SetActive(false);
			}
			go1.SetActive(false);
			battleroyal.SetActive(true);
		}
		else if (DataKeeper.GameType == GameType.SkyWars)
		{
			if (skywars != null)
			{
				skywars.SetActive(true);
			}
			go1.SetActive(false);
			battleroyal.SetActive(false);
		}
		else
		{
			battleroyal.SetActive(false);
			go1.SetActive(true);
			if (skywars != null)
			{
				skywars.SetActive(false);
			}
		}
	}
}
