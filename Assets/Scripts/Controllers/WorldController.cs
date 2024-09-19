using System.Collections;
using System.Collections.Generic;
using CodeStage.AdvancedFPSCounter;
using CodeStage.AntiCheat.ObscuredTypes;
using ExitGames.Client.Photon;
using JsonFx.Json;
using Photon;
using System.IO;
using UnityEngine;

public class StatisticsInfo
{
	public int ZombieKills;
	public int CreatureKills;
	public int PlayerKills;
	public int CraftItems;
	public int PlantsPlanted;
	public int Die;
	public int Suicide;
}

public class WorldInfo
{
	public string Id;
	public string Name;
	public List<WorldObject> Objects;
	public List<ObjectsSpawn> Spawns;
}

public class WorldObject
{
	public string ObjectId;
	public string Position;
	public string Rotation;
	public string AdditionInfo;
	public string OwnerId;
}

public static class StartLootManager
{
	private static int StartLootMinItem = 2;
	private static int StartLootMaxItem = 3;

	private static List<KeyValueInt> StartLootItems = new List<KeyValueInt>
	{
		new KeyValueInt
		{
			Key = "hammer",
			Value = 5
		},
		new KeyValueInt
		{
			Key = "Torch",
			Value = 5
		},
		new KeyValueInt
		{
			Key = "tatteredGreenPants",
			Value = 10
		},
		new KeyValueInt
		{
			Key = "tatteredBluePants",
			Value = 10
		},
		new KeyValueInt
		{
			Key = "tatteredRedPants",
			Value = 10
		},
		new KeyValueInt
		{
			Key = "Moldy Rat",
			Value = 10
		},
		new KeyValueInt
		{
			Key = "Raw Rat",
			Value = 10
		},
		new KeyValueInt
		{
			Key = "Moldy Milk",
			Value = 5
		},
		new KeyValueInt
		{
			Key = "Moldy Bottled Water",
			Value = 10
		},
		new KeyValueInt
		{
			Key = "Rag",
			Value = 10
		},
		new KeyValueInt
		{
			Key = "Vaccine",
			Value = 5
		},
		new KeyValueInt
		{
			Key = "Vitamins",
			Value = 10
		}
	};

	public static void TestAll(ContentInfo content)
	{
		foreach (KeyValueInt startLootItem in StartLootItems)
		{
			Item itemInfo = content.GetItemInfo(startLootItem.Key);
			if (itemInfo == null)
			{
				Debug.Log("START LOOT ITEM BUG: " + startLootItem.Key);
			}
		}
	}

	public static List<Item> GetStartItems(ContentInfo content)
	{
		List<Item> list = new List<Item>();
		int num = 0;
		bool flag = true;
		bool flag2 = true;
		foreach (KeyValueInt startLootItem in StartLootItems)
		{
			num += startLootItem.Value;
		}
		int num2 = UnityEngine.Random.Range(StartLootMinItem, StartLootMaxItem + 1);
		while (num2 > 0)
		{
			int itemIndex = GetItemIndex(num);
			if (itemIndex < 0)
			{
				break;
			}
			Item itemInfo = content.GetItemInfo(StartLootItems[itemIndex].Key);
			if (itemInfo == null)
			{
				continue;
			}
			switch (itemInfo.Type)
			{
			case ItemType.Weapon:
				if (flag)
				{
					list.Add(itemInfo);
					num2--;
					flag = false;
				}
				break;
			case ItemType.Clothing:
				if (flag2)
				{
					list.Add(itemInfo);
					num2--;
					flag2 = false;
				}
				break;
			default:
				list.Add(itemInfo);
				num2--;
				break;
			}
		}
		return list;
	}

	private static int GetItemIndex(int chanceSum)
	{
		if (StartLootItems.Count == 0)
		{
			return -1;
		}
		int result = StartLootItems.Count - 1;
		int num = UnityEngine.Random.Range(0, chanceSum);
		int num2 = 0;
		for (int i = 0; i < StartLootItems.Count; i++)
		{
			num2 += StartLootItems[i].Value;
			if (num < num2)
			{
				return i;
			}
		}
		return result;
	}
}


public static class ZombieBalanceService
{
	public static int SimpleZombieMaxCount = 225;
	public static int InfectiveZombieMaxCount = 25;
	public static int MegaZombieMaxCount = 1;
	public static float DayZombieSpeedFactor = 0.6f;
	public static float DayZombieAttackColdownFactor = 2f;
	public static float DayZombieCountFactor = 0.5f;
	public static int MaxZombieOnMap = 250;
	public static int MaxMobCountInSector = 10;
	public static int ZombiePerPlayerCount = 5;

	public static int CallculateSectorZombiesLimit(bool isDayTime, int zombiesOnMap, int availableZombiesInSector, int playersInRoom)
	{
		float num = ((!isDayTime) ? 1f : DayZombieCountFactor);
		int num2 = (int)((float)MaxZombieOnMap * num) - zombiesOnMap - Mathf.Max(0, ZombiePerPlayerCount * (playersInRoom - 1));
		if (num2 > 0)
		{
			int num3 = Mathf.Min(availableZombiesInSector, num2);
			if (num3 > 0)
			{
				return (int)Mathf.Min((float)num3 * num, MaxMobCountInSector);
			}
			return 0;
		}
		return 0;
	}
}

public class WorldController : Photon.MonoBehaviour
{
	public const string ClothingPath = "Clothings/";
	public const string WeaponPath = "Weapons/";
	public const string ConsumablePath = "Consumables/";
	public const string WorldObjectsPath = "WorldObjects/";
	public const string MobsPath = "Mobs/";
	public const string PhotonItemPrefab = "PhotonItem";
	public const string LocalItemPrefab = "LocalItem";
	private const float TryReconnectCooldown = 3f;
	private const int TryReconnectMaxCount = 20;

	public static WorldController I;

	[SerializeField] bool _enableTestSpawns;
	[SerializeField] float _testSpawnArea;
	[SerializeField] int _testSpawnObjCount;
	[SerializeField] LayerMask _testSpawnLayerMask;
	[SerializeField] TestSpawns _testSpawns;
	[SerializeField] Transform _items;
	[SerializeField] Transform _mobs;
	[SerializeField] Transform _destructibleObjects;
	[SerializeField] LayerMask _mobsSpawnLayerMask;

