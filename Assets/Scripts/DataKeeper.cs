using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VariableValue
{
	public float Min;
	public float Max;

	public int GetValueInt()
	{
		return (int)UnityEngine.Random.Range(Min, Max + 1f);
	}

	public float GetValueFloat()
	{
		return UnityEngine.Random.Range(Min, Max);
	}
}

public class DropItem
{
	public string ItemId;
	public VariableValue Count;
}

public enum GameType
{
	Single = 0,
	Multiplayer = 1,
	Tutorial = 2,
	BattleRoyale = 3,
	SkyWars = 4
}

public enum Language
{
	Russian = 0,
	English = 1
}

public enum Sex : byte
{
	Male = 0,
	Female = 1
}

public class PlayerInfoBackup
{
	public Vector3 Position;
	public int HitPoints;
	public int Hunger;
	public int Thirst;
	public int Sickness;
}

public static class DataKeeper
{
	public static bool IsNewGameClick = false;
	public static bool IsBattleRoyaleClick = false;
	public static bool IsSkyWarsClick = false;
	public static string BuildVersion = "2.66";
	public static string _chatAppVersion = "1.32";
	public static float AutoSaveTime = 600f;
	public static float AutoSaveWorldTime = 300f;
	public static float AutoSaveWorldTimeTesting = 30f;
	public static short MaxPlayerHp = 100;
	public static float BR_MoreWeaponsChancePerSpawnZone = 0.2f;
	public static float LocalLootFactor = 0.75f;
	public static float ZombieLootFactor = 0.5f;
	public static float DefaultMobDropChance = 15f;
	public static float DistanceToDropLocalLoot = 75f;
	public static float DefaultPhotonItemLifetime = 180f;
	public static int PhotonDropChanceOnPremium = 10;
	public static Language Language;
	public static GameType GameType;
	public static string MapId;
	public static Sex Sex;
	public static byte FaceIndex;
	public static byte SkinColorIndex;
	public static byte PremiumPackIndex;
	public static byte VipRaincoatIndex;
	public static string NoIcon = "noIcon";
	public static string DummyUserId = "dummyUser";
	private static BackendInfo _backendInfo;
	public static int SelectedSpecialPackId;
	public static bool ShowProfileInfo;
	public static bool OfflineMode = true;
	public static PlayerInfoBackup PlayerGameInfoBackup = new PlayerInfoBackup();

	private static BackendInfo _backendInfoDummy = new BackendInfo
	{
		purchased_items = new List<PurchasedItemsBackensInfo>(),
		user = new UserBackendInfo
		{
			premium_end_time = string.Empty,
			user_id = DummyUserId
		}
	};

	public static BackendInfo BackendInfo
	{
		get
		{
			return _backendInfo ?? _backendInfoDummy;
		}
		set
		{
			_backendInfo = value;
		}
	}

	public static bool IsUserDummy
	{
		get
		{
			return BackendInfo.user.user_id == DummyUserId;
		}
	}

	public static void SetupBuildVersion()
	{
		/*float num = Convert.ToSingle(BuildVersion.Replace('.',','));
		float num2 = Convert.ToSingle(_chatAppVersion.Replace('.',','));
		if (JsSpeeker.I.IsFacebook())
		{
			num += 1f;
			num2 += 1f;
			if (JsSpeeker.I.ReskinType == ReskinGameType.Wasteland)
			{
				num += 2f;
				num2 += 2f;
			}
		}
		else if (JsSpeeker.I.ReskinType == ReskinGameType.Wasteland)
		{
			Debug.LogError("reskin change version");
			num += 3f;
			num2 += 3f;
		}
		BuildVersion = num.ToString();
		_chatAppVersion = num2.ToString();*/
	}
}
