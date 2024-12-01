using System;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using UnityEngine;

public enum ReskinGameType
{
	Default = 0,
	Wasteland = 1,
	SkyWars = 2,
	SkyWarsMobile = 3,
	UnturnedMobile = 4
}

public class VkPlayerInfo
{
	public string uid;
	public string first_name;
	public string last_name;
	public string photo_max_url;
}

public class JsSpeeker : MonoBehaviour
{
	private string _uploadScreenShotUrl;
	public bool IsMobile;
	public ReskinGameType ReskinType;
	private static JsSpeeker i;
	private bool can_start = true;
	private float fb_payout_foreign_exchange_rate;
	private string fb_user_currency;
	public bool isDevGroup;
	public int nickLengh;
	public bool CanGetKey;

	public static string viewer_id { get; private set; }
	public static string auth_key { get; private set; }
	public static string vk_name { get; private set; }
	public static Texture vk_photo { get; private set; }
	public static string PhotoUrl { get; private set; }
	public static List<VkPlayerInfo> FriendsInfos { get; private set; }

	public static JsSpeeker I
	{
		get
		{
			return i;
		}
	}

	public void SetFriendsInfos(List<VkPlayerInfo> friends)
	{
		FriendsInfos = friends;
	}

	public float GetFbExchangeRate()
	{
		return fb_payout_foreign_exchange_rate;
	}

	public string GetFbCurrency()
	{
		return fb_user_currency;
	}

