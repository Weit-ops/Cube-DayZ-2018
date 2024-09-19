using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using LitJson;
using UnityEngine;

[Serializable]
public class UserBackendInfo
{
	public int playerId;
	public string user_id;
	public bool has_premium;
	public string items;
	public string premium_end_time;
	public int premium_remain_seconds;
	public string premium_vip_end_time;
	public int premium_vip_remain_seconds;
	public bool has_vip;
	public int profit;
	public string GetClientId(){
		string text = SystemInfo.deviceUniqueIdentifier.Length.ToString() + "0403" + SystemInfo.deviceName.Length ;
		return text;
	}
}

[Serializable]
public class BackendInfo
{
	public UserBackendInfo user;
	public List<PurchasedItemsBackensInfo> purchased_items;
}

[Serializable]
public class PurchasedItemsBackensInfo
{
	public int shop_id;
	public int count;
}

public class Md5
{
	public static string Md5Sum(string strToEncrypt)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(strToEncrypt);
		return Md5Sum(bytes);
	}

	public static string Md5Sum(byte[] bytes)
	{
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += Convert.ToString(array[i], 16).PadLeft(2, "0"[0]);
		}
		return text.PadLeft(32, "0"[0]);
	}
}

public class VkMissions
{
	public static int[] vkMissionsIDs = new int[65]
	{
		9, 8, 7, 6, 57, 56, 55, 54, 53, 52,
		51, 50, 5, 49, 48, 47, 46, 45, 44, 43,
		42, 41, 40, 4, 39, 38, 37, 36, 35, 34,
		33, 32, 31, 30, 3, 29, 28, 27, 26, 25,
		24, 23, 22, 21, 20, 19, 18, 17, 16, 15,
		14, 13, 12, 11, 10, 65, 64, 63, 62, 61,
		60, 59, 58, 66, 67
	};

	public static List<int> SessionCompleatedMissions = new List<int>();

	public static string GetMissionID_IOS(string id)
	{
		return "com.cubedz.ach" + id;
	}
}

public delegate void WebResponse(string webData);

public class PHPNetwork : MonoBehaviour
{

	public static string[] SpecialPacksNames = new string[]
	{
		"Builder",
		"Special_Forces",
		"Doctor",
		"Farmer",
		"Sniper",
		"BomberMan",
		"Stalker",
		"Trapper",
		"Rambo"
	};

	public static string db_url = "https://static2-speedybyte.ru/cdz/";

	public string phpSecret { get; private set;}

	public int serverId
	{
		get
		{
			return DataKeeper.BackendInfo.user.playerId;
		}
	}

	public static PHPNetwork I;



	public byte GetOfferStatus()
	{
		byte result = 1;
		if (DataKeeper.BackendInfo.purchased_items != null)
		{
			PurchasedItemsBackensInfo purchasedItemsBackensInfo = DataKeeper.BackendInfo.purchased_items.Find((PurchasedItemsBackensInfo item) => item.shop_id == 102 && item.count > 0);
			if (purchasedItemsBackensInfo != null)
			{
				result = 2;
				PurchasedItemsBackensInfo purchasedItemsBackensInfo2 = DataKeeper.BackendInfo.purchased_items.Find((PurchasedItemsBackensInfo item) => item.shop_id == 103 && item.count > 0);
				if (purchasedItemsBackensInfo2 != null)
				{
					result = 3;
					PurchasedItemsBackensInfo purchasedItemsBackensInfo3 = DataKeeper.BackendInfo.purchased_items.Find((PurchasedItemsBackensInfo item) => item.shop_id == 108 && item.count > 0);
					if (purchasedItemsBackensInfo3 != null)
					{
						result = 0;
					}
				}
			}
		}
		return result;
	}

	public static string GetMD5(string input)
	{
		byte[] array = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}
		

