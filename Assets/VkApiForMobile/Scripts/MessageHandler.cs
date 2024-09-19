using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Reflection;

namespace com.playGenesis.VkUnityPlugin {

	public class MessageHandler : MonoBehaviour 
	{
		public Dictionary<string,string> tokeninfo;
		public Dictionary<string,string> errorinfo;

	
		private VkApi vkapi;


		void Awake()
		{
			vkapi = GetComponent<VkApi> ();
		}

		public void ReceiveNewTokenMessage(string message)
		{
			//#if UNITY_ANDROID || UNITY_EDITOR||UNITY_WP8||UNITY_IOS
			string[] token = message.Split('#');
			TokenInfo ti=new TokenInfo();
			ti.access_token = token [0];

			ti.tokenRecievedTime = DateTime.Now;

			ti.expires_in = int.Parse(token [2])==0?999999:int.Parse(token [2]);

			ti.user_id = token [3];
			vkapi.onReceiveNewToken (ti);
			//#endif


		}
		public void AccessDeniedMessage(string errormessage)
		{

			string[] error=errormessage.Split('#');
			ErrorInfo ei=new ErrorInfo();
			ei.errorcode = error [0];
			ei.error_description = error [1];
			vkapi.onAccessDenied (ei);


		}




	}
}