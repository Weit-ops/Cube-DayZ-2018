using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;
using UnityEngine;
using LitJson;
using com.playGenesis.VkUnityPlugin;

public class Controller : MonoBehaviour
{
    const string PLR_GUID = "plrguid8";
    const string PLR_NAM = "plrgnam8";
    public const string PLR_LGT = "logout3";

    public VkApi vkapi;
    public Downloader d;
    public List<VKUser> friends = new List<VKUser>();
    public VkSettings sets;
    public tk2dUITextInput inputName;
    public GameObject SmallNickErrObj;
    bool _isLogout;

    public static Controller I;

    public bool IsLogged
    {
        get
        {
            return JsSpeeker.viewer_id != null;
        }
    }

    private void Awake()
    {
        if (I == null)
        {
            I = this;
        }
    }

    private void Start()
    {
        PHPNetwork.I.GetProfile(OnAuthResponse);
        MainMenuController.I.ToggleMainMenu(true);
        ShowLoginWindow(false);
    }

    private IEnumerator CheckLogin()
    {
        if (string.IsNullOrEmpty(JsSpeeker.vk_name) || string.IsNullOrEmpty(JsSpeeker.viewer_id))
        {
            while (string.IsNullOrEmpty(JsSpeeker.vk_name) && string.IsNullOrEmpty(JsSpeeker.viewer_id))
            {
                GetVkUserInfo(0);
                yield return new WaitForSeconds(1f);
            }
        }
        Debug.Log("Exit from CheckLogin");
    }

    public void ShowLoginWindow(bool flag)
    {
        Debug.Log("showLogin window");
        StartCoroutine("StartShowLoginWindow", flag);
    }

    private IEnumerator StartShowLoginWindow(bool flag)
    {
        if (flag)
        {
            yield return new WaitForSeconds(1f);
        }
        MainMenuController.I.MobileLoginWindow.SetActive(flag);
    }

