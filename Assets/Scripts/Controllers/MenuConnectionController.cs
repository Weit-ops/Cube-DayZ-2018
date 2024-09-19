using System;
using System.Collections;
using System.Collections.Generic;
using BattleRoyale;
using ExitGames.Client.Photon;
using SkyWars;
using UnityEngine;

public class MenuConnectionController : MonoBehaviour
{
	private const float TryReconnectCooldown = 3f;

	public static MenuConnectionController I;

	private bool _withZombies;
	private bool _withFriends;
	private bool _isRandomRoom;
	private List<RoomInfo> _roomInfos = new List<RoomInfo>();
	private bool _disconnectBySelf;
	[HideInInspector]
	public int room_list_updates;
	public string CurrentSessionKey = string.Empty;
	public string KeyFromServer = string.Empty;

	public bool CanPlayNetworkGame
	{
		get
		{
			return PhotonNetwork.connected;
		}
	}

	public List<RoomInfo> RoomInfos
	{
		get
		{
			return _roomInfos;
		}
	}

	private void Awake()
	{
		I = this;
		ConnectToPhoton();
	}

	private void OnJoinedLobby()
	{
		DataKeeper.OfflineMode = string.IsNullOrEmpty(JsSpeeker.viewer_id);
		if (JsSpeeker.FriendsInfos != null)
		{
			string[] array = new string[JsSpeeker.FriendsInfos.Count];
			for (int i = 0; i < JsSpeeker.FriendsInfos.Count; i++)
			{
				if (JsSpeeker.FriendsInfos[i] != null)
				{
					array[i] = JsSpeeker.FriendsInfos[i].uid;
				}
			}
			PhotonNetwork.FindFriends(array);
		}
		else
		{
			Debug.Log("JsSpeeker.FriendsInfos is Null");
		}
	}

	public void ConnectToPhoton()
	{
		if (!string.IsNullOrEmpty(JsSpeeker.viewer_id))
		{
			DataKeeper.OfflineMode = false;
		}
		if ((!string.IsNullOrEmpty(JsSpeeker.viewer_id) && !PhotonNetwork.connected) || PhotonNetwork.offlineMode)
		{
			PhotonNetwork.offlineMode = false;
			PhotonNetwork.ConnectUsingSettings("UNT" + DataKeeper.BuildVersion);
		}
	}

	private IEnumerator Reconnect()
	{
		yield return new WaitForSeconds(3f);
		PhotonNetwork.ConnectUsingSettings("UNT" + DataKeeper.BuildVersion);
	}

	private void OnDisconnectedFromPhoton()
	{
		DataKeeper.OfflineMode = true;
		Debug.LogError("Disconnect from Photon.");
		if (!_disconnectBySelf)
		{
			StartCoroutine("Reconnect");
		}
	}

