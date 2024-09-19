using UnityEngine;

public class FriendItemView : MonoBehaviour
{
	[SerializeField] tk2dTextMesh _name;
	[SerializeField] tk2dTextMesh _friendId;

	public GameObject play_button;
	public tk2dSprite _friendPhoto;
	public GameObject _photoAnchor;

	[SerializeField] tk2dUIToggleButton _friendSelectBtn;
	[SerializeField] BoxCollider _btnCollider;

	public Color32 onlineColor;
	public Color32 offlineColor;
	public string id;
	public string photon_url_max;
	public bool isOnline;
	public bool isPhotoLoaded;

	private void Awake()
	{
		onlineColor = new Color32(25, 205, 25, byte.MaxValue);
		offlineColor = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
	}

	public void SetBaseInfo(VkPlayerInfo friendInfo)
	{
		_name.text = friendInfo.first_name + " " + friendInfo.last_name;
		_friendId.text = friendInfo.uid;
		id = friendInfo.uid;
	}

	public bool GetToogle()
	{
		return _friendSelectBtn.IsOn;
	}

	public void SetToogle(bool state)
	{
		_friendSelectBtn.IsOn = false;
	}

	public string GetFriendId()
	{
		return _friendId.text;
	}

	public void ClickFriendItem()
	{
		if (_friendSelectBtn.IsOn)
		{
			FriendsListView.I.SetUpSelectedFriendsCount();
		}
		else
		{
			FriendsListView.I.SetDownSelectedFriendsCount();
		}
	}

	public void AvtivateBtn()
	{
		_btnCollider.enabled = true;
	}

	public void DisavtiveBtn()
	{
		_btnCollider.enabled = false;
	}

	public void SetOnlineStatus()
	{
		isOnline = true;
		_name.color = onlineColor;
		if ((bool)play_button)
		{
			FriendInfo thisFriend = GetThisFriend();
			if (thisFriend.IsInRoom && thisFriend.Room.Contains("#"))
			{
				play_button.SetActive(true);
			}
		}
	}

	public void SetOfflineStatus()
	{
		isOnline = false;
		_name.color = offlineColor;
		if ((bool)play_button)
		{
			play_button.SetActive(false);
		}
	}

	public FriendInfo GetThisFriend()
	{
		return PhotonNetwork.Friends.Find((FriendInfo fr1) => fr1.UserId == GetFriendId());
	}

	public void OnGoToFriendServer()
	{
		if (MainMenuController.I != null)
		{
			MainMenuController.I.HideMainMenu();
		}
		FriendInfo thisFriend = GetThisFriend();
		string room = thisFriend.Room;
		if (thisFriend.Room.Contains("#"))
		{
			MenuConnectionViewController.I._joinRoomName = room;
			MenuConnectionViewController.I._isRandomRoom = false;
			MenuConnectionViewController.I.ShowWaitingScreen(true);
			DataKeeper.GameType = GameType.Multiplayer;
			MenuConnectionViewController.I._connectionController.JoinRoom(room, false);
		}
		else
		{
			Debug.LogError("friend not in multiplayer server");
		}
	}
}