	public IEnumerator GetProfile(string id,string newName,string secret,WebResponse wr)
	{
		string baseServer = db_url + "?";
		string param = string.Empty;
		string replyUrl = string.Empty;
		string requestSig = string.Empty;
		param = "user_id=" + id;
		requestSig += param;
		replyUrl += param + "&";
		param = "secretKey=" + secret;
		requestSig += param;
		replyUrl += param + "&";
		param = "v=" + DataKeeper.BuildVersion;
		requestSig += param;
		replyUrl += param + "&";
		param = "requestCode=1";
		replyUrl += param + "&";
		param = "playerName=" + newName;
		requestSig += param;
		replyUrl += param + "&";
		string paramsHash = GetMD5 (requestSig + phpSecret );
		param = "hasher=" + paramsHash;
		replyUrl += param;
		WWW newCallback = new WWW (baseServer + replyUrl);
		Debug.Log (newCallback.url);
		yield return newCallback;
		string response = newCallback.text;
		wr (newCallback.text);

	}
	public void SaveView(PlayerViewSaveInfo info)
	{
		StartCoroutine (_SaveView (info));
	}
	private IEnumerator _SaveView(PlayerViewSaveInfo info)
	{
		string baseServer = db_url + "?";
		string param = string.Empty;
		string replyUrl = string.Empty;
		string requestSig = string.Empty;
		param = "playerId=" + serverId;
		requestSig += param;
		replyUrl += param + "&";
		param = "requestCode=2";
		replyUrl += param + "&";
		param = "skinFace=" + info.FaceIndex;
		requestSig += param;
		replyUrl += param + "&";
		param = "skinColor=" + info.SkinColorIndex;
		requestSig += param;
		replyUrl += param + "&";
		param = "premiumPack=" + info.PremiumPackIndex;
		requestSig += param;
		replyUrl += param + "&";
		param = "vipRainCoat=" + info.VipRainCoat;
		requestSig += param;
		replyUrl += param + "&";
		string paramsHash = GetMD5 (requestSig + phpSecret);
		param = "hasher=" + paramsHash;
		replyUrl += param;
		WWW newCallback = new WWW (baseServer + replyUrl);
		yield return newCallback;
		Debug.Log (newCallback.text);
	}

	public void SavePlayerProgressMultiplayer(string id)
	{
		Dictionary <string,string> possable = new Dictionary<string,string> ();
		possable ["playerId"] = (serverId + "_" + id).ToString();
		possable ["playerData"] = Convert.ToBase64String(WorldController.I.SavePlayerData());
		SendWaitingCallback (3,possable,null);
	}
	public void LoadPlayerProgressMultiplayer(string id,Action<string> callback)
	{
		Dictionary <string,string> possable = new Dictionary<string,string> ();
		possable ["playerId"] = (serverId + "_" + id).ToString();
		SendWaitingCallback (4, possable,callback);
	}

	public void SaveWorldData(string name,string data)
	{
		Dictionary <string,string> possable = new Dictionary<string,string> ();
		possable ["worldName"] = name;
		possable ["worldData"] = data;
		SendWaitingCallback (6, possable,null);
	}

	public void LoadWorldMultiplayer(string name,Action<string> responseCallback){
		Dictionary <string,string> possable = new Dictionary<string,string> ();
		possable ["worldName"] = name;
		SendWaitingCallback (7, possable, responseCallback);
	}

	public void UseSpecialPack(int packId)
	{
		Dictionary <string,string> Data = new Dictionary<string,string> ();
		Data ["playerId"] = serverId.ToString();
		Data ["specialPackId"] = packId.ToString ();
		SendWaitingCallback (9, Data, UpdateSpecialPackCount);
	}


	public void UpdateSpecialPackCount(string response)
	{
		JsonData jsix = JsonMapper.ToObject (response);
		int num = DataKeeper.SelectedSpecialPackId;
		DataKeeper.BackendInfo.purchased_items[num - 1].count = Convert.ToInt32(jsix[SpecialPacksNames[num - 1]].ToString());
	}
	public void SendWaitingCallback(int q, Dictionary<string,string> param,Action<string> responseCallback)
	{
		StartCoroutine (_SendWaitingCallback (q,param,responseCallback));
	}

	public IEnumerator _SendWaitingCallback(int q,Dictionary<string,string> param,Action<string> responseCallback)
	{
		WWWForm form = new WWWForm ();
		form.AddField ("requestCode", q);
		string requestSig = string.Empty;
		foreach (var headers in param)
		{
			form.AddField(headers.Key,headers.Value);
			requestSig = requestSig + headers.Key + "=" + headers.Value;
		}
		form.AddField ("hasher", GetMD5(requestSig));
		WWW newCallback = new WWW (db_url,form);
		yield return newCallback;
		if (newCallback.error == null && responseCallback != null) 
		{
			responseCallback (newCallback.text);
		} 
		else if (newCallback.error != null)
		{
			Debug.Log("Server Request Error - " + newCallback.error);
		}
	}

	private void Awake(){
		
		DontDestroyOnLoad (gameObject);
		phpSecret = GetMD5 ("ivanXO_devloper2008");
		db_url = "https://static2-speedybyte.ru/cdz/cubez_backend.php";
		I = this;
	}
}
