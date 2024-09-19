using BattleRoyaleFramework;
using UnityEngine;

namespace SkyWars
{
	public class SkyWarsConnectionController : MonoBehaviour
	{
		public GameObject Lobby;

		private int _recconnectAttempt = 5;

		[HideInInspector]
		public int ReconnectCount;

		public static SkyWarsConnectionController I;

		private void Awake()
		{
			if (I == null)
			{
				I = this;
			}
		}

		private void OnClickSkyWarsBtn()
		{
			if (!string.IsNullOrEmpty(JsSpeeker.viewer_id) && !string.IsNullOrEmpty(JsSpeeker.vk_name) && PhotonNetwork.connectionStateDetailed == ClientState.JoinedLobby && PhotonNetwork.connected)
			{
				ReconnectCount = 0;
				DataKeeper.IsSkyWarsClick = true;
				Lobby.SetActive (true);
				MenuConnectionViewController.I.HideMainMenu(true);
			}
		}

		public void ConnectToSkyWarsServer()
		{
			MainMenuController.I.HideMainMenu();
			MenuConnectionViewController.I.ShowWaitingScreen(true);
			DataKeeper.GameType = GameType.SkyWars;
			Join();
		}

		private void Join()
		{
			if (ReconnectCount > _recconnectAttempt)
			{
				BRFConnection.CreateRoomForGameMode(GameType.SkyWars, SkyWarsSetupOptions.MaxPlayerOnServer);
			}
			else
			{
				BRFConnection.JoinOrCreateForGameMode(GameType.SkyWars, SkyWarsSetupOptions.MaxPlayerOnServer);
			}
		}
	}
}