	public GameObject WaitingBack;

	private string _mapPath;

	[HideInInspector]
	public List<PhotonWorldObject> WorldObjects = new List<PhotonWorldObject>();
	[HideInInspector]
	public List<PhotonDropObject> WorldItems = new List<PhotonDropObject>();
	[HideInInspector]
	public List<PhotonMob> WorldMobs = new List<PhotonMob>();
	[HideInInspector]
	public List<LocalDropObject> LocalDropObjects = new List<LocalDropObject>();
	[HideInInspector]
	public Dictionary<string, PhotonMan> WorldPlayers = new Dictionary<string, PhotonMan>();
	[HideInInspector]
	public ObscuredString MyId;

	[SerializeField] GameObject _defaultItemPrefab;
	[SerializeField] GameObject _defaultDestructObjPrefab;
	[SerializeField] GameObject _defaultMobPrefab;
	[SerializeField] GameObject _localDestructedObjects;
	[SerializeField] PhotonObjectsManager _objectsManager;
	[SerializeField] TOD_Sky _sky;
	[SerializeField] TOD_Time _time;
	[SerializeField] tk2dTextMesh _reconnectingLabel;
	[SerializeField] tk2dTextMesh _reconnectToNewRoomLabel;
	[SerializeField] WeatherSystem _weather;

	private bool _localObjSpawed;
	private int _reconnectTryCount;
	private bool _firstSpawn = true;
	private string _myRoom;
	private bool _myRoomWithZombie;
	private Vector3 _myLastPosition = Vector3.zero;
	private WorldInfo _worldInfo;
	public PlayerSaveHelper PlayerSaveHelper;
	public bool _multiplayerWorldLoaded;
	public bool CanEnterTheCar;
	public static bool IsDieNotCorrect;
	public ContentInfo Info { get; private set; }
	public bool IsDone { get; private set; }
	public StatisticsInfo PlayerStatistic { get; private set; }

	public GameObject DefaultMobPrefab
	{
		get
		{
			return _defaultMobPrefab;
		}
	}

	public GameObject DefaultDestructObjPrefab
	{
		get
		{
			return _defaultDestructObjPrefab;
		}
	}

	public GameObject DefaultItemPrefab
	{
		get
		{
			return _defaultItemPrefab;
		}
	}

	public Transform Items
	{
		get
		{
			return _items;
		}
	}

	public Transform Mobs
	{
		get
		{
			return _mobs;
		}
	}

	public Transform DestructibleObjects
	{
		get
		{
			return _destructibleObjects;
		}
	}

	public PhotonObjectsManager ObjectsManager
	{
		get
		{
			return _objectsManager;
		}
	}

	public int SuccessReconnectionCount { get; private set; }
	public int ReconnectionTryCount { get; private set; }
	public PhotonManInfo MyInfo { get; private set; }
	public bool NeedRestoreInfo { get; set; }
	public bool LeaveRoomBySelf { get; set; }
	public int ActiveMobCount { get; set; }

	public bool MultiplayerWorldLoaded
	{
		get
		{
			return _multiplayerWorldLoaded;
		}
	}

	public bool IsDay
	{
		get
		{
			if (JsSpeeker.I.ReskinType == ReskinGameType.Default)
			{
				return _sky.IsDay;
			}
			return true;
		}
	}

	public PhotonMan Player;

	public IEnumerator DisableWeather()
	{
		yield return new WaitForSeconds(5f);
		if ((DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars) && JsSpeeker.I.ReskinType == ReskinGameType.Default)
		{
			_time.DayLengthInMinutes = 2400f;
			_sky.Cycle.Hour = 17f;
		}
	}

