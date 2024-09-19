using System.Collections.Generic;
using UnityEngine;

public class CubezStatic : MonoBehaviour
{
	private float _roomViewSize;

	[SerializeField]
	private float _offsetBetweenRoomView;

	[SerializeField]
	private tk2dUIScrollableArea _scrollableArea;

	public static List<FriendItemView> FillFriendsList(GameObject _friendsItemView, GameObject _parentObj, float _roomViewSize, tk2dUIScrollableArea _scrollableArea, int ScrollSpeed, float _offsetBetweenRoomView)
	{
		List<FriendItemView> list = new List<FriendItemView>();
		Debug.Log(" FillFriendsList!!!!!!!!!!!!!!!!!!!1");
		List<VkPlayerInfo> friendsInfos = JsSpeeker.FriendsInfos;
		for (int i = 0; i < friendsInfos.Count; i++)
		{
			FriendItemView component = Object.Instantiate(_friendsItemView).GetComponent<FriendItemView>();
			component.transform.parent = _parentObj.transform;
			component.transform.localScale = new Vector3(1f, 1f, 1f);
			component.transform.localPosition = -new Vector3(0f, (_roomViewSize + _offsetBetweenRoomView) * (float)i);
			component.photon_url_max = friendsInfos [i].photo_max_url;
			component.SetBaseInfo(friendsInfos[i]);
			FriendPhotoDownloader.I.StartDownloadPhoto (friendsInfos[i].uid,friendsInfos[i].photo_max_url);
			list.Add(component);
		}
		_scrollableArea.ContentLength = _roomViewSize + (_roomViewSize + _offsetBetweenRoomView) * (float)(friendsInfos.Count - 1) + _offsetBetweenRoomView * 2f;
		int num = friendsInfos.Count / ScrollSpeed;
		_scrollableArea.scrollBar.buttonUpDownScrollDistance = 1f / (float)num;
		_parentObj.SetActive(true);

		return list;
	}

	public static void UpdateFriendOnlineStatus(List<FriendItemView> FillFriendsList)
	{
		if (PhotonNetwork.connectionState != ConnectionState.Connected) {
			return;
		}
		foreach (FriendItemView FillFriends in FillFriendsList)
		{
			foreach (FriendInfo friend in PhotonNetwork.Friends)
			{
				if (friend.UserId == FillFriends.id)
				{
					if (friend.IsOnline)
					{
						FillFriends.SetOnlineStatus();
					}
					else
					{
						FillFriends.SetOfflineStatus();
					}
				}
			}
		}
	}



	public static void UpdatePhotonFriends()
	{
		if (JsSpeeker.FriendsInfos == null)
		{
			return;
		}
		string[] array = new string[JsSpeeker.FriendsInfos.Count];
		for (int i = 0; i < JsSpeeker.FriendsInfos.Count; i++)
		{
			if (JsSpeeker.FriendsInfos[i] != null)
			{
				array[i] = JsSpeeker.FriendsInfos[i].uid;
			}
		}
		PhotonNetwork.FindFriends(array);
	}

	public static void SetFriendsPhotoInMainMenu(List<FriendItemView> FillFriendsList)
	{
		foreach (FriendItemView FillFriends in FillFriendsList)
		{
			if (!FillFriends.isPhotoLoaded)
			{
				Rect region = new Rect(0f, 0f, 50f, 50f);
				tk2dSpriteCollectionSize size = tk2dSpriteCollectionSize.ForTk2dCamera();
				Texture2D photoById = FriendPhotoDownloader.I.GetPhotoById(FillFriends.id);
				if (!(photoById == null))
				{
					GameObject gameObject = tk2dSprite.CreateFromTexture(photoById, size, region, new Vector2(0f, 0f));
					gameObject.layer = 27;
					gameObject.transform.parent = FillFriends._photoAnchor.transform;
					gameObject.transform.localScale = new Vector3(0.012f, 0.012f, 0.012f);
					gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
					Object.Destroy(FillFriends._friendPhoto);
					FillFriends.isPhotoLoaded = true;
				}
			}
		}
	}
}