	private void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		Debug.LogError("Fail to connect to Photon. Reason: " + cause);
		StartCoroutine("Reconnect");
	}

	private void OnJoinedRoom()
	{
		PhotonNetwork.isMessageQueueRunning = false;
		PhotonNetwork.networkingPeer.loadingLevelAndPausedNetwork = true;


		if (DataKeeper.GameType == GameType.Tutorial)
		{
			PhotonNetwork.LoadLevel("TutorialScene");
			return;
		}

		if (JsSpeeker.I.ReskinType == ReskinGameType.Wasteland)
		{
			PhotonNetwork.LoadLevel("UnturnedGameMap_2Wasteland");
		}
		else if (DataKeeper.GameType == GameType.SkyWars)
		{
			PhotonNetwork.LoadLevel("UnturnedGameMap_3_skywars");
		}
		else
		{
			PhotonNetwork.LoadLevel("UnturnedGameMap_2");
		}
		SetupSessionKey();
	}

	public void StartOfflinePlay()
	{
		_disconnectBySelf = true;
		PhotonNetwork.Disconnect();
		PhotonNetwork.offlineMode = true;
		_withZombies = true;
		CreateRoom(null, true, true, false);
	}

	public void StartSinglePlayer()
	{
		Debug.Log("Offline mode " + DataKeeper.OfflineMode);
		if (!DataKeeper.OfflineMode)
		{
			_withZombies = true;
			CreateRoom(null, true, true, false);
		}
		else
		{
			StartOfflinePlay();
		}
	}

	public void CreateRoom(string roomName, bool withZombies, bool single, bool isPrivate, int maxPlayers = 0)
	{
		_withZombies = withZombies;
		string roomName2 = roomName;
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = (byte)(single ? 1 : 20);
		if (maxPlayers != 0)
		{
			roomOptions.MaxPlayers = (byte)maxPlayers;
		}
		roomOptions.IsVisible = !single;
		roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
		string value = string.Empty;
		if (!single && !isPrivate)
		{
			List<GameRoomInfo> emptyRoomsInfos = RoomsService.Instance.GetEmptyRoomsInfos(RoomInfos);
			if (emptyRoomsInfos != null && emptyRoomsInfos.Count > 0)
			{
				GameRoomInfo gameRoomInfo = emptyRoomsInfos.Find((GameRoomInfo r) => r.Name == roomName);
				if (gameRoomInfo != null)
				{
					roomOptions.MaxPlayers = (byte)(gameRoomInfo.MaxPlayersCount + gameRoomInfo.PlayersReserveCount);
					value = gameRoomInfo.Id;
				}
			}
		}
		if (isPrivate && RoomsService.Instance.MyPrivateRooms != null && RoomsService.Instance.MyPrivateRooms.Count > 0)
		{
			GameRoomInfo gameRoomInfo2 = RoomsService.Instance.MyPrivateRooms.Find((GameRoomInfo r) => r.Name == roomName);
			if (gameRoomInfo2 != null)
			{
				roomOptions.MaxPlayers = (byte)(gameRoomInfo2.MaxPlayersCount + gameRoomInfo2.PlayersReserveCount);
				value = gameRoomInfo2.Id;
			}
		}
		roomOptions.CustomRoomProperties.Add("C0", 1);
		roomOptions.CustomRoomProperties.Add("id", value);
		roomOptions.CustomRoomProperties.Add("single", single);
		roomOptions.CustomRoomProperties.Add("zombies", _withZombies);
		roomOptions.CustomRoomProperties.Add("startTime", PhotonNetwork.time);
		roomOptions.CustomRoomProperties.Add("private", isPrivate);
		roomOptions.CustomRoomProperties.Add("creatorId", JsSpeeker.viewer_id);
		roomOptions.CustomRoomPropertiesForLobby = new string[6] { "zombies", "single", "private", "creatorId", "softcap", "id" };

		if (roomOptions.MaxPlayers > 20)
			roomOptions.MaxPlayers = 20;
		PhotonNetwork.CreateRoom(roomName2, roomOptions, PhotonNetwork.lobby);
	}

	public void CallReconnectBR()
	{
		Debug.Log("Faild to join room battle royale reconnect  " + Time.time);
		BattleRoyaleConnectionController.I.ConnectToBattleRoyaleServer();
	}

	public void CallReconnectSW()
	{
		Debug.Log("Faild to join room sky wars reconnect  " + Time.time);
		SkyWarsConnectionController.I.ConnectToSkyWarsServer();
	}

	private void OnPhotonJoinRoomFailed()
	{
		if (DataKeeper.IsBattleRoyaleClick)
		{
			BattleRoyaleConnectionController.I.ReconnectCount++;
			Invoke("CallReconnectBR", 1.5f);
		}
		else if (DataKeeper.IsSkyWarsClick)
		{
			SkyWarsConnectionController.I.ReconnectCount++;
			Invoke("CallReconnectSW", 1.5f);
		}
		else if (_isRandomRoom)
		{
			JoinRandomRoom();
		}
	}

	public void JoinRandomRoom(bool withZombie, bool withFriends)
	{
		if (DataKeeper.GameType == GameType.BattleRoyale)
		{
			Debug.Log("Battle royale random room");
			return;
		}
		if (DataKeeper.GameType == GameType.SkyWars)
		{
			Debug.Log("SkyWars random room");
			return;
		}
		_withZombies = withZombie;
		_withFriends = withFriends;
		Debug.LogError("Join random room   with friends = " + withFriends + "  with zombies  = " + withZombie);
		if (!_withFriends)
		{
			Debug.Log(" если не с друзьями - стандарная логика рандом джоина фотона (с необходимыми пропами");
			JoinRandomRoom();
			return;
		}
		Debug.Log(" находим друзей, которые сейчас онлайн и в комнатах");
		List<FriendInfo> list = ((PhotonNetwork.Friends == null) ? null : PhotonNetwork.Friends.FindAll((FriendInfo f) => f != null && f.IsOnline && f.IsInRoom));
		if (list != null && list.Count > 0)
		{
			if (_roomInfos.Count > 0)
			{
				List<RoomInfo> availableRooms = _roomInfos.FindAll((RoomInfo r) => r.PlayerCount < r.MaxPlayers && (!r.CustomProperties.ContainsKey("zombies") || _withZombies == (bool)r.CustomProperties["zombies"]) && (!r.CustomProperties.ContainsKey("private") || !(bool)r.CustomProperties["private"]));
				FriendInfo friendInfo = list.Find((FriendInfo friend) => availableRooms.Find((RoomInfo room) => room.Name == friend.Room) != null);
				if (friendInfo != null)
				{
					Debug.Log("нашли - коннектимся к комнате друга");
					JoinRoom(friendInfo.Room, true);
				}
				else
				{
					Debug.Log("не нашли - стандарная логика рандом джоина фотона (с необходимыми пропами)");
					JoinRandomRoom();
				}
			}
			else
			{
				Debug.Log("//если есть друзья в комнатах, но не успел обновиться список комнат в фотоне - пытаемся законнектится к любому из друзей");
				FriendInfo friendInfo2 = list[UnityEngine.Random.Range(0, list.Count)];
				JoinRoom(friendInfo2.Room, true);
			}
		}
		else
		{
			Debug.Log(" //если нет друзей в комнатах - стандарная логика рандом джоина фотона (с необходимыми пропами) ");
			JoinRandomRoom();
		}
	}

	private void OnReceivedRoomListUpdate()
	{
		room_list_updates++;
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		if (roomList != null)
		{
			_roomInfos = new List<RoomInfo>(roomList);
		}
		else
		{
			_roomInfos.Clear();
		}
	}

	private void JoinRandomRoom()
	{
		if (DataKeeper.GameType == GameType.BattleRoyale)
		{
			Debug.Log("Battle royale random room");
			return;
		}
		if (DataKeeper.GameType == GameType.SkyWars)
		{
			Debug.Log("Battle royale random room");
			return;
		}
		Debug.LogError("Join random room! ");
		PhotonNetwork.JoinRandomRoom();
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("zombies", _withZombies);
		hashtable.Add("single", false);
		hashtable.Add("private", false);
		if (!DataKeeper.IsUserDummy && DataKeeper.BackendInfo.user.has_premium)
		{
			PhotonNetwork.JoinRandomRoom(hashtable, 50, MatchmakingMode.FillRoom, PhotonNetwork.lobby, "C0 > 0");
			return;
		}
		string sqlLobbyFilter = "C0 < 30";
		PhotonNetwork.JoinRandomRoom(hashtable, 50, MatchmakingMode.FillRoom, PhotonNetwork.lobby, sqlLobbyFilter);
	}

	public void JoinRoom(string roomName, bool joinRandomIfFail)
	{
		_isRandomRoom = joinRandomIfFail;
		PhotonNetwork.JoinRoom(roomName);
	}

	private void SetupSessionKey()
	{
		CurrentSessionKey = Guid.NewGuid().ToString();
		JsSpeeker.I.VkStorageSet("ses_key", CurrentSessionKey);
	}
}