    private IEnumerator HideMainMenu()
    {
        Debug.Log("start corutine HideMainMen!!!");
        while (MainMenuController.I == null)
        {
            Debug.Log("Disable " + MainMenuController.I);
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("ENABLE " + MainMenuController.I);
        MainMenuController.I.ToggleMainMenu(false);
        ShowLoginWindow(true);
    }

    public void LogOut()
    {
        PlayerPrefs.DeleteKey("plrguid8");
        PlayerPrefs.DeleteKey("plrgnam8");
        Debug.Log("On Click Logout");
        JsSpeeker.I._vk_name(null);
        JsSpeeker.I._vk_nick(null);
        if (JsSpeeker.FriendsInfos != null)
        {
            JsSpeeker.FriendsInfos.Clear();
        }
        DataKeeper.BackendInfo.user.has_vip = false;
        DataKeeper.BackendInfo.user.has_premium = false;
        FriendsAndClanPanelView.I.ClearFriendsList();
        MainMenuController.I.ToggleMainMenu(false);
        ShowLoginWindow(true);
        MenuConnectionViewController.I.InviteFriendsBtn.SetActive(false);
    }

    public void OnClickGuestBtn()
    {

    }


    public void CloseSmallNickPanel()
    {
        SmallNickErrObj.gameObject.SetActive(false);
    }


    public void OnVkAuthSuccesfull()
    {
        /*string userName = VKApiClient.I.user.first_name + "_" + VKApiClient.I.user.last_name;
		StartCoroutine(PHPNetwork.I.GetProfile (VKApiClient.I.user.userId,userName,PHPNetwork.GetMD5(VKApiClient.I.user.userId + "0300400243"), OnAuthResponse));
		MainMenuController.I.ToggleMainMenu(true);
		ShowLoginWindow(false);
		VKApiClient.I.IsAuth = true;
		Debug.Log ("VK auth succesfull!");*/
    }

    public void OnAuthResponse(string data)
    {
        MainMenuController.I._loadingAccountPanel.SetActive(false);
        if (data == "Version^old")
        {
            MainMenuController.I._errorAuthOldVersionPanel.gameObject.SetActive(true);
            Debug.Log("Version game old..");
            return;
        }
        if (data == "is^ban")
        {
            MainMenuController.I._errorAuthBanPanel.gameObject.SetActive(true);
        }
        JsonData jsonData = JsonMapper.ToObject(data);
        DataKeeper.BackendInfo.user.user_id = jsonData["uid"].ToString();
        DataKeeper.BackendInfo.user.playerId = Convert.ToInt32(jsonData["id"].ToString());
        DataKeeper.SkinColorIndex = Convert.ToByte(jsonData["SkinColor"].ToString());
        DataKeeper.FaceIndex = Convert.ToByte(jsonData["SkinFace"].ToString());
        DataKeeper.PremiumPackIndex = Convert.ToByte(jsonData["PremiumPack"].ToString());
        DataKeeper.VipRaincoatIndex = Convert.ToByte(jsonData["VipRainCoat"].ToString());
        if (jsonData["IsVip"].ToString() == "true")
        {
            DataKeeper.BackendInfo.user.has_vip = true;
            DataKeeper.BackendInfo.user.premium_end_time = jsonData["vip_endTime"].ToString();
        }
        if (jsonData["IsSuperVip"].ToString() == "true")
        {
            DataKeeper.BackendInfo.user.has_premium = true;
            DataKeeper.BackendInfo.user.premium_vip_end_time = jsonData["superVip_endTime"].ToString();
        }
        List<PurchasedItemsBackensInfo> array = new List<PurchasedItemsBackensInfo>();
        for (int i = 0; i < PHPNetwork.SpecialPacksNames.Length; i++)
        {
            PurchasedItemsBackensInfo items = new PurchasedItemsBackensInfo();
            items.shop_id = (i + 1);
            items.count = Convert.ToInt32(jsonData[PHPNetwork.SpecialPacksNames[i]].ToString());
            array.Add(items);
        }
        DataKeeper.BackendInfo.purchased_items = new List<PurchasedItemsBackensInfo>();
        DataKeeper.BackendInfo.purchased_items = array;
        DataKeeper.BackendInfo.user.user_id = jsonData["uid"].ToString();
        JsSpeeker.I._vk_name(DataKeeper.BackendInfo.user.user_id.ToString());
        string name = jsonData["NickName"].ToString().Replace("_", " ");
        JsSpeeker.I._vk_nick(name);
        if (name.Length < 4)
        {
            MainMenuController.I._newNamePanel.SetActive(true);
        }
    }

    private bool CheckIfGuestExist()
    {
        return PlayerPrefs.HasKey("plrguid8");
    }

    public void Get3FriendsDataFromVk(int attempt)
    {
        string requestString = "friends.get?user_id=" + VkApi.currentToken.user_id + "&count=3&fields=photo_200";
        vkapi.Call(requestString, OnGet5FriendsCompleted, new object[1] { attempt });
    }

    private void OnGet5FriendsCompleted(VkResponseRaw arg1, object[] arg2)
    {
        if (arg1.ei != null)
        {
            int num = (int)arg2[0];
            if (num < 5)
            {
                Get3FriendsDataFromVk(num + 1);
            }
            return;
        }
        Dictionary<string, object> dictionary = Json.Deserialize(arg1.text) as Dictionary<string, object>;
        Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["response"];
        List<object> list = (List<object>)dictionary2["items"];
        foreach (object item in list)
        {
            friends.Add(VKUser.Deserialize(item));
        }
        for (int i = 0; i < 3; i++)
        {
            FriendManager[] array = UnityEngine.Object.FindObjectsOfType<FriendManager>();
            Action<WWW, object[]> doWhenFinished = delegate (WWW www, object[] objarray)
            {
                FriendManager friendManager = (FriendManager)objarray[0];
            };
            array[i].t.text = friends[i].first_name;
            d.download(friends[i].photo_200, doWhenFinished, new object[1] { array[i] });
        }
    }

    public void GetVkUserInfo(int attempt)
    {
        Debug.Log("GET VK USER INFO");
        string requestString = "users.get?user_ids=" + VkApi.currentToken.user_id + "&fields=photo_200";
        vkapi.Call(requestString, OnGetVKUserInfo, new object[1] { attempt });
    }

    private void OnGetVKUserInfo(VkResponseRaw arg1, object[] arg2)
    {
        if (arg1.ei != null)
        {
            Debug.Log("OnGetVKUserInfo  ERRORRRR!!!!!!!!!!!!!!!!!!!!!" + arg1.ei.error_code + "--- " + arg1.ei.error_msg);
            int num = (int)arg2[0];
            if (num < 5)
            {
                GetVkUserInfo(num + 1);
            }
            StartCoroutine("CheckLogin");
            return;
        }

        Dictionary<string, object> dictionary = Json.Deserialize(arg1.text) as Dictionary<string, object>;
        List<object> list = (List<object>)dictionary["response"];

        Debug.Log(VKUser.Deserialize(list[0]).first_name + "---" + VKUser.Deserialize(list[0]).last_name + " --- " + VKUser.Deserialize(list[0]).id);

        JsSpeeker.I._vk_name(VKUser.Deserialize(list[0]).id.ToString());
        JsSpeeker.I._vk_nick(VKUser.Deserialize(list[0]).last_name + " " + VKUser.Deserialize(list[0]).first_name);

        StartCoroutine("CheckLogin");
        MenuConnectionViewController.I.InviteFriendsBtn.SetActive(true);

        Debug.Log("OnGetVKUserInfo!!!!!!!!!!!!!!!!!!!!!");
    }

    public void GetInviteVkUserFriends()
    {
        Debug.Log("call GetInviteVkUserFriends");
        string requestString = "apps.getFriendsList?extended=1&type=invite&count=" + 500 + "&offset=" + 0 + "&fields=photo_200";
        vkapi.Call(requestString, OnGetInviteVkUserFriendsCompleted, new object[0]);
    }

    private void OnGetInviteVkUserFriendsCompleted(VkResponseRaw arg1, object[] arg2)
    {
        if (arg1.ei != null)
        {
            Debug.Log(arg1.ei.error_code + "---" + arg1.ei.error_msg);
            return;
        }

        Dictionary<string, object> dictionary = Json.Deserialize(arg1.text) as Dictionary<string, object>;
        Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["response"];
        int num = Convert.ToInt32(dictionary2["count"]);
        List<object> list = (List<object>)dictionary2["items"];

        Debug.Log("OnGetInviteVkUserFriendsCompleted: " + arg1.text + "====" + list);
        List<VkPlayerInfo> list2 = new List<VkPlayerInfo>();

        foreach (object item in list)
        {
            VkPlayerInfo vkPlayerInfo = new VkPlayerInfo();
            vkPlayerInfo.first_name = VKUser.Deserialize(item).first_name;
            vkPlayerInfo.last_name = VKUser.Deserialize(item).last_name;
            vkPlayerInfo.uid = VKUser.Deserialize(item).id.ToString();
            list2.Add(vkPlayerInfo);
        }

        JsSpeeker.I.SetFriendsInfos(new List<VkPlayerInfo>(list2));
    }

    public void InviteFriends()
    {
        Debug.Log("Press invite friends!!!!!!!!!!!!!!!!!");
        MainMenuController.I.ShowInviteFriendsPanel();
    }

    public void InviteFriend(long vkUserId, object[] arg2)
    {
        string empty = string.Empty;
        string requestString = "apps.sendRequest?user_id=" + vkUserId + "&text=" + empty + "&type=invite";
        vkapi.Call(requestString, OnInviteFriendsComplete, arg2);
        Debug.Log("Call Invate friends " + vkUserId);
    }

    private void OnInviteFriendsComplete(VkResponseRaw arg1, object[] arg2)
    {
        Debug.Log("OnInviteFriendsComplete");
        if (arg1.ei != null)
        {
            Debug.Log(arg1.ei.error_code + " ---" + arg1.ei.error_msg);
        }
        else
        {
            Debug.Log(arg1.text);
        }
    }

    public void OpenGameGroup()
    {
        Application.OpenURL("https://vk.com/cubdayz");
    }

    public void OpenPlatformGroup()
    {
        Application.OpenURL("https://vk.com/vkgames");
    }

    private IEnumerator СheckInternetConnection(Action<bool> action)
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape))
        {
            return;
        }
        if (GameUIController.I != null)
        {
            if (GameUIController.I._characterMenuController.gameObject.activeSelf)
            {
                GameUIController.I.ShowCharacterMenu(false, CharacterMenuType.Menu);
            }
            else
            {
                GameUIController.I.ShowCharacterMenu(true, CharacterMenuType.Menu);
            }
        }
        if (MainMenuController.I != null)
        {
            StopAllCoroutines();
            Application.Quit();
        }
    }
}