	private void Awake()
	{
		if (PlayerPrefs.HasKey("QuailityLvl"))
		{
			QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QuailityLvl"));
		}

		StartCoroutine("DisableWeather");
		I = this;
		DataKeeper.PlayerGameInfoBackup = new PlayerInfoBackup();

		if (PhotonNetwork.room != null)
		{
			if (PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
				{
					"startTime",
					PhotonNetwork.time
				} });
			}
		}
		else
		{
			Debug.LogError("room is null");
		}

		PlayerStatistic = new StatisticsInfo();
		LoadContent();
		PlayerSaveHelper = new PlayerSaveHelper();
	}

	private void Start()
	{
		OnJoinedRoom();
	}

	public bool AddItemsFromPremium()
	{
		if (!DataKeeper.IsUserDummy && DataKeeper.BackendInfo.user.has_premium)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add("Knapsack", 1);
			dictionary.Add("axe", 1);
			dictionary.Add("pickaxe", 1);
			dictionary.Add("Bandage", 3);
			dictionary.Add("Canned Soup", 3);
			dictionary.Add("Torch", 1);
			Dictionary<string, int> dictionary2 = dictionary;
			PremiumClothingPack premiumClothingPack = CustomizationRegistry.Instance.PremiumClothings[DataKeeper.PremiumPackIndex];
			dictionary2.Add(premiumClothingPack.BodyClothingId, 1);
			dictionary2.Add(premiumClothingPack.LegsClothingId, 1);
			foreach (KeyValuePair<string, int> item in dictionary2)
			{
				Item itemInfo = Info.GetItemInfo(item.Key);
				if (itemInfo != null)
				{
					if (itemInfo.Type == ItemType.Consumables)
					{
						InventoryController.Instance.AddItems(itemInfo, item.Value);
					}
					else
					{
						InventoryController.Instance.EquipFromPack(itemInfo, 0);
					}
				}
			}
			return true;
		}
		return false;
	}

	public void AddItemsFromPack()
	{
		int packId = ((!PlayerPrefs.HasKey("SpecialPack")) ? (-1) : PlayerPrefs.GetInt("SpecialPack"));
		SpecialPack specialPack = SpecialPacksRegistry.SpecialPacks.Find((SpecialPack pack) => pack.Id == packId);
		if (specialPack != null)
		{
			foreach (KeyValueInt item in specialPack.Items)
			{
				Item itemInfo = Info.GetItemInfo(item.Key);
				if (itemInfo != null)
				{
					if (itemInfo.Type == ItemType.Consumables)
					{
						InventoryController.Instance.AddItems(itemInfo, item.Value);
					}
					else
					{
						InventoryController.Instance.EquipFromPack(itemInfo, 0);
					}
				}
			}
			SavePlayer(false);
		}
		else if (!AddItemsFromPremium())
		{
			StartLootManager.TestAll(Info);
			List<Item> startItems = StartLootManager.GetStartItems(Info);
			foreach (Item item2 in startItems)
			{
				if (item2 != null)
				{
					if (item2.Type == ItemType.Consumables)
					{
						InventoryController.Instance.AddItems(item2, 1);
					}
					else
					{
						InventoryController.Instance.EquipFromPack(item2, 0);
					}
				}
			}
		}
		if (packId >= 0)
		{
			PlayerPrefs.SetInt("SpecialPack", -1);
			PlayerPrefs.Save();
		}
	}

	private void OnJoinedLobby()
	{
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
		if (string.IsNullOrEmpty(_myRoom))
		{
			Debug.LogError("ahctung!");
			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("zombies", _myRoomWithZombie);
			hashtable.Add("single", false);
			hashtable.Add("private", false);
			if (!DataKeeper.IsUserDummy && DataKeeper.BackendInfo.user.has_premium)
			{
				Debug.LogError("Regular random join 333");
				PhotonNetwork.JoinRandomRoom(hashtable, 50, MatchmakingMode.FillRoom, PhotonNetwork.lobby, "C0 > 0");
			}
			else
			{
				string sqlLobbyFilter = "C0 < 30";
				Debug.LogError("Regular random join 444");
				PhotonNetwork.JoinRandomRoom(hashtable, 50, MatchmakingMode.FillRoom, PhotonNetwork.lobby, sqlLobbyFilter);
			}
		}
		else
		{
			PhotonNetwork.JoinRoom(_myRoom);
		}
	}

	private void OnPhotonRandomJoinFailed()
	{
		string empty = string.Empty;
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = 20;
		roomOptions.IsVisible = true;
		roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
		string value = string.Empty;
		List<RoomInfo> photonRooms = new List<RoomInfo>(PhotonNetwork.GetRoomList());
		List<GameRoomInfo> emptyRoomsInfos = RoomsService.Instance.GetEmptyRoomsInfos(photonRooms);
		if (emptyRoomsInfos != null && emptyRoomsInfos.Count > 0)
		{
			GameRoomInfo gameRoomInfo = emptyRoomsInfos[UnityEngine.Random.Range(0, emptyRoomsInfos.Count)];
			if (gameRoomInfo != null)
			{
				roomOptions.MaxPlayers = (byte)(gameRoomInfo.MaxPlayersCount + gameRoomInfo.PlayersReserveCount);
				value = gameRoomInfo.Id;
				empty = gameRoomInfo.Name;
			}
		}
		if (!string.IsNullOrEmpty(empty))
		{
			roomOptions.CustomRoomProperties.Add("C0", 1);
			roomOptions.CustomRoomProperties.Add("id", value);
			roomOptions.CustomRoomProperties.Add("single", false);
			roomOptions.CustomRoomProperties.Add("zombies", _myRoomWithZombie);
			roomOptions.CustomRoomProperties.Add("startTime", PhotonNetwork.time);
			roomOptions.CustomRoomProperties.Add("private", false);
			roomOptions.CustomRoomProperties.Add("creatorId", JsSpeeker.viewer_id);
			roomOptions.CustomRoomPropertiesForLobby = new string[6] { "zombies", "single", "private", "creatorId", "softcap", "id" };
			PhotonNetwork.lobby = new TypedLobby("blabla", LobbyType.Default);
			PhotonNetwork.CreateRoom(empty, roomOptions, PhotonNetwork.lobby);
		}
		else
		{
			Debug.Log("No room available for join!");
			PhotonNetwork.LoadLevel("MainMenu");
		}
	}

	private void OnPhotonJoinRoomFailed()
	{
		ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
		hashtable.Add("zombies", _myRoomWithZombie);
		hashtable.Add("single", false);
		hashtable.Add("private", false);
		if (!DataKeeper.IsUserDummy && DataKeeper.BackendInfo.user.has_premium)
		{
			Debug.LogError("Regular random join 444");
			PhotonNetwork.JoinRandomRoom(hashtable, 50, MatchmakingMode.FillRoom, PhotonNetwork.lobby, "C0 > 0");
		}
		else
		{
			string sqlLobbyFilter = "C0 < 30";
			Debug.LogError("Regular random join 555");
			PhotonNetwork.JoinRandomRoom(hashtable, 50, MatchmakingMode.FillRoom, PhotonNetwork.lobby, sqlLobbyFilter);
		}
	}

	public void AddItemsFromLoad()
	{
		string[] equipedItems = PlayerSaveHelper.GetEquipedItems();
		if (equipedItems != null)
		{
			string[] array = equipedItems;
			foreach (string text in array)
			{
				if (text != null)
				{
					Item itemInfo = Info.GetItemInfo(text);
					if (itemInfo != null)
					{
						InventoryController.Instance.EquipFromPack(itemInfo, PlayerSaveHelper.GetAmmoForWeaponInHand());
					}
				}
			}
		}
		Dictionary<byte, InventorySlot> inventoryInfo = PlayerSaveHelper.GetInventoryInfo();
		if (inventoryInfo != null)
		{
			foreach (KeyValuePair<byte, InventorySlot> item in inventoryInfo)
			{
				if (InventoryController.Instance.Slots.Count > item.Key && item.Value.Item != null && (int)item.Value.Count > 0)
				{
					InventoryController.Instance.Slots[item.Key].AddSome(item.Value.Item, item.Value.Count, item.Value.Ammo);
				}
			}
		}
		PlayerSaveHelper.ClearInventoryInfo();
	}

	private void TrySyncTime()
	{
		if (PhotonNetwork.room != null)
		{
			if (PhotonNetwork.room.CustomProperties.ContainsKey("startTime"))
			{
				double num = (double)PhotonNetwork.room.CustomProperties["startTime"];
				if (JsSpeeker.I.ReskinType == ReskinGameType.Default)
				{
					_time.SyncTime((float)(PhotonNetwork.time - num));
				}
			}
			else
			{
				Debug.Log("Time not sync!");
			}
		}
		else
		{
			Debug.LogError("room is null");
		}
	}

	private IEnumerator ShowReconnectToNewRoomMessage()
	{
		Debug.Log("Join room after recconect failed. Create new room!");
		_reconnectToNewRoomLabel.gameObject.SetActive(true);
		yield return new WaitForSeconds(5f);
		_reconnectToNewRoomLabel.gameObject.SetActive(false);
	}

	private void OnJoinedRoom()
	{
		PhotonNetwork.isMessageQueueRunning = true;

		if (PlayerPrefs.HasKey("Die"))
		{
			IsDieNotCorrect = PlayerPrefs.GetString("Die") == "true";
		}

		if (!string.IsNullOrEmpty(_myRoom))
		{
			SuccessReconnectionCount++;
		}

		if (!string.IsNullOrEmpty(_myRoom) && _myRoom != PhotonNetwork.room.Name)
		{
			StartCoroutine("ShowReconnectToNewRoomMessage");
		}

		_reconnectingLabel.gameObject.SetActive(false);
		_myRoom = PhotonNetwork.room.Name;
		_myRoomWithZombie = false;

		if (PhotonNetwork.room.CustomProperties.ContainsKey("zombies"))
		{
			_myRoomWithZombie = (bool)PhotonNetwork.room.CustomProperties["zombies"];
		}

		_reconnectTryCount = 0;

		if (PhotonNetwork.isMasterClient)
		{
			if (_firstSpawn)
			{
				CreateWorld();
			}
			StartCoroutine("AddMobsForPulling");
		}
		if (DataKeeper.GameType == GameType.Multiplayer && !_multiplayerWorldLoaded)
		{
			PHPNetwork.I.LoadWorldMultiplayer (PhotonNetwork.room.Name,LoadWorldInMultiplayer);
		}
		if (DataKeeper.GameType == GameType.Multiplayer) {
			PHPNetwork.I.LoadPlayerProgressMultiplayer ((string)PhotonNetwork.room.CustomProperties["id"],LoadPlayerData);
		}
		TrySyncTime();

		if (string.IsNullOrEmpty(MyId))
		{
			MyId = JsSpeeker.viewer_id;
		}

		if (string.IsNullOrEmpty(MyId))
		{
			Debug.LogError("login error? Попробуйте другой браузер или отключите адблок!");
			if (DataKeeper.GameType == GameType.Single)
			{
				MyId = "%userId%";
			}
		}

		Player = PhotonNetwork.Instantiate("Player", _myLastPosition, Quaternion.identity, 0, new object[5]
		{
			JsSpeeker.vk_name + VipName(),
			(byte)DataKeeper.Sex,
			DataKeeper.SkinColorIndex,
			DataKeeper.FaceIndex,
				(byte)DataKeeper.VipRaincoatIndex
		}).GetComponent<PhotonMan>();
		
		if (!_localObjSpawed)
		{
			_worldInfo = LoadWorldInfo();
			InstantiateLocalItems();
		}

		if (_firstSpawn)
		{
			Vector3? playerPosition = PlayerSaveHelper.GetPlayerPosition();
			PlayerSaveHelper.ClearPosition();
			PhotonSectorService.I.InitializeSpawnInfo(GetMobsSpawnInfo());

			if (playerPosition.HasValue && !IsDieNotCorrect)
			{
				PlayerSpawnsController.I.Respawn(_firstSpawn, playerPosition.Value);
			}
			else
			{
				PlayerSpawnsController.I.Respawn(_firstSpawn);
			}

			_firstSpawn = false;
		}
		StartCoroutine("AutoSaveProcess");
	}


	public byte[] SavePlayerData()
	{
		MemoryStream ms = new MemoryStream ();
		BinaryWriter bw = new BinaryWriter (ms);
		bw.Write ((short)Player.ManInfo.HealthPoints);
		bw.Write ((int)Player.CurrentHunger);
		bw.Write ((int)Player.CurrentSickness);
		bw.Write ((int)Player.CurrentThirst);
		bw.Write ((float)GameControls.I.Player.transform.position.x);
		bw.Write ((float)GameControls.I.Player.transform.position.y);
		bw.Write ((float)GameControls.I.Player.transform.position.z);
		bw.Write ((float)GameControls.I.Player.transform.rotation.x);
		bw.Write ((float)GameControls.I.Player.transform.rotation.y);
		bw.Write ((float)GameControls.I.Player.transform.rotation.z);
		bw.Write ((float)GameControls.I.Player.transform.rotation.w);
		byte[] array = ms.ToArray ();
		array = Ionic.Zlib.ZlibStream.CompressBuffer (array);
		return array;
	}

	public void LoadPlayerData(string callbackResponse)
	{
		if (callbackResponse == "empty-player" || callbackResponse == string.Empty) {
			return;
		}
		byte[] data = System.Convert.FromBase64String (callbackResponse);
		data = Ionic.Zlib.ZlibStream.UncompressBuffer (data);
		MemoryStream ms = new MemoryStream (data);
		BinaryReader br = new BinaryReader (ms);
		short hp = br.ReadInt16();
		int hunger = br.ReadInt32 ();
		int sickness = br.ReadInt32 ();
		int thirst = br.ReadInt32 ();
		GameControls.I.Player.hitPoints = (float)hp;
		Player.ManInfo.HealthPoints = hp;
		Player.CurrentHunger = hunger;
		Player.CurrentSickness = sickness;
		Player.CurrentThirst = thirst;
		StatesView.I.UpdateState (ManState.HealthPoint,hp);
		StatesView.I.UpdateState (ManState.Hunger,hunger);
		StatesView.I.UpdateState (ManState.Sickness, sickness);
		StatesView.I.UpdateState (ManState.Thirst, thirst);
		float xp = br.ReadSingle ();
		float yp = br.ReadSingle ();
		float zp = br.ReadSingle ();
		GameControls.I.Player.transform.position = new Vector3 (xp, yp, zp);
		float xr = br.ReadSingle ();
		float yr = br.ReadSingle ();
		float zr = br.ReadSingle ();
		float wr = br.ReadSingle ();
		GameControls.I.Player.transform.rotation = new Quaternion (xr,yr,zr,wr);
	}	

	public string VipName()
	{
		string result = string.Empty;
		if (!DataKeeper.IsUserDummy && DataKeeper.BackendInfo.user.has_vip)
		{
			result = "^CFFC300FF VIP";
		}
		return result;
	}

	private IEnumerator Reconnect()
	{
		_reconnectingLabel.gameObject.SetActive(true);
		yield return new WaitForSeconds(3f);
		ReconnectionTryCount++;
		_reconnectTryCount++;
		PhotonNetwork.lobby = new TypedLobby("softcap", LobbyType.SqlLobby);
		PhotonNetwork.ConnectUsingSettings("UNT" + DataKeeper.BuildVersion);
	}

	private void OnDisconnectedFromPhoton()
	{
		StopCoroutine("AddMobsForPulling");
		StopCoroutine("AutoSaveMultiplayerWorld");

		if (IsTester())
		{
			Debug.Log("autosaveworld cor STOPPED " + Time.time);
		}

		MobPullingSystem.Instance.ClearAllInfo();
		StartCoroutine("AutoSaveProcess");
		SavePlayer(false);

		if (DataKeeper.GameType == GameType.Multiplayer)
		{
			if (NeedRestoreInfo)
			{
				return;
			}
			_myLastPosition = GameControls.I.Player.transform.position;
			MyInfo = new PhotonManInfo
			{
				HealthPoints = Player.ManInfo.HealthPoints,
				Equipment = new Dictionary<string, string>()
			};
			foreach (KeyValuePair<string, string> item in Player.ManInfo.Equipment)
			{
				MyInfo.Equipment.Add(item.Key, item.Value);
			}
			NeedRestoreInfo = true;
			StartCoroutine("Reconnect");
		}
		else
		{
			Debug.Log("Disconnect in single player");
			PhotonNetwork.LoadLevel("MainMenu");
		}
	}

	private void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		Debug.Log("Fail to connect to Photon. Reason: " + cause);
		if (_reconnectTryCount > 20)
		{
			Debug.Log("Could not connet to Photon");
			PhotonNetwork.LoadLevel("MainMenu");
		}
		else
		{
			StartCoroutine("Reconnect");
		}
	}

	private List<PhotonSpawnMobInfo> GetMobsSpawnInfo()
	{
		List<PhotonSpawnMobInfo> list = new List<PhotonSpawnMobInfo>();
		if (_worldInfo.Spawns != null)
		{
			foreach (ObjectsSpawn spawn in _worldInfo.Spawns)
			{
				SpawnType spawnType = spawn.SpawnType;
				if (spawnType == SpawnType.Mob)
				{
					list.AddRange(spawn.GetMobSpawnInfo(_mobsSpawnLayerMask));
				}
			}
		}
		return list;
	}

	public WorldInfo LoadWorldInfo()
	{
		string path = "world";
		if (JsSpeeker.I.ReskinType == ReskinGameType.Wasteland)
		{
			path = "worldwasteland";
		}
		if (DataKeeper.GameType == GameType.SkyWars)
		{
			path = "worldskywars";
		}
		if (DataKeeper.GameType == GameType.Tutorial)
		{
			path = "tutorial";
		}
		TextAsset textAsset = (TextAsset)Resources.Load(path);
		if (!string.IsNullOrEmpty(textAsset.text))
		{
			return JsonReader.Deserialize<WorldInfo>(textAsset.text);
		}
		return null;
	}

	public void InstantiateLocalItem(string id, Vector3 position, Quaternion rotation, byte count, byte additionalCount)
	{
		LocalDropObject component = ((GameObject)Object.Instantiate(Resources.Load("LocalItem"), position, rotation)).GetComponent<LocalDropObject>();
		component.Initialize(id, count, additionalCount);
	}

	private void InstantiateLocalItems()
	{
		if (_worldInfo.Spawns != null)
		{
			foreach (ObjectsSpawn spawn in _worldInfo.Spawns)
			{
				spawn.SpawnObjects(Info, true);
			}
		}
		if (DataKeeper.GameType == GameType.Single)
		{
			StartCoroutine("LoadWorldInSingle");
		}
		_localObjSpawed = true;
	}

	private void CreateWorld()
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		_worldInfo = LoadWorldInfo();
		if (_worldInfo.Objects != null)
		{
			foreach (WorldObject @object in _worldInfo.Objects)
			{
				DestructibleObject worldObjInfo = Info.GetDestructibleObjectInfo(@object.ObjectId);
				short num = (short)Info.DestructibleObjects.FindIndex((DestructibleObject wobj) => wobj.Id == worldObjInfo.Id);
				PhotonNetwork.InstantiateSceneObject("WorldObjects/" + worldObjInfo.Prefab, ParseUtils.Vector3FromString(@object.Position), Quaternion.Euler(ParseUtils.Vector3FromString(@object.Rotation)), 0, new object[5] { 0, num, null, null, null });
			}
		}
		if (_enableTestSpawns)
		{
			StartCoroutine("TestItemsSpawns");
		}
	}

	private void OnMasterClientSwitched()
	{
		StopCoroutine("AddMobsForPulling");
		StopCoroutine("AutoSaveMultiplayerWorld");
		if (IsTester())
		{
			Debug.Log("autosaveworld cor STOPPED " + Time.time);
		}
		if (PhotonNetwork.isMasterClient)
		{
			StartCoroutine("AddMobsForPulling");
			StartCoroutine("AutoSaveMultiplayerWorld");
		}
	}

	private IEnumerator AddMobsForPulling()
	{
		if (PhotonNetwork.room.CustomProperties.ContainsKey("zombies") && (bool)PhotonNetwork.room.CustomProperties["zombies"])
		{
			int simpleZombieCount = MobPullingSystem.Instance.GetAllMobsCountByType(ZombieType.Simple);
			int infectiveZombieCount = MobPullingSystem.Instance.GetAllMobsCountByType(ZombieType.Infective);
			int megaZombieCount = MobPullingSystem.Instance.GetAllMobsCountByType(ZombieType.Mega);
			for (int k = 0; k < ZombieBalanceService.SimpleZombieMaxCount - simpleZombieCount; k++)
			{
				PhotonNetwork.InstantiateSceneObject("SZ", Vector3.zero, Quaternion.identity, 0, null);
				yield return new WaitForSeconds(0.1f);
			}
			for (int j = 0; j < ZombieBalanceService.InfectiveZombieMaxCount - infectiveZombieCount; j++)
			{
				PhotonNetwork.InstantiateSceneObject("IZ", Vector3.zero, Quaternion.identity, 0, null);
				yield return new WaitForSeconds(0.1f);
			}
			for (int i = 0; i < ZombieBalanceService.MegaZombieMaxCount - megaZombieCount; i++)
			{
				PhotonNetwork.InstantiateSceneObject("MZ", Vector3.zero, Quaternion.identity, 0, null);
				yield return new WaitForSeconds(0.1f);
			}
		}
	}

	private IEnumerator TestItemsSpawns()
	{
		List<Item> items = Info.GetAllItems();
		for (int i = 0; i < 499; i++)
		{
			int itemIndex = UnityEngine.Random.Range(0, items.Count);
			Item itemInfo = items[itemIndex];
			InstantiateLocalItem(additionalCount: (byte)((itemInfo.Type == ItemType.Weapon) ? ((uint)UnityEngine.Random.Range(0, 10)) : 0u), id: itemInfo.Id, position: _testSpawns.SpawnPoints[i], rotation: _testSpawns.SpawnPointsRotations[i], count: (byte)UnityEngine.Random.Range(1, itemInfo.MaxInStack));
			if (i % 50 == 0)
			{
				yield return new WaitForSeconds(0.25f);
			}
		}
		Debug.Log("DONE");
	}

	public string GetResourceLoadPath(ItemType type)
	{
		switch (type)
		{
		case ItemType.Consumables:
			return ConsumablePath;
		case ItemType.Weapon:
			return WeaponPath;
		case ItemType.Clothing:
			return ClothingPath;
		default:
			return string.Empty;
		}
	}

	public void SavePlayer(bool saveWorld)
	{
		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars)
		{
			return;
		}

		int num = 0;

		if (InventoryController.Instance != null)
		{
			if (InventoryController.Instance.Equipment.ContainsKey("Hand"))
			{
				string id = InventoryController.Instance.Equipment["Hand"];
				if (Info.GetWeaponInfo(id) != null)
				{
					int weaponIndex = ItemsRegistry.I.GetWeaponIndex(id);
					if (GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count > weaponIndex)
					{
						num = GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex].bulletsLeft;
					}
				}
			}
			if (DataKeeper.GameType == GameType.Multiplayer)
			{
				PlayerServerSave.Save(DataKeeper.PlayerGameInfoBackup.Position, InventoryController.Instance.Equipment, InventoryController.Instance.Slots, (byte)num, DataKeeper.PlayerGameInfoBackup.HitPoints, DataKeeper.PlayerGameInfoBackup.Hunger, DataKeeper.PlayerGameInfoBackup.Thirst, DataKeeper.PlayerGameInfoBackup.Sickness);
			}
			else if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
			{
				PlayerSaveInfo.Save(DataKeeper.GameType, DataKeeper.MapId, DataKeeper.PlayerGameInfoBackup.Position, InventoryController.Instance.Equipment, InventoryController.Instance.Slots, (byte)num, DataKeeper.PlayerGameInfoBackup.HitPoints, DataKeeper.PlayerGameInfoBackup.Hunger, DataKeeper.PlayerGameInfoBackup.Thirst, DataKeeper.PlayerGameInfoBackup.Sickness);
			}
		}

		if (saveWorld)
		{
			SaveWorldInSingle();
		}
		if (DataKeeper.GameType == GameType.Multiplayer) 
		{
			SaveWorldInMultiplayer ();
		}
	}

	private IEnumerator LoadWorldInSingle()
	{
		if (PlayerPrefs.HasKey("SinglePlayerMap") && !DataKeeper.IsNewGameClick)
		{
			WorldObject[] objects = JsonReader.Deserialize<WorldObject[]>(PlayerPrefs.GetString("SinglePlayerMap"));
			if (objects != null)
			{
				WorldObject[] array = objects;
				foreach (WorldObject worldObject in array)
				{
					DestructibleObject worldObjInfo = Info.GetDestructibleObjectInfo(worldObject.ObjectId);
					string[] storageItemsIds = null;
					byte[] storageItemsCount = null;
					byte[] storageItemsAmmo = null;
					string storageItems = string.Empty;
					string storageAmmo = string.Empty;
					string idsStr = string.Empty;
					if (!string.IsNullOrEmpty(worldObject.AdditionInfo))
					{
						ItemSaveInfo[] storage = JsonReader.Deserialize<ItemSaveInfo[]>(worldObject.AdditionInfo);
						if (storage != null)
						{
							storageItemsIds = new string[storage.Length];
							storageItemsCount = new byte[storage.Length];
							storageItemsAmmo = new byte[storage.Length];
							for (int i = 0; i < storage.Length; i++)
							{
								if (storage[i] != null && !string.IsNullOrEmpty(storage[i].ItemId))
								{
									idsStr = idsStr + storage[i].ItemId + "/";
									storageItems = storageItems + storage[i].Count + "/";
									storageAmmo = storageAmmo + storage[i].AdditionalCount + "/";
									storageItemsIds[i] = storage[i].ItemId;
									storageItemsCount[i] = storage[i].Count;
									storageItemsAmmo[i] = storage[i].AdditionalCount;
								}
							}
						}
					}
					short[] itemsIdsInShort = ((storageItemsIds == null) ? null : GetItemsShortIds(storageItemsIds));
					short worldObjIndex = (short)Info.DestructibleObjects.FindIndex((DestructibleObject wobj) => wobj.Id == worldObjInfo.Id);
					PhotonNetwork.InstantiateSceneObject("WorldObjects/" + worldObjInfo.Prefab, ParseUtils.Vector3FromString(worldObject.Position), Quaternion.Euler(ParseUtils.Vector3FromString(worldObject.Rotation)), 0, new object[5]
					{
						DataKeeper.BackendInfo.user.playerId,
						worldObjIndex,
						itemsIdsInShort,
						storageItemsCount,
						storageItemsAmmo
					});
				}
			}
		}
		yield return null;
	}

	public short[] GetItemsShortIds(string[] itemsIds)
	{
		List<Item> allItems = Info.GetAllItems();
		short[] array = new short[itemsIds.Length];
		for (int i = 0; i < itemsIds.Length; i++)
		{
			array[i] = (short)allItems.FindIndex((Item obj) => obj.Id == itemsIds[i]);
		}
		return array;
	}

	public string[] GetItemsIdsByIndices(short[] itemsIndices)
	{
		List<Item> allItems = Info.GetAllItems();
		string[] array = new string[itemsIndices.Length];
		for (int i = 0; i < itemsIndices.Length; i++)
		{
			array[i] = ((itemsIndices[i] >= 0) ? allItems[itemsIndices[i]].Id : null);
		}
		return array;
	}

	public void SaveWorldInSingle()
	{
		List<WorldObject> list = new List<WorldObject>();
		foreach (PhotonWorldObject worldObject in WorldObjects)
		{
			if (!worldObject.IsLocal)
			{
				list.Add(worldObject.GetSingleplayerSaveInfo());
			}
		}
		string value = JsonWriter.Serialize(list);
		PlayerPrefs.SetString("SinglePlayerMap", value);
		PlayerPrefs.Save();
	}

	public void LoadContent()
	{
		IsDone = false;
		TextAsset textAsset = Resources.Load<TextAsset>("content");
		if (!string.IsNullOrEmpty(textAsset.text))
		{
			Info = JsonReader.Deserialize<ContentInfo>(textAsset.text);
		}
		Info.Initialize();
		IsDone = true;
	}

	public void ChangeWeather(byte weatherType)
	{
		base.photonView.RPC("SyncWeather", PhotonTargets.All, weatherType);
	}

	private IEnumerator AutoSaveProcess()
	{
		yield return new WaitForSeconds(DataKeeper.AutoSaveTime);
		SavePlayer(DataKeeper.GameType == GameType.Single);
		StartCoroutine("AutoSaveProcess");
	}

	public static bool IsTester()
	{
		if (PhotonNetwork.playerName != null && PhotonNetwork.playerName == "20478755")
		{
			return true;
		}
		return false;
	}

	private IEnumerator AutoSaveMultiplayerWorld()
	{
		if (IsTester())
		{
			Debug.Log("autosaveworld cor started " + Time.time);
		}
		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars)
		{
			yield break;
		}
		while (true)
		{
			if (IsTester())
			{
				Debug.Log("TESTER INFO 1: AutoSaveWorldCoroutine. master=" + PhotonNetwork.isMasterClient + ", _multiplayerWorldLoaded: " + _multiplayerWorldLoaded + ", " + Time.time);
			}
			if (PhotonNetwork.isMasterClient && PhotonNetwork.room != null && PhotonNetwork.room.CustomProperties != null) {
				if (!PhotonNetwork.room.CustomProperties.ContainsKey ("lastSaveTime")) {
					Debug.Log ("no lastSaveTime, INIT! " + Time.time);
					ExitGames.Client.Photon.Hashtable hs = new ExitGames.Client.Photon.Hashtable { {
						"lastSaveTime",
						PhotonNetwork.time
					} };
					PhotonNetwork.room.SetCustomProperties(hs);
				}
				bool needSave = false;
				if (_multiplayerWorldLoaded && PhotonNetwork.room.CustomProperties.ContainsKey("lastSaveTime"))
				{
					float save_timeout = DataKeeper.AutoSaveWorldTime;
					if (IsTester())
					{
						Debug.Log("TESTER INFO 3: PhotonNetwork.time=" + PhotonNetwork.time + ", save_timeout=" + save_timeout + ", lastSaveTime: " + (double)PhotonNetwork.room.CustomProperties["lastSaveTime"]);
						save_timeout = DataKeeper.AutoSaveWorldTimeTesting;
					}
					double diff = PhotonNetwork.time - (double)PhotonNetwork.room.CustomProperties["lastSaveTime"];
					needSave = diff > (double)save_timeout;
				}
				if (IsTester() && !PhotonNetwork.room.CustomProperties.ContainsKey("lastSaveTime"))
				{
					Debug.LogError("TESTER INFO 2: no lastSaveTime prop");
				}
				if (needSave && MultiplayerWorldLoaded && DataKeeper.GameType == GameType.Multiplayer)
				{
					SaveWorldInMultiplayer();
					Debug.Log("Auto save(new) world! from master client");
				}
			}
			else if (!PhotonNetwork.isMasterClient && MultiplayerWorldLoaded && DataKeeper.GameType == GameType.Multiplayer)
			{
				PHPNetwork.I.SavePlayerProgressMultiplayer ((string)PhotonNetwork.room.CustomProperties["id"]);
				Debug.Log ("Auto save world player");
			}

			yield return new WaitForSeconds(1f);
		}
	}

	public void SaveWorldInMultiplayer()
	{
		List<WorldObject> list = new List<WorldObject>();

		foreach (PhotonWorldObject worldObject in WorldObjects)
		{
			if (!worldObject.IsLocal)
			{
				list.Add(worldObject.GetSingleplayerSaveInfo());
			}
		}

		string value = JsonWriter.Serialize(list);

		if (value != string.Empty && MultiplayerWorldLoaded) 
		{
			PHPNetwork.I.SaveWorldData (PhotonNetwork.room.Name, value);
			PHPNetwork.I.SavePlayerProgressMultiplayer ((string)PhotonNetwork.room.CustomProperties["id"]);
		}

		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}

		Debug.Log("SAVING WORLD attempt, i am master " + Time.time);
		string text = ((PhotonNetwork.room == null || !PhotonNetwork.room.CustomProperties.ContainsKey("id")) ? null : ((string)PhotonNetwork.room.CustomProperties["id"]));
		if (!string.IsNullOrEmpty(text) && MultiplayerWorldLoaded)
		{
			Debug.Log("world saved from master client " + Time.time + " room id " + text);

			ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
			hashtable.Add("lastSaveTime", PhotonNetwork.time);
			PhotonNetwork.room.SetCustomProperties(hashtable);
		}
		else if (string.IsNullOrEmpty(text))
		{
			Debug.LogError("error save, roomid = null");
		}
	}

	public void LoadWorldInMultiplayer(string serverData)
	{
		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars || PhotonNetwork.room == null)
		{
			return;
		}

		/*string text = ((PhotonNetwork.room == null || !PhotonNetwork.room.customProperties.ContainsKey("id")) ? null : ((string)PhotonNetwork.room.customProperties["id"]));
		if (!string.IsNullOrEmpty(text))
		{
			if (PhotonNetwork.room == null || !PhotonNetwork.room.customProperties.ContainsKey("RL") || !(bool)PhotonNetwork.room.customProperties["RL"])
			{
				PHPNetwork.I.GetMapState(1, text, OnMultiplayerMapStateLoaded);
			}
		}
		else
		{
			Debug.LogError("won't load world in multiplayer, roomId is null or empty");
		}*/

		if ((serverData != string.Empty || serverData != "Array" || serverData != "[]") && !_multiplayerWorldLoaded && PhotonNetwork.isMasterClient) 
		{
				WorldObject[] objects = JsonReader.Deserialize<WorldObject[]> (serverData);
				if (objects != null)
			     {
					WorldObject[] array = objects;
					foreach (WorldObject worldObject in array) {
						DestructibleObject worldObjInfo = Info.GetDestructibleObjectInfo (worldObject.ObjectId);
						string[] storageItemsIds = null;
						byte[] storageItemsCount = null;
						byte[] storageItemsAmmo = null;
						string storageItems = string.Empty;
						string storageAmmo = string.Empty;
						string idsStr = string.Empty;
						if (!string.IsNullOrEmpty (worldObject.AdditionInfo)) {
							ItemSaveInfo[] storage = JsonReader.Deserialize<ItemSaveInfo[]> (worldObject.AdditionInfo);
							if (storage != null) {
								storageItemsIds = new string[storage.Length];
								storageItemsCount = new byte[storage.Length];
								storageItemsAmmo = new byte[storage.Length];
								for (int i = 0; i < storage.Length; i++) {
									if (storage [i] != null && !string.IsNullOrEmpty (storage [i].ItemId)) {
										idsStr = idsStr + storage [i].ItemId + "/";
										storageItems = storageItems + storage [i].Count + "/";
										storageAmmo = storageAmmo + storage [i].AdditionalCount + "/";
										storageItemsIds [i] = storage [i].ItemId;
										storageItemsCount [i] = storage [i].Count;
										storageItemsAmmo [i] = storage [i].AdditionalCount;
									}
								}
							}
						}
						short[] itemsIdsInShort = ((storageItemsIds == null) ? null : GetItemsShortIds (storageItemsIds));
						short worldObjIndex = (short)Info.DestructibleObjects.FindIndex ((DestructibleObject wobj) => wobj.Id == worldObjInfo.Id);
						PhotonNetwork.InstantiateSceneObject ("WorldObjects/" + worldObjInfo.Prefab, ParseUtils.Vector3FromString (worldObject.Position), Quaternion.Euler (ParseUtils.Vector3FromString (worldObject.Rotation)), 0, new object[5] {
							DataKeeper.BackendInfo.user.playerId,
							worldObjIndex,
							itemsIdsInShort,
							storageItemsCount,
							storageItemsAmmo
						});
					}
				}

		}
		_multiplayerWorldLoaded = true;
	}

	private void OnMultiplayerMapStateLoaded(string info)
	{
		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars || _multiplayerWorldLoaded)
		{
			return;
		}
		if (!string.IsNullOrEmpty(info) && PhotonNetwork.isMasterClient)
		{
			Debug.Log("I am master: load save");
			StartCoroutine("LoadObjFromMultiplayerSave", info);
			return;
		}
		_multiplayerWorldLoaded = true;
		if (!string.IsNullOrEmpty(info))
		{
			Debug.Log("not Empty map save on server! Master? " + PhotonNetwork.isMasterClient);
		}
		else
		{
			Debug.Log("(yes!) Empty map save on server! Master? " + PhotonNetwork.isMasterClient);
		}
	}

	private IEnumerator LoadObjFromMultiplayerSave(string info)
	{
		WorldObjectSaveInfo[] mapInfo = JsonReader.Deserialize<WorldObjectSaveInfo[]>(info);
		if (mapInfo != null)
		{
			int groupCount = 0;
			WorldObjectSaveInfo[] array = mapInfo;
			foreach (WorldObjectSaveInfo objInfo in array)
			{
				if (objInfo != null && objInfo.I < Info.DestructibleObjects.Count)
				{
					DestructibleObject worldObjInfo = Info.DestructibleObjects[objInfo.I];
					short[] storageItemsIndices = null;
					byte[] storageItemsCount = null;
					byte[] storageItemsAmmo = null;
					if (objInfo.S != null)
					{
						storageItemsIndices = objInfo.S.I;
						storageItemsCount = objInfo.S.C;
						storageItemsAmmo = objInfo.S.A;
					}
					PhotonNetwork.InstantiateSceneObject(position: Vector3Short.GetVector3(objInfo.P, 10f), rotation: Quaternion.Euler(Vector3Short.GetVector3(objInfo.R, 10f)), prefabName: "WorldObjects/" + worldObjInfo.Prefab, group: 0, data: new object[5] { objInfo.UId, objInfo.I, storageItemsIndices, storageItemsCount, storageItemsAmmo });
					groupCount++;
					if (groupCount > 5)
					{
						yield return new WaitForSeconds(0.1f);
						groupCount = 0;
					}
				}
			}
		}
		if (PhotonNetwork.room != null)
		{
			ExitGames.Client.Photon.Hashtable hs = new ExitGames.Client.Photon.Hashtable { { "RL", true } };
			PhotonNetwork.room.SetCustomProperties(hs);
		}
		_multiplayerWorldLoaded = true;
	}

	private string GetInfoForSaveMultiplayer()
	{
		List<WorldObjectSaveInfo> list = new List<WorldObjectSaveInfo>();
		foreach (PhotonWorldObject worldObject in WorldObjects)
		{
			if (!worldObject.IsLocal)
			{
				list.Add(worldObject.GetMultiplayerSaveInfo());
			}
		}
		return JsonWriter.Serialize(list);
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("SyncWeather", player, _weather.currentWeather);

			if (PhotonNetwork.room != null && PhotonNetwork.room.CustomProperties.ContainsKey("C0"))
			{
				ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
				hashtable.Add("C0", PhotonNetwork.room.PlayerCount);
				PhotonNetwork.room.SetCustomProperties(hashtable);
			}
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient && GlobalCarManager.I != null)
		{
			GlobalCarManager.I.ExitFromCarIfEixst(player.NickName);
		}
	}

	[PunRPC]
	public void SyncWeather(byte weatherType)
	{
		_weather.SyncWeather(weatherType);
	}
}
