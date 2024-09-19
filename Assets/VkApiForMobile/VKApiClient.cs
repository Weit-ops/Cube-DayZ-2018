using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;
using LitJson;

[Serializable]
public class Vk_Data
{
	public string first_name;
	public string last_name;
	public string userId;
	public string token;
}


public class VKApiClient : MonoBehaviour {

	public string application_id;
	public string secure_secret;
    public string phpScriptUrl = "https://static2-speedybyte.ru/";
	public Vk_Data user;
	public bool IsAuth;
	private int countLoopsWait;
	public static VKApiClient I;
	private void Awake()
	{
		I = this;
	}

    void Start()
    {
		user = new Vk_Data ();
    }
    public void Authenticate()
    {
        // Перенаправление на страницу авторизации
        Application.OpenURL("https://oauth.vk.com/authorize?client_id=51906143&redirect_uri=" + phpScriptUrl + "vk_auth.php" + "&display=page&scope=friends&response_type=code");
		StartCoroutine (StartWaitingLoopBack ());
	}
	public IEnumerator StartWaitingLoopBack()
	{
		while (true)
		{
		WWW www = new WWW (phpScriptUrl + "vk_auth_get.php");
		countLoopsWait++;
		Debug.Log ("waiting loop vk auth - " + countLoopsWait);
		yield return www;
		string response = www.text;
		if (response == "code_not_find") 
		{
			Debug.Log ("code not find, starting new wait...");
		} 
		else
		{
				StartCoroutine(OnAuthenticateResponse (response));
			yield break;
		}
			yield return new WaitForSeconds(1.5f);
		}
	}
	private IEnumerator OnAuthenticateResponse(string data)
	{
		WWW newWWW = new WWW ("https://oauth.vk.com/access_token?client_id=" + application_id + "&client_secret=" + secure_secret
		             + "&redirect_uri=" + phpScriptUrl + "vk_auth.php" + "&code=" + data);
		yield return newWWW;
		JsonData js = JsonMapper.ToObject (newWWW.text);
		string userId = js ["user_id"].ToString ();
		string token = js ["access_token"].ToString ();
		user.token = token;
		StartCoroutine (LoadUserProfile(userId,token));

	}
	private IEnumerator LoadUserProfile(string userId,string token)
	{
		WWW newWWw = new WWW ("https://api.vk.com/method/users.get?user_ids=" + userId + "&fields=photo_max_orig&access_token=" + token + "&v=5.131" );
		yield return newWWw;
		Debug.Log (newWWw.text);
		JsonData jsd = JsonMapper.ToObject (newWWw.text.Substring(12));
		user.first_name = jsd [0]["first_name"].ToString ();
		user.last_name = jsd [0]["last_name"].ToString ();
		user.userId = userId;
		Controller.I.OnVkAuthSuccesfull ();
		IsAuth = true;
		StopCoroutine ("StartWaitingLoopBack");
		StartCoroutine (LoadFriendsProfile ()); 
	}
	public IEnumerator LoadFriendsProfile()
	{
		WWW newWWw = new WWW ("https://api.vk.com/method/friends.get?user_ids=" + user.userId + "&fields=photo_50&access_token=" + user.token + "&v=5.131" );
		yield return newWWw;
		Debug.Log (newWWw.text.Substring(12));
		JsonData jsd = JsonMapper.ToObject (newWWw.text.Substring(12));
		List<VkPlayerInfo> vkFriend = new List<VkPlayerInfo> ();
		for (int i = 0; i < int.Parse(jsd["count"].ToString()); i++) {
			VkPlayerInfo friendInst = new VkPlayerInfo ();
			friendInst.uid = jsd["items"][i]["id"].ToString();
			friendInst.first_name = jsd["items"] [i]["first_name"].ToString();
			friendInst.last_name = jsd ["items"][i]["last_name"].ToString();
			friendInst.photo_max_url = jsd ["items"] [i] ["photo_50"].ToString ();
			vkFriend.Add (friendInst);
		}
		JsSpeeker.I.SetFriendsInfos (vkFriend);
		FriendsAndClanPanelView.I.FillFriendsList ();
	}
  
}
