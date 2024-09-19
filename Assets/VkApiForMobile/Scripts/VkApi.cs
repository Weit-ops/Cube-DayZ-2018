using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
//using Pathfinding.Serialization.JsonFx;
//using Newtonsoft.Json;
//using JsonFx.Json;
using System.Runtime.InteropServices;
using PlayGenesisVkSdk;

namespace com.playGenesis.VkUnityPlugin 
{
	[RequireComponent(typeof(VkSettings))]
	public class VkApi:MonoBehaviour
	{

		#if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern void _VkAuthorization(string authUrl);

		[DllImport("__Internal")]
		private static extern void _doLogOutUser();

		public void  VkAuthorizationCall(string url){
			_VkAuthorization (url);

		}
		public void Login()
		{
			var scope=string.Join(",",vsetts.scope.ToArray());
			string[] prms=new string[4];
			prms[0]= scope;
			prms[1]=vsetts.VkAppId.ToString();
			prms[2]=vsetts.forceOAtuh.ToString();
			prms [3] =vsetts.revoke.ToString ();
			var data = string.Join (" ", prms);

			var url = "https://oauth.vk.com/authorize?client_id=" + prms[1] +
				"&scope=" + scope +
				"&redirect_uri=https://oauth.vk.com/blank.html&display=mobile" +
				"&forceOAtuh="+prms[2]+
				"&revokeAccess="+prms[3]+
				"&v="+vsetts.apiVersion +
				"&response_type=token";
			_VkAuthorization (url);
		}
		public void Logout()
		{
			onLoggedOut ();
			_doLogOutUser ();
		}
		public void doLogOutUser()
		{
			_doLogOutUser ();
		}
		#endif
		#if  UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaObject jo;
		public void Login()
		{

			var scope=string.Join("#",vsetts.scope.ToArray());
			string[] prms=new string[4];
			prms[0]= scope;
			prms[1]=vsetts.VkAppId.ToString();
			prms[2]=vsetts.forceOAtuh.ToString();
			prms [3] =vsetts.revoke.ToString ();
			var data = string.Join (" ", prms);
			jo = new AndroidJavaObject ("com.playgenesis.vkunityplugin.Initializer"); 
			jo.Call ("Init", data);
		}

		public void Logout()
		{
			using (AndroidJavaObject jo = new AndroidJavaObject ("com.playgenesis.vkunityplugin.Initializer")) 
			{
				onLoggedOut();
				jo.Call ("Logout",vsetts.VkAppId.ToString());
			}
		}
		#endif
		#if (!UNITY_EDITOR)
		private Registrator r= new Registrator();
		public void Login()
		{

			String appId=vsetts.VkAppId.ToString();
			bool forceOAuth=vsetts.forceOAtuh;
			bool revoke=vsetts.revoke;
			var scope=new List<string>();

			Registrator r= new Registrator();
			r.VkInit(vsetts.scope,appId,forceOAuth);
		}
		public void Logout()
		{
			onLoggedOut();
			r.Logout();
		}
		#endif
		#if UNITY_EDITOR
		public void Login()
		{
			String rr=access_token_manual+"#"+DateTime.Now.ToString()+"#"+999999+"#"+user_id_manual;
			GameObject.FindObjectOfType<MessageHandler>().ReceiveNewTokenMessage(rr);

		}
		public void Logout()
		{
			onLoggedOut();
		}
		#endif



		public bool isUserLoggedIn;
		public static TokenInfo currentToken;
		public static VkApi VkApiInstance;
		public static Downloader downloader;
		public VkSettings vsetts;
		private string vkrequestUrlBase="https://api.vk.com/method/";	

		#region for testing
		 bool UseManualData;
		 public  string access_token_manual;
		 public  string user_id_manual;
		#endregion


		#region Events
		public event EventHandler<ErrorInfo> AccessDenied;
		public event EventHandler<TokenInfo> ReceivedNewToken;
		public event Action LoggedIn;
		public event Action LoggedOut;
		#endregion

