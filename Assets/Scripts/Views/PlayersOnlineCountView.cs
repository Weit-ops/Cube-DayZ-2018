using System.Collections;
using BattleRoyaleFramework;
using UnityEngine;

public class PlayersOnlineCountView : MonoBehaviour
{
	[SerializeField] GameType gameType = GameType.Multiplayer;
	[SerializeField] tk2dTextMesh _label;
	[SerializeField] float _checkTime = 1f;

	[SerializeField]
	private void Awake()
	{
		StartCoroutine("UpdateView");
	}

	private IEnumerator UpdateView()
	{
		while (true)
		{
			if (gameType == GameType.Multiplayer)
			{
				if (DataKeeper.Language == Language.Russian)
				{
					_label.text = "Игроков онлайн: " + PhotonNetwork.countOfPlayers;
				}
				else
				{
					_label.text = "Players online: " + PhotonNetwork.countOfPlayers;
				}
			}
			else if (gameType == GameType.BattleRoyale)
			{
				if (DataKeeper.Language == Language.Russian)
				{
					_label.text = "Голодные игры: " + BRFConnection.GetPlayersCountInGameMode(GameType.BattleRoyale);
				}
				else
				{
					_label.text = "In Battle Royale: " + BRFConnection.GetPlayersCountInGameMode(GameType.BattleRoyale);
				}
			}
			else if (gameType == GameType.SkyWars)
			{
				if (DataKeeper.Language == Language.Russian)
				{
					_label.text = "Sky Wars: " + BRFConnection.GetPlayersCountInGameMode(GameType.SkyWars);
				}
				else
				{
					_label.text = "Sky Wars: " + BRFConnection.GetPlayersCountInGameMode(GameType.BattleRoyale);
				}
			}
			yield return new WaitForSeconds(_checkTime);
		}
	}
}
