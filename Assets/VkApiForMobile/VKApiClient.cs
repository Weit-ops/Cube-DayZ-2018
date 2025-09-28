using UnityEngine;
using LitJson;


public class VKApiClient : MonoBehaviour {

	public string first_name = "Leo";
	public string last_name = "Tesla";
	public string uid = "2018940123";
	public static VKApiClient I;
	private void Awake()
	{
		I = this;
	}
	public void Start()
	{
		Application.ExternalCall("ReceiveVKInfo");
	}
	public void OnGetVKData(string data)
	{
		JsonData litjson = JsonMapper.ToObject(data);
		first_name = litjson["first_name"].ToString();
		last_name = litjson["last_name"].ToString();
		uid = litjson["id"].ToString();
	}
}