		#region EventTriggers
		public void onReceiveNewToken(TokenInfo e)
		{
			currentToken = e;

			PlayerPrefs.SetString ("access_token", e.access_token);
			PlayerPrefs.SetInt ("expires_in", e.expires_in);
			PlayerPrefs.SetString ("tokenRecievedTime", e.tokenRecievedTime.ToString());
			PlayerPrefs.SetString ("user_id", e.user_id);
			if(ReceivedNewToken!=null)
				ReceivedNewToken (this,e);
			onLoggedIn ();
			Debug.Log("New token is"+e.access_token);
		}
		public void onLoggedIn()
		{
			isUserLoggedIn = true;
			if(LoggedIn!=null)
			 LoggedIn();
		}
		public void onLoggedOut()
		{
			isUserLoggedIn = false;
			if(LoggedOut!=null)
				LoggedOut();
			Debug.Log ("LoggedOut");
			resetToken();

		}
		public void onAccessDenied (ErrorInfo e)
		{
			if (AccessDenied != null) {
				AccessDenied (this, e);

			}
			Debug.Log("Access denied1 "+e.error_description);
		}
		#endregion
		public  string getEditorToken()
		{
			return access_token_manual;
		}

		public void SetUpEditor()
		{
			var authUrl=vsetts.auth_url;
			string[] firstsplit=authUrl.Split('#');
			string[] secondsplit=firstsplit[1].Split('&');
			
			var tokeninfo = new Dictionary<string,string> ();
			
			foreach (var secondsplitemevent in secondsplit)
			{
				string[] thirdsplit=secondsplitemevent.Split('=');
				tokeninfo.Add(thirdsplit[0],thirdsplit[1]);
			}

			access_token_manual=tokeninfo ["access_token"];;
			user_id_manual=tokeninfo ["user_id"];
		}
		void Awake()
		{
			DontDestroyOnLoad(transform.gameObject);
			if (VkApiInstance == null)
				VkApiInstance = this;
			if(downloader==null)
			downloader = GetComponent<Downloader> ();
			vsetts=GetComponent<VkSettings>();
		}

		void Start()
		{
#if !UNITY_EDITOR
			UseManualData=false;
#else
			UseManualData=true;
#endif
			if (!UseManualData) 
			{
				currentToken = new TokenInfo{
					access_token=PlayerPrefs.GetString ("access_token", ""),
					expires_in=PlayerPrefs.GetInt ("expires_in", 0),
					user_id=PlayerPrefs.GetString ("user_id", "")
				};
				DateTime.TryParse (PlayerPrefs.GetString ("tokenRecievedTime", "1/1/1990"), out currentToken.tokenRecievedTime);
			} else
			{
				SetUpEditor();
				currentToken = new TokenInfo
				{
					access_token=access_token_manual,
					tokenRecievedTime=DateTime.Now,
					expires_in=999999,
					user_id=user_id_manual
				};
			}

			
			if (isValidToken (currentToken))
			{
				isUserLoggedIn = true;
			} 
			else 
			{
				isUserLoggedIn = false;
			}
		}

		public bool isValidToken(TokenInfo ti)
		{
			var isvalid=currentToken.tokenRecievedTime.AddSeconds(currentToken.expires_in)>DateTime.Now?true:false;
			return isvalid;
		} 
		public int TokenValidFor()
		{
			return (int)(currentToken.tokenRecievedTime.AddSeconds (currentToken.expires_in) - DateTime.Now).TotalSeconds;
		}

		public void resetToken()
		{
			DateTime.TryParse("1/1/1990",out currentToken.tokenRecievedTime);
			currentToken.access_token = "";
			currentToken.user_id      = "";
			currentToken.expires_in   = 0;
			PlayerPrefs.SetString ("access_token", "");
			PlayerPrefs.SetString("tokenRecievedTime","1/1/1990");
			PlayerPrefs.SetInt ("expires_in", 0);
			PlayerPrefs.SetString ("user_id", "");
			Debug.Log ("Token has been reset");
			
		}
		public static Error ParseVkError(string resp)
		{
			if(string.IsNullOrEmpty(resp))
			{
				Error ei=new Error();
				ei.error_code="404";
				ei.error_msg="Non connection";
				//ei.errorType=ErrorType.VKError;
				return ei;

			}
			//ErrorContainer e = JsonConvert.DeserializeObject<ErrorContainer>(resp);//js jr.Read<ErrorContainer> (resp);
			Error e =Error.Deserialize(resp);//js jr.Read<ErrorContainer> (resp);

			if (e!=null && !String.IsNullOrEmpty(e.error_code))  
			{
				Debug.LogError(e.error_code+" "+e.error_msg);
				Error ei=new Error();
				ei.error_code=e.error_code;
				ei.error_msg=e.error_msg;
				//ei.errorType=ErrorType.VKError;
				return ei;
			}
			return null;
		}

		
		private string GenerateFullHttpReqString(string request)
		{
			string _request = request;
			//adding access token and api version info to request
			_request=_request+"&v="+vsetts.apiVersion+"&access_token="+currentToken.access_token;
			_request.Replace("#","%23");
			return vkrequestUrlBase + System.Uri.EscapeUriString (_request);
		}
		private WWW GenerateWWWForm(string requestString,Action<VkResponseRaw,object[]> CallbackFunction,object[]data=null)
		{
			string _request = requestString;
			WWWForm form = new WWWForm();
			form.AddBinaryData("file", (byte[])data[0], (string)data[1], (string)data[2]);
			return new WWW (System.Uri.EscapeUriString (_request),form);
		}
		
