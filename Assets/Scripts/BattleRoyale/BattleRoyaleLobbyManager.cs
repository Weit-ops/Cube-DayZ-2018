using System.Collections;
using Photon;
using SkyWars;
using UnityEngine;

namespace BattleRoyale
{
	public class BattleRoyaleLobbyManager : Photon.MonoBehaviour
	{
		[SerializeField] tk2dTextMesh _plrCount;
		[SerializeField] GameObject _secondCountDown;
		[SerializeField] GameObject _ui;
		[SerializeField] GameObject _battleUi;
		[SerializeField] GameObject _lobbyObj;
		[SerializeField] tk2dTextMesh[] _playerConnectedLogs;

		public static BattleRoyaleLobbyManager I;

		private string _allPlayerPhrase = "Всего участников: ";
		private string _minText = " (минимум для старта:";
		private bool _enableFirst;

		private void Awake()
		{
			if (I == null)
			{
				I = this;
			}
		}

		private void OnDisable()
		{
			if (_ui != null){
				_ui.SetActive(false);
			}
			if (_battleUi != null) {
				_battleUi.SetActive (DataKeeper.IsBattleRoyaleClick || DataKeeper.IsSkyWarsClick);
			}

			StopCoroutine("ShowCountDown");
		}

		private void Start()
		{
			if (DataKeeper.Language != 0)
			{
				_allPlayerPhrase = "Players in game: ";
				_minText = " (min players:";
			}

			UpdatePlayerCount();

			base.photonView.RPC("FillPlayerConnectedLogs", PhotonTargets.All, JsSpeeker.vk_name, true);
		}

		private void UpdatePlayerCount()
		{
			int maxPlayerOnServer = BattleRoyaleSetupOptions.MaxPlayerOnServer;
			int num = (int)BattleRoyaleSetupOptions.MinStartPercent;
			if (DataKeeper.GameType == GameType.SkyWars)
			{
				maxPlayerOnServer = SkyWarsSetupOptions.MaxPlayerOnServer;
				num = (int)SkyWarsSetupOptions.MinStartPercent;
			}
			if (BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.WaitForFillingRoom || BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.CountdownForStart)
			{
				_plrCount.text = "^CFFFFFFFF" + _allPlayerPhrase + "^C00CC00FF" + PhotonNetwork.room.PlayerCount + "^CFFFFFFFF / ^C00CC00FF" + maxPlayerOnServer + _minText + num + ")";
			}
			else
			{
				_plrCount.text = "^CFFFFFFFF" + _allPlayerPhrase + "^CFF0000FF" + PhotonNetwork.room.PlayerCount + "^CFFFFFFFF / ^CFF0000FF" + maxPlayerOnServer + _minText + num + ")";
			}
		}

		private void Update()
		{
			WaitForMinimumPlayers();

			if (BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.WaitForFillingRoom || BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.CountdownForStart)
			{
				StartCoroutine("ShowCountDown");
			}
		}

		private IEnumerator ShowCountDown()
		{
			if (_enableFirst)
			{
				yield break;
			}
			_enableFirst = true;
			_secondCountDown.SetActive(true);
			tk2dTextMesh tm = _secondCountDown.GetComponent<tk2dTextMesh>();
			while (true)
			{
				string tmp = "Матч начнется через: " + GetTimeForLobby() + " cекунд.";
				if (DataKeeper.Language != 0)
				{
					tmp = "Match starts in: " + GetTimeForLobby() + " seconds.";
				}
				tm.text = tmp;
				yield return new WaitForSeconds(0.3f);
			}
		}

		public int GetTimeForLobby()
		{
			int result = 0;
			if (BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.WaitForFillingRoom)
			{
				result = (int)BattleRoyaleTimeManager.I.GetCurrentTime() + 10;
			}
			if (BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.CountdownForStart)
			{
				result = (int)BattleRoyaleTimeManager.I.GetCurrentTime();
			}
			return result;
		}

		private void OnPhotonPlayerConnected(PhotonPlayer player)
		{
			if (!BattleRoyaleGameManager.I.IsLobby())
			{
				return;
			}

			UpdatePlayerCount();

			if (PhotonNetwork.isMasterClient)
			{
				if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers)
				{
					PhotonNetwork.room.IsOpen = false;
				}
				WaitForMinimumPlayers();
			}
		}

		private void OnPhotonPlayerDisconnected(PhotonPlayer player)
		{
			if (BattleRoyaleGameManager.I.IsLobby())
			{
				UpdatePlayerCount();
				Debug.Log("player Disconnected :(" + player);
				FillPlayerConnectedLogs(player.NickName, false);
			}
		}

		public void WaitForMinimumPlayers()
		{
			if (PhotonNetwork.room != null)
			{
				int playerCount = PhotonNetwork.room.PlayerCount;
				float minStartPercent = BattleRoyaleSetupOptions.MinStartPercent;
				int waitInLobbyForFillRoomToMax = BattleRoyaleSetupOptions.WaitInLobbyForFillRoomToMax;
				if (DataKeeper.GameType == GameType.SkyWars)
				{
					minStartPercent = SkyWarsSetupOptions.MinStartPercent;
					waitInLobbyForFillRoomToMax = SkyWarsSetupOptions.WaitInLobbyForFillRoomToMax;
				}
				if ((float)playerCount >= minStartPercent && BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.Waiting)
				{
					_plrCount.color = Color.green;
					BattleRoyaleTimeManager.I.CallStartTimer(waitInLobbyForFillRoomToMax, TimerCurrentTask.WaitForFillingRoom);
				}
			}
		}

		public int GetMinimumPlayers(int MaxPlayer, float minPercent)
		{
			return (int)minPercent;
		}

		public void StartCountdownForStartGame()
		{
			if (PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.room.IsOpen = false;
				PhotonNetwork.room.Name += "_RUNNING_";
				int startingCountdown = BattleRoyaleSetupOptions.StartingCountdown;

				if (DataKeeper.GameType == GameType.SkyWars)
				{
					startingCountdown = SkyWarsSetupOptions.StartingCountdown;
				}

				BattleRoyaleTimeManager.I.CallStartTimer(startingCountdown, TimerCurrentTask.CountdownForStart);
				CalculateSpawnPoints();
			}
		}

		public void DisableLobbyManager()
		{
			base.enabled = false;
			if (_lobbyObj != null)
			{
				_lobbyObj.SetActive(false);
			}
		}

		[PunRPC]
		private void FillPlayerConnectedLogs(string plrName, bool isConnected = true)
		{
			string empty = string.Empty;

			if (isConnected)
			{
				empty = plrName + " - подключился к игре";
				if (DataKeeper.Language != 0)
				{
					empty = plrName + " - enter the game";
				}
			}else{
				empty = WorldController.I.WorldPlayers[plrName].Name + " - испугался и вышел";
				if (DataKeeper.Language != 0)
				{
					empty = WorldController.I.WorldPlayers[plrName].Name + " - disconnected";
				}
			}

			_playerConnectedLogs[3].text = _playerConnectedLogs[2].text;
			_playerConnectedLogs[2].text = _playerConnectedLogs[1].text;
			_playerConnectedLogs[1].text = _playerConnectedLogs[0].text;
			_playerConnectedLogs[0].text = empty;
		}

		public void CalculateSpawnPoints()
		{
			if (PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.room.IsOpen = false;
				PhotonPlayer[] playerList = PhotonNetwork.playerList;

				for (int i = 0; i < playerList.Length; i++)
				{
					base.photonView.RPC("SetupSpawnPoint", playerList[i], (byte)i);
				}
			}
		}

		[PunRPC]
		public void SetupSpawnPoint(byte point)
		{
			BattleRoyaleGameManager.I.MySkyWarsSpawnPoint = point;
		}
	}
}