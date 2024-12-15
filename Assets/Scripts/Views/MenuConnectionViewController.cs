using System.Collections;
using System.Collections.Generic;
using BattleRoyale;
using SkyWars;
using UnityEngine;

public class MenuConnectionViewController : MonoBehaviour
{
	public static MenuConnectionViewController I;

	public MenuConnectionController _connectionController;

	[SerializeField] GameObject _mainMenu;

	public GameObject MainMenuContent;
	public GameObject MainMenuZombiez;

	[SerializeField] GameObject _content;
	[SerializeField] GameObject _waitingScreen;
	[SerializeField] GameObject _failToConnectWindow;
	[SerializeField] GameObject _failToCreateWindow;
	[SerializeField] GameObject _chooseGameTypeMenu;
	[SerializeField] GameObject _singleplayer;
	[SerializeField] GameObject _multiplayer;
	[SerializeField] tk2dUIToggleButton _zombieToggle;
	[SerializeField] tk2dUIToggleButton _friendsToggle;

	[HideInInspector]
	public string _joinRoomName;
	[HideInInspector]
	public bool _isRandomRoom;

	public GameObject InviteFriendsBtn;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	public void HideMainMenu(bool flag)
	{
		if (_mainMenu != null)
		{
			_mainMenu.SetActive(!flag);
		}
	}

	public void ShowWaitingScreen(bool show)
	{
		_content.SetActive(!show);
		_waitingScreen.SetActive(show);
	}

	private void OnClickSingleplayer()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		_chooseGameTypeMenu.SetActive(false);
		_singleplayer.SetActive(true);
	}

	private void OnClickMultiplayer()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		_chooseGameTypeMenu.SetActive(false);
		_multiplayer.SetActive(true);
	}

	private void BackToChooseMenu()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		_singleplayer.SetActive(false);
		_multiplayer.SetActive(false);
		_chooseGameTypeMenu.SetActive(true);
	}

	private void BackToMainMenu()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		_chooseGameTypeMenu.SetActive(false);
		_mainMenu.SetActive(true);
	}

	private void StartTutorial()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		if (AssetBundlesService.I.CheckBundle("map_1"))
		{
			AssetBundlesService.I.DownloadImportantBundle("tutorial_1");
			ShowWaitingScreen(true);
			DataKeeper.GameType = GameType.Tutorial;
			_connectionController.StartOfflinePlay();
		}
		else
		{
			Debug.Log("Downloading...");
		}
	}

	private void ContinueSingleplayer()
	{
		DataKeeper.IsSkyWarsClick = false;
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsNewGameClick = false;
		ShowWaitingScreen(true);
		DataKeeper.GameType = GameType.Single;
		_connectionController.StartSinglePlayer();
	}

	private void StartNewSingleplayer()
	{
		DataKeeper.IsNewGameClick = true;
		DataKeeper.IsSkyWarsClick = false;
		DataKeeper.IsBattleRoyaleClick = false;
		_isRandomRoom = false;

		string key = "SinglePlayerMap";
		string key2 = GameType.Single.ToString();

		if (PlayerPrefs.HasKey(key2))
		{
			PlayerPrefs.DeleteKey(key2);
		}

		if (PlayerPrefs.HasKey(key))
		{
			PlayerPrefs.DeleteKey(key);
		}

		PlayerPrefs.Save();

		ShowWaitingScreen(true);
		DataKeeper.GameType = GameType.Single;
		_connectionController.StartSinglePlayer();
	}

	public void IfEmptyServersList()
	{
		StartCoroutine("IfEmptyServersListC");
	}

	public IEnumerator IfEmptyServersListC()
	{
		yield return new WaitForSeconds(2f);
		RoomInfo[] photonRoomList = PhotonNetwork.GetRoomList();

		if (photonRoomList.Length == 0)
		{
			ConnectToRoom("Survival PvP #1");
		}
	}

	private void Update()
	{
	}

	public void ConnectToRoom(string roomName)
	{
		_joinRoomName = roomName;
		_isRandomRoom = false;
		ShowWaitingScreen(true);
		DataKeeper.GameType = GameType.Multiplayer;
		_connectionController.JoinRoom(roomName, false);
	}

	private void ConnectToRandomRoom()
	{
		DataKeeper.IsSkyWarsClick = false;
		DataKeeper.IsBattleRoyaleClick = false;
		_isRandomRoom = true;
		ShowWaitingScreen(true);
		DataKeeper.GameType = GameType.Multiplayer;
		_connectionController.JoinRandomRoom(_zombieToggle.IsOn, _friendsToggle.IsOn);
	}

	private void OnPhotonJoinRoomFailed()
	{
		if (DataKeeper.IsBattleRoyaleClick || DataKeeper.IsSkyWarsClick || (_isRandomRoom && _friendsToggle.IsOn))
		{
			return;
		}
		List<GameRoomInfo> myPrivateRooms = RoomsService.Instance.MyPrivateRooms;
		if (myPrivateRooms != null)
		{
			GameRoomInfo gameRoomInfo = myPrivateRooms.Find((GameRoomInfo room) => room.Name == _joinRoomName);
			if (gameRoomInfo != null)
			{
				_connectionController.CreateRoom(gameRoomInfo.Name, gameRoomInfo.WithZombies, false, true);
				return;
			}
		}
		List<GameRoomInfo> emptyRoomsInfos = RoomsService.Instance.GetEmptyRoomsInfos(MenuConnectionController.I.RoomInfos);
		if (emptyRoomsInfos != null)
		{
			GameRoomInfo gameRoomInfo2 = emptyRoomsInfos.Find((GameRoomInfo room) => room.Name == _joinRoomName);
			if (gameRoomInfo2 != null)
			{
				_connectionController.CreateRoom(gameRoomInfo2.Name, gameRoomInfo2.WithZombies, false, false);
				return;
			}
		}
		ShowWaitingScreen(false);
		Debug.Log("FAIL TO JOIN ROOM!");
		_failToConnectWindow.SetActive(true);
	}

	private void OnPhotonRandomJoinFailed()
	{
		if (DataKeeper.IsBattleRoyaleClick)
		{
			Debug.Log("Faild to join room battle royale reconnect " + Time.time);
			BattleRoyaleConnectionController.I.ConnectToBattleRoyaleServer();
			return;
		}
		if (DataKeeper.IsSkyWarsClick)
		{
			Debug.Log("Faild to join room skywars " + Time.time);
			SkyWarsConnectionController.I.ConnectToSkyWarsServer();
			return;
		}
		List<GameRoomInfo> emptyRoomsInfos = RoomsService.Instance.GetEmptyRoomsInfos(MenuConnectionController.I.RoomInfos);
		if (emptyRoomsInfos != null)
		{
			GameRoomInfo gameRoomInfo = emptyRoomsInfos.Find((GameRoomInfo room) => room.WithZombies == _zombieToggle.IsOn);
			if (gameRoomInfo != null)
			{
				_connectionController.CreateRoom(gameRoomInfo.Name, gameRoomInfo.WithZombies, false, false);
				return;
			}
		}
		ShowWaitingScreen(false);
		Debug.Log("FAIL TO JOIN RANDOM ROOM!");
		_failToConnectWindow.SetActive(true);
	}

	private void OnPhotonCreateRoomFailed()
	{
		ShowWaitingScreen(false);
		Debug.Log("FAIL TO CREATE ROOM!");
		_failToCreateWindow.SetActive(true);
	}

	public void MobileLogout()
	{
		if (Controller.I != null)
		{
			Controller.I.LogOut();
		}
	}

	public void ServerLogin()
	{
		if (Controller.I != null)
		{
			Controller.I.OnClickGuestBtn();
		}
	}

	
	private void OnEnable()
	{
		if (Controller.I != null && Controller.I.vkapi.isUserLoggedIn)
		{
			InviteFriendsBtn.SetActive(true);
		}
	}
}
