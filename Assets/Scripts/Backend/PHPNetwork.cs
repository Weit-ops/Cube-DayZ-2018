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
	public int GetAuthId(){
		return new Guid().ToString().Length * 240000 + PHPNetwork.MD5Sun(new Guid().ToString()).Length * 400;
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

	public static string db_url = "http://playme24.ru/cdz/cubez_backend.php";

	public string phpSecret { get; private set;}

	public int backendId
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

	public static string MD5Sun(string input)
	{
		byte[] array = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}
		

	public void GetProfile(Action<string> response)
	{
		WWWForm hData = new WWWForm();
		string userId = MD5Sun(SystemInfo.deviceUniqueIdentifier).Substring(0,9) ;
		
        hData.AddField("user_id",userId);
        hData.AddField("version",DataKeeper.BuildVersion);
		hData.AddField("callback","GetProfile");
		SendWaitingCallback(hData,response);
	}
	public void SaveView(PlayerViewSaveInfo info)
	{
		WWWForm hData = new WWWForm();
		hData.AddField("backendId",backendId.ToString());
		hData.AddField("skinFace",info.FaceIndex);
		hData.AddField("skinColor",info.SkinColorIndex);
		hData.AddField("premiumPack",info.PremiumPackIndex);
		hData.AddField("vipRainCoat",info.VipRainCoat);
		hData.AddField("callback","SaveView");
		SendWaitingCallback(hData);
	}


	public void SavePlayerProgressMultiplayer(string id)
	{
		WWWForm form = new WWWForm();
		form.AddField("backendId",(backendId + "_" + id).ToString());
		form.AddField("playerData",Convert.ToBase64String(WorldController.I.SavePlayerData()));
		form.AddField("callback","SavePlayerMultiplayer");
		SendWaitingCallback(form);
		//SendWaitingCallback (3,possable,null);
	}
	public void LoadPlayerProgressMultiplayer(string id,Action<string> callback)
	{
	    WWWForm form = new WWWForm();
		form.AddField("backendId",(backendId + "_" + id).ToString());
		form.AddField("callback","LoadPlayerMultiplayer");
		//SendWaitingCallback (4, possable,callback);
		SendWaitingCallback(form,callback);
	}

	public void SaveWorldData(string name,string data)
	{
		WWWForm form = new WWWForm();
		if (name != string.Empty){
		form.AddField("worldName",name);
		}
		form.AddField("worldData",data);
		form.AddField("callback","SaveWorld");
		SendWaitingCallback(form);
		//SendWaitingCallback (6, possable,null);
	}

	public void LoadWorldMultiplayer(string name,Action<string> responseCallback){
		WWWForm wWForm = new WWWForm();
		wWForm.AddField("worldName",name);
		wWForm.AddField("callback","LoadWorld");
		//SendWaitingCallback (7, possable, responseCallback);
		SendWaitingCallback(wWForm,responseCallback);
	}

	public void UseSpecialPack(int packId)
	{
		WWWForm form = new WWWForm();
		form.AddField("backendId",backendId.ToString());
		form.AddField("specialPackId",packId.ToString());
		form.AddField("callback","UseSpecialPack");
		SendWaitingCallback(form,UpdateSpecialPackCount);
		/*Dictionary <string,string> Data = new Dictionary<string,string> ();
		Data ["playerId"] = backendId.ToString();
		Data ["specialPackId"] = packId.ToString ();
		SendWaitingCallback (9, Data, UpdateSpecialPackCount);*/
	}
	public void ChangeName(string text)
	{
        WWWForm form = new WWWForm();
        form.AddField("backendId", backendId.ToString());
        form.AddField("user_name", text);
        form.AddField("callback", "NickName");
        SendWaitingCallback(form, UpdateSpecialPackCount);
    }

	public void UpdateSpecialPackCount(string response)
	{
		JsonData jsix = JsonMapper.ToObject (response);
		int num = DataKeeper.SelectedSpecialPackId;
		DataKeeper.BackendInfo.purchased_items[num - 1].count = Convert.ToInt32(jsix[SpecialPacksNames[num - 1]].ToString());
	}
	public void SendWaitingCallback( WWWForm hData,Action<string> responseCallback = null)
	{
		StartCoroutine (_SendWaitingCallback (hData,responseCallback));
	}

	public IEnumerator _SendWaitingCallback(WWWForm data,Action<string> responseCallback = null)
	{
		string MD5SUN = string.Empty;
		foreach (var headers in data.headers)
		{
            MD5SUN = MD5SUN + headers.Key + "=" + headers.Value;
		}
		data.AddField ("MD", MD5Sun(MD5SUN));
		WWW newCallback = new WWW (db_url,data);
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
		phpSecret = MD5Sun ("ivanXO_devloper2008");
		I = this;
	}
}
