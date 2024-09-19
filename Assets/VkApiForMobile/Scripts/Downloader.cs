using UnityEngine;
using System.Collections;
using System;

namespace com.playGenesis.VkUnityPlugin {

	public class Downloader : MonoBehaviour 
	{
		public void download(string murl,Action<WWW,object[]> doWhenFinished,object[]parameters)
		{
			StartCoroutine(_download(murl,doWhenFinished,parameters));
		}

		private IEnumerator _download(string url,Action<WWW,object[]> doWhenFinished,object[] parameters)
		{
			string _request = url;
			WWW www = new WWW (System.Uri.EscapeUriString (_request));
			yield return www;
			if(doWhenFinished!=null)
				doWhenFinished(www,parameters);
		}
	}
}