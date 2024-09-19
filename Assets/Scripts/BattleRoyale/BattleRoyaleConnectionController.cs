using BattleRoyaleFramework;
using UnityEngine;

namespace BattleRoyale
{
	public enum BattleRoyalMode
	{
		Simple = 0,
		Team2 = 1,
		Team5 = 2
	}

	public class BattleRoyaleConnectionController : MonoBehaviour
	{
		private int _recconnectAttempt = 5;

		private BattleRoyalMode _battleRoyalMode;

		public static BattleRoyaleConnectionController I;

		[HideInInspector]
		public int ReconnectCount;

		private void Awake()
		{
			if (I == null)
			{
				I = this;
			}
		}

		private void OnClickBattleRoyaleBtn()
		{
			if (!string.IsNullOrEmpty(JsSpeeker.viewer_id) && !string.IsNullOrEmpty(JsSpeeker.vk_name))
			{
				ReconnectCount = 0;
				DataKeeper.IsBattleRoyaleClick = true;
				ConnectToBattleRoyaleServer();
			}
		}

		public void ConnectToBattleRoyaleServer()
		{
			MainMenuController.I.HideMainMenu();
			MenuConnectionViewController.I.ShowWaitingScreen(true);
			DataKeeper.GameType = GameType.BattleRoyale;
			BattleRoyalJoin();
		}

		private void BattleRoyalJoin()
		{
			if (ReconnectCount > _recconnectAttempt)
			{
				BRFConnection.CreateRoomForGameMode(GameType.BattleRoyale, BattleRoyaleSetupOptions.MaxPlayerOnServer);
			}
			else
			{
				BRFConnection.JoinOrCreateForGameMode(GameType.BattleRoyale, BattleRoyaleSetupOptions.MaxPlayerOnServer);
			}
		}
	}
}