		private void HandleTokenExpired(string fullHttpRequestString, Action<VkResponseRaw,object[]> CallbackFunction,object[]data=null)
		{
				Debug.Log("Invalid token. Are you logged in?");
				VkResponseRaw vkargs = new VkResponseRaw ();
				vkargs.request = fullHttpRequestString;
				vkargs.text = "";
				
				vkargs.ei=new Error{error_code="401",error_msg="invalid token" };
				if(CallbackFunction!=null)
					CallbackFunction (vkargs,data);

		}
		private void HandleWWWError( WWW www,string requestString,Action<VkResponseRaw,object[]> CallbackFunction,object[]data=null){
			var vkerror=new Error()
			{
				error_code="404",
				error_msg=www.error
			};
			
			VkResponseRaw vkargs = new VkResponseRaw ()
			{request = requestString,
				text = www.text};
			
			vkargs.ei=vkerror;
			
			if(CallbackFunction!=null)
				CallbackFunction (vkargs,data);
		}
		private void HandleVKError( WWW www,string requestString,Action<VkResponseRaw,object[]> CallbackFunction,object[]data=null){
			var vkerror=ParseVkError(www.text);
			VkResponseRaw vkargs = new VkResponseRaw ()
			{request = requestString,
				text = www.text};
			
			vkargs.ei=vkerror;
			
			if(CallbackFunction!=null)
				CallbackFunction (vkargs,data);
			
		}
		private void HandleNoError( WWW www,string requestString,Action<VkResponseRaw,object[]> CallbackFunction,object[]data=null){
			
			VkResponseRaw vkargs = new VkResponseRaw ();
			vkargs.request = requestString;
			vkargs.text = www.text;
			if(CallbackFunction!=null)
				CallbackFunction (vkargs,data);
		}
		private void HandleResponse(WWW www,string requestString,Action<VkResponseRaw,object[]> CallbackFunction,object[]data=null)
		{
			if (!string.IsNullOrEmpty (www.error)) {
				HandleWWWError(www,requestString,CallbackFunction,data);
			}
			
			if (string.IsNullOrEmpty (www.error) && ParseVkError (www.text) != null) {
				HandleVKError(www,requestString,CallbackFunction,data);
			}
			
			if (String.IsNullOrEmpty (www.error)&& ParseVkError (www.text)==null) 
			{
				HandleNoError(www,requestString,CallbackFunction,data);
				
			} 
		}
		public void Call( string requestString, Action<VkResponseRaw,object[]> callbackFunction=null,object[]data=null)
		{
			StartCoroutine (_Call (requestString, callbackFunction,data));
		}
		private IEnumerator _Call( string requestString, Action<VkResponseRaw,object[]> CallbackFunction=null,object[]data=null)
		{
			var fullhttprequeststring = GenerateFullHttpReqString (requestString);

			if (isValidToken(currentToken)) 
			{
				WWW www = new WWW (fullhttprequeststring);
				yield return www;
				HandleResponse(www,fullhttprequeststring,CallbackFunction,data);
				
			} else 
			{
				HandleTokenExpired(fullhttprequeststring,CallbackFunction,data);
			}
		}
		public void UploadToserverCall( string requestString,Action<VkResponseRaw,object[]> CallbackFunction,object[]data=null)
		{
			StartCoroutine(_UploadToserverCall(requestString,CallbackFunction,data));
		}
		
		private IEnumerator _UploadToserverCall( string requestString,Action<VkResponseRaw,object[]> CallbackFunction,object[]data=null)
		{
			var www=GenerateWWWForm (requestString, CallbackFunction, data);
			yield return www;
			HandleResponse (www, requestString, CallbackFunction, data);	
		}

	}

}