	private void Awake()
	{
		if (i == null)
		{
			i = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			DataKeeper.SetupBuildVersion();
		}
		else
		{
			can_start = false;
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void SetDebugUser_Vadim2()
	{
		vk_name = "Вадим Янтбелидзе 2";
		viewer_id = "7230618000";
		PhotonNetwork.playerName = viewer_id;
	}

	private IEnumerator AsyncInit()
	{
		yield return null;

		if (MenuConnectionController.I != null)
		{
			MenuConnectionController.I.ConnectToPhoton();
		}
	}

	private void Start()
	{
		if (UpdateManager.I != null) {
			UpdateManager.I.UpdateTaggedName ();
			UpdateManager.I.StartConnection ();
		}
	}

	public IEnumerator SocialLoginCheker()
	{
		while (string.IsNullOrEmpty(viewer_id))
		{
			yield return new WaitForSeconds(1f);
			if (string.IsNullOrEmpty(viewer_id))
			{
				if (Application.absoluteURL.Contains("play_fb") || Application.absoluteURL.Contains("facebook"))
				{
					Debug.Log("Try to login FB");
					//Application.ExternalCall("InitFB");
				}
				else
				{
					Debug.Log("Try to login VK");
					//Application.ExternalCall("InitVK");
				}
			}
		}
		Debug.Log("SocialLoginCheker Stoped ");
	}

	public bool IsFacebook()
	{
		return Application.absoluteURL.Contains("play_fb") || Application.absoluteURL.Contains("facebook");
	}

	private void OnBuySomething(string id)
	{
		Debug.Log("OnBuySomething " + id);

		if ((id == "333" || id == "533") && UpdateManager.I != null)
		{
			UpdateManager.I.UserActionSendMessage(" купил VIP");
		}
	}

	private void OnGetVkFriends(string response)
	{
		Debug.Log("On GET FRIENDS " + response);
		if (string.IsNullOrEmpty(response))
		{
			return;
		}

		VkPlayerInfo[] array = JsonReader.Deserialize<VkPlayerInfo[]>(response);

		if (array != null)
		{
			FriendsAndClanPanelView.I.ActivateInvateFriendBtn(false);
			FriendsInfos = new List<VkPlayerInfo>(array);

			FriendsAndClanPanelView.I.ActivateFriendPanel(true);
			Debug.Log("Get Vk friend ");

			if (array.Length <= 0)
			{
				FriendsAndClanPanelView.I.ActivateInvateFriendBtn(true);
			}
		}
		else
		{
			FriendsAndClanPanelView.I.ActivateInvateFriendBtn(true);
		}
	}

	public void _auth_key(string v_key)
	{
		auth_key = v_key;
	}

	public void _vk_name(string vkId)
	{
		viewer_id = vkId;
		Debug.Log("user_id: " + viewer_id);

		if (LoginMenuBlock.I != null)
		{
			LoginMenuBlock.I.SetPlayerId(vkId);
		}

		PhotonNetwork.playerName = vkId;
		PhotonNetwork.player.UserId = vkId;
		if (MenuConnectionController.I != null && vkId != null)
		{
			Debug.Log("Connect to Photon");
			MenuConnectionController.I.ConnectToPhoton();
		}
	}

	public void _vk_nick(string v_name)
	{
		vk_name = v_name;
		Debug.Log("user _ name: " + vk_name);

		if (LoginMenuBlock.I != null)
		{
			LoginMenuBlock.I.SetPlayerName(vk_name);
		}
	}

	public void CheckDevGroup(object response)
	{
		isDevGroup = Convert.ToInt32(response) > 1;
		nickLengh = viewer_id.Length;
	}

	public void _vk_photo(string v_photo)
	{
		PhotoUrl = v_photo;
	}

	

	public void _fb_user_currency(object currency)
	{

	}

	public void InviteFriendsManual()
	{
		Debug.LogError("NOT IMPLEMENTED YET");
	}

	public void PostAchievement()
	{
		if (!Application.isEditor)
		{
			Screen.SetResolution(960, 600, false);
		}
		//Debug.LogError("NOT IMPLEMENTED YET 2");
	}

	public void WallPostOnFriendsWall(string friend_id, string _text)
	{
		if (!Application.isEditor)
		{
			Screen.SetResolution(960, 600, false);
		}
		//Application.ExternalCall("SAPIAddPost", friend_id, _text);
	}

	public void InviteFriendsNative()
	{
		if (!Application.isEditor)
		{
			Screen.SetResolution(960, 600, false);
		}
		//Application.ExternalCall("SAPIInviteFriend");
	}

	public void WallPostSomething(string id, string _text)
	{
		if (!Application.isEditor)
		{
			Screen.SetResolution(960, 600, false);
		}
		//Application.ExternalCall("SAPIAddPost", id, _text);
	}

	public void ShareCar(int carShopId)
	{
		string id = "car1";
		int current_mission_id = 65;
		Debug.Log(carShopId);
		switch (carShopId)
		{
		case 101:
			id = "car1";
			current_mission_id = 66;
			break;
		case 102:
			id = "car2";
			current_mission_id = 67;
			break;
		case 103:
			id = "car3";
			current_mission_id = 58;
			break;
		case 104:
			id = "car4";
			current_mission_id = 59;
			break;
		case 105:
			id = "car5";
			current_mission_id = 62;
			break;
		case 106:
			id = "car6";
			current_mission_id = 60;
			break;
		case 107:
			id = "car7";
			current_mission_id = 61;
			break;
		case 108:
			id = "car8";
			current_mission_id = 63;
			break;
		case 109:
			id = "car9";
			current_mission_id = 65;
			break;
		case 110:
			id = "car10";
			current_mission_id = 64;
			break;
		}
		WallPostSomething(id, string.Empty);

	}

	public void BuySomething(string id)
	{
		Application.OpenURL ("https://vk.com/club227221769");
	}

	public void FacebookPaymentWindow(string item_id)
	{

	}

	public void OnItemBuy(string bla)
	{

	}

	private IEnumerator OnFbBuyWait()
	{
		yield break;
	}

	public void OnBuyGold()
	{
	}

	public void ReloadFrame()
	{
		Debug.LogError("Refresh page!");
		//Application.ExternalEval("document.location.reload(true)");
	}

	public void OnRefreshPage()
	{
		Debug.Log("OnRefreshPAGE! ");
	}

	public void VkStorageSet(string key, string value)
	{
		CanGetKey = false;
	}

	public void OnStorageSetValue(object key)
	{
		if (key.ToString() == "1")
		{
			CanGetKey = true;
		}
	}

	public void VkStorageGet(string key)
	{
		if (CanGetKey)
		{
			CanGetKey = false;
			//Application.ExternalCall("VkStorageGet", key);
		}
	}

	public void OnStorageGet(string key)
	{
		if (key != null)
		{
			MenuConnectionController.I.KeyFromServer = key;

			if (!string.IsNullOrEmpty(MenuConnectionController.I.CurrentSessionKey) && !string.IsNullOrEmpty(key) && MenuConnectionController.I.CurrentSessionKey != key && TwoTabsDetector.I != null)
			{
				Debug.Log("Session is not valid, two tabs detected!");
				TwoTabsDetector.I.ExitFromServer();
				PhotonNetwork.Disconnect();
			}

			CanGetKey = true;
		}
	}

	public void PlaySong()
	{
		if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
		{
			//Application.ExternalCall("PlaySong");
		}
	}

	public void PlayNextSong()
	{
		if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
		{
			//Application.ExternalCall("PlayNextSong");
		}
	}

	public void PlayPrevSong()
	{
		if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
		{
			//Application.ExternalCall("PlayPrevSong");
		}
	}

	public void StopPlayingSong()
	{
		if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
		{
			//Application.ExternalCall("StopPlayingSong");
		}
	}

	public void MuteSong()
	{
		if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
		{
			//Application.ExternalCall("MuteSong");
		}
	}

	public void RadioCallBack(string callback)
	{
		if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
		{
			Debug.Log("Something change in radio" + callback);
		}
	}

	private IEnumerator loadPhoto(string s)
	{
		WWW www = new WWW(s);
		yield return www;
		if (www.error == null)
		{
			vk_photo = www.texture;
		}
		else
		{
			Debug.LogError("invalid load photo");
		}
	}

	public void ScreenShot()
	{
		if (string.IsNullOrEmpty(_uploadScreenShotUrl))
		{
			//Application.ExternalCall("GetAlbums");
		}
		else
		{
			StartCoroutine("PostScreenShot");
		}
	}

	public void OnGetUploadServer(string response)
	{
		_uploadScreenShotUrl = response;
		StartCoroutine("PostScreenShot");
	}

	public IEnumerator PostScreenShot()
	{
		yield return new WaitForEndOfFrame();
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);
		tex.Apply();
		byte[] bytes = tex.EncodeToPNG();
		UnityEngine.Object.Destroy(tex);
		WWWForm form = new WWWForm();
		form.AddField("upload_url", _uploadScreenShotUrl);
		form.AddBinaryData("file1", bytes, "screen.png", "image/png");

	}

	public void OnPostScreenShot(string response)
	{
		//null
	}

	public IEnumerator WaitingForResponse(WWW www, Action<string> callback)
	{
		yield return www;
		if (www.error == null)
		{
			if (callback != null)
			{
				callback(www.text);
			}
		}
		else
		{
			Debug.Log("Failed to post screen shot!");
		}
		www.Dispose();
	}

	public void FakeFrindsList()
	{
		VkPlayerInfo vkPlayerInfo = new VkPlayerInfo();
		vkPlayerInfo.first_name = "Вадим";
		vkPlayerInfo.last_name = "Янтбелидзе";
		vkPlayerInfo.uid = "7230618";
		VkPlayerInfo vkPlayerInfo2 = new VkPlayerInfo();
		vkPlayerInfo2.first_name = "player2";
		vkPlayerInfo2.last_name = "player2player2";
		vkPlayerInfo2.uid = "131987286";
		VkPlayerInfo vkPlayerInfo3 = new VkPlayerInfo();
		vkPlayerInfo3.first_name = "gnoblin";
		vkPlayerInfo3.last_name = "gnoblin";
		vkPlayerInfo3.uid = "3554042";
		VkPlayerInfo vkPlayerInfo4 = new VkPlayerInfo();
		vkPlayerInfo4.first_name = "Leo";
		vkPlayerInfo4.last_name = "Tesla";
		vkPlayerInfo4.uid = "234200499";
		List<VkPlayerInfo> list = new List<VkPlayerInfo>();
		list.Add(vkPlayerInfo);
		list.Add(vkPlayerInfo2);
		list.Add(vkPlayerInfo3);
		list.Add(vkPlayerInfo4);
		FriendsInfos = new List<VkPlayerInfo>(list);
	}

	public void FakeFrindsList2()
	{
		List<VkPlayerInfo> collection = new List<VkPlayerInfo>();
		FriendsInfos = new List<VkPlayerInfo>(collection);
	}
}
