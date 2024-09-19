using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendPhotoDownloader : MonoBehaviour
{
	public Dictionary<string, Texture2D> FriendsPhotos = new Dictionary<string, Texture2D>();
	public bool allPhotoLoaded;
	public static FriendPhotoDownloader I;

	private WWW www;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	public void StartDownloadPhoto(string id,string photo)
	{
		StartCoroutine (_DownloadPhoto (id, photo));
	}
	private IEnumerator _DownloadPhoto(string id, string photo)
	{
		WWW www = new WWW (photo);
		yield return www;
		FriendsPhotos[id] = www.texture;
		OnAllPhotoDownloaded ();
	}


	public Texture2D GetPhotoById(string friendId)
	{
		Texture2D result = null;
		foreach (KeyValuePair<string, Texture2D> friendsPhoto in FriendsPhotos)
		{
			if (friendsPhoto.Key == friendId)
			{
				result = friendsPhoto.Value;
			}
		}
		return result;
	}

	public void OnAllPhotoDownloaded()
	{
		if (FriendsAndClanPanelView.I != null)
		{
			CubezStatic.SetFriendsPhotoInMainMenu(FriendsAndClanPanelView.I.GetAllFriendsItems());
		}
	}
}
