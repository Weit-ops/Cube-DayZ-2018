using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomListType
{
	All = 0,
	Private = 1
}

public class MenuRoomListView : MonoBehaviour
{
	private const int ScrollSpeed = 5;

	[SerializeField] MenuConnectionViewController _connectionViewController;
	[SerializeField] GameObject _roomView;
	[SerializeField] float _roomViewSize;
	[SerializeField] float _offsetBetweenRoomView;
	[SerializeField] GameObject _randomJoinBlock;
	[SerializeField] GameObject _allRoomsContent;
	[SerializeField] tk2dUIScrollableArea _scrollableArea;
	[SerializeField] tk2dUIToggleButtonGroup _roomsTypeGroup;

	private RoomListType _currentType;
	private List<RoomInfo> _roomInfos = new List<RoomInfo>();
	private Transform _contentChild;
	private List<MenuRoomView> _allRoomsViews = new List<MenuRoomView>();
	private bool _roomListUpdated;
	private bool _isInitialized;

	public void Social_AddWallPost_InviteToPrivate()
	{
		JsSpeeker.I.WallPostSomething("invite_friends_to_private", "Cube Day Z: Заходи на мой приватный сервер!");
	}

	private void FillAllRoomsViews()
	{
		List<GameRoomInfo> gameRoomsInfos = RoomsService.Instance.GameRoomsInfos;
		for (int i = 0; i < gameRoomsInfos.Count; i++)
		{
			MenuRoomView component = Object.Instantiate(_roomView).GetComponent<MenuRoomView>();
			component.transform.parent = _allRoomsContent.transform;
			component.transform.localPosition = -new Vector3(0f, (_roomViewSize + _offsetBetweenRoomView) * (float)i);
			component.SetBaseInfo(gameRoomsInfos[i]);
			_allRoomsViews.Add(component);
		}
		_scrollableArea.ContentLength = _roomViewSize + (_roomViewSize + _offsetBetweenRoomView) * (float)(_allRoomsViews.Count - 1) + _offsetBetweenRoomView * 2f;
		int num = _allRoomsViews.Count / 5;
		_scrollableArea.scrollBar.buttonUpDownScrollDistance = 1f / (float)num;
		_allRoomsContent.SetActive(_roomListUpdated && _currentType == RoomListType.All);
		UpdateRoomList();
		_isInitialized = true;
	}

	private void OnEnable()
	{
		if (MenuConnectionViewController.I != null)
		{
			MenuConnectionViewController.I.IfEmptyServersList();
		}
		if (!_isInitialized)
		{
			FillAllRoomsViews();
		}
		_roomsTypeGroup.SelectedIndex = 0;
		StartCoroutine("UpdateInfos");
	}

	private void OnDisable()
	{
		StopCoroutine(" UpdateInfos");
	}

	private IEnumerator UpdateInfos()
	{
		while (true)
		{
			PhotonNetwork.GetRoomList();
			UpdateFriends();
			yield return new WaitForSeconds(1f);
		}
	}

	private void OnChangeType(tk2dUIToggleButtonGroup roomTypesGroup)
	{
		int selectedIndex = roomTypesGroup.SelectedIndex;
		if (selectedIndex == 1)
		{
			_currentType = RoomListType.Private;
		}
		else
		{
			_currentType = RoomListType.All;
		}
		UpdateRoomList();
	}

	private void UpdateRoomList()
	{
		_roomInfos.Clear();
		if (MenuConnectionController.I.RoomInfos != null && MenuConnectionController.I.RoomInfos.Count > 0)
		{
			switch (_currentType)
			{
			case RoomListType.Private:
			{
				List<RoomInfo> list2 = MenuConnectionController.I.RoomInfos.FindAll((RoomInfo room) => room.CustomProperties.ContainsKey("private") && (bool)room.CustomProperties["private"] && room.CustomProperties.ContainsKey("creatorId") && PhotonNetwork.Friends != null && PhotonNetwork.Friends.Find((FriendInfo friend) => friend.UserId == (string)room.CustomProperties["creatorId"]) != null);
				if (list2.Count > 0)
				{
					_roomInfos.AddRange(list2);
				}
				break;
			}
			case RoomListType.All:
			{
				List<RoomInfo> list = MenuConnectionController.I.RoomInfos.FindAll((RoomInfo room) => !room.CustomProperties.ContainsKey("private") || !(bool)room.CustomProperties["private"]);
				if (list.Count > 0)
				{
					_roomInfos.AddRange(list);
				}
				break;
			}
			}
		}
		if (MenuConnectionController.I == null)
		{
			Debug.LogError("MenuConnectionController.I == null");
		}
		if (MenuConnectionController.I.RoomInfos != null && MenuConnectionController.I.RoomInfos.Count > 0)
		{
			UpdateView();
		}
	}

	private void UpdateFriends()
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

	private void OnGUI()
	{
		GUILayout.Label("\t\t\t\t\t\t\t\t\t\t\tconnection state0: " + PhotonNetwork.connectionState);
	}

	private void UpdateView()
	{
		switch (_currentType)
		{
		case RoomListType.All:
		{
			_allRoomsContent.SetActive(true);
			if (_contentChild != null)
			{
				Object.Destroy(_contentChild.gameObject);
			}
			for (int i = 0; i < _allRoomsViews.Count; i++)
			{
				bool hasFriend3 = false;
				RoomInfo photonRoom = MenuConnectionController.I.RoomInfos.Find((RoomInfo r) => r.Name == _allRoomsViews[i].RoomName);
				if (photonRoom != null && PhotonNetwork.Friends != null)
				{
					hasFriend3 = PhotonNetwork.Friends.Find((FriendInfo f) => f.IsOnline && f.Room == photonRoom.Name) != null;
				}
				int currentPlayerCount = ((photonRoom != null) ? photonRoom.PlayerCount : 0);
				_allRoomsViews[i].SetInfo(currentPlayerCount, hasFriend3, true, _connectionViewController.ConnectToRoom);
			}
			_scrollableArea.ContentLength = _roomViewSize + (_roomViewSize + _offsetBetweenRoomView) * (float)(_allRoomsViews.Count - 1) + _offsetBetweenRoomView * 2f;
			int num2 = ((_allRoomsViews.Count <= 0) ? 1 : (_allRoomsViews.Count / 5));
			_scrollableArea.scrollBar.buttonUpDownScrollDistance = 1f / (float)num2;
			break;
		}
		case RoomListType.Private:
		{
			_allRoomsContent.SetActive(false);
			if (_contentChild != null)
			{
				Object.Destroy(_contentChild.gameObject);
			}
			_contentChild = new GameObject("Content").transform;
			_contentChild.transform.parent = _scrollableArea.contentContainer.transform;
			_contentChild.transform.localPosition = Vector3.zero;
			List<GameRoomInfo> myPrivateRooms = RoomsService.Instance.MyPrivateRooms;
			for (int k = 0; k < myPrivateRooms.Count; k++)
			{
				RoomInfo myRoomInfo = MenuConnectionController.I.RoomInfos.Find((RoomInfo r) => r.Name == myPrivateRooms[k].Name);
				if (myRoomInfo != null)
				{
					MenuRoomView component = Object.Instantiate(_roomView).GetComponent<MenuRoomView>();
					component.transform.parent = _contentChild;
					component.transform.localPosition = -new Vector3(0f, (_roomViewSize + _offsetBetweenRoomView) * (float)k);
					bool hasFriend = false;
					if (PhotonNetwork.Friends != null)
					{
						hasFriend = PhotonNetwork.Friends.Find((FriendInfo f) => f.IsOnline && f.Room == myRoomInfo.Name) != null;
					}
					component.SetInfo(myRoomInfo, hasFriend, true, _connectionViewController.ConnectToRoom);
				}
				else
				{
					MenuRoomView component2 = Object.Instantiate(_roomView).GetComponent<MenuRoomView>();
					component2.transform.parent = _contentChild;
					component2.transform.localPosition = -new Vector3(0f, (_roomViewSize + _offsetBetweenRoomView) * (float)k);
					component2.SetBaseInfo(myPrivateRooms[k]);
					component2.SetInfo(0, false, false, _connectionViewController.ConnectToRoom);
				}
			}
			int j;
			for (j = 0; j < _roomInfos.Count; j++)
			{
				MenuRoomView component3 = Object.Instantiate(_roomView).GetComponent<MenuRoomView>();
				component3.transform.parent = _contentChild;
				component3.transform.localPosition = -new Vector3(0f, (_roomViewSize + _offsetBetweenRoomView) * (float)(j + myPrivateRooms.Count));
				bool hasFriend2 = false;
				if (PhotonNetwork.Friends != null)
				{
					hasFriend2 = PhotonNetwork.Friends.Find((FriendInfo f) => f.IsOnline && f.Room == _roomInfos[j].Name) != null;
				}
				component3.SetInfo(_roomInfos[j], hasFriend2, false, _connectionViewController.ConnectToRoom);
			}
			_scrollableArea.ContentLength = _roomViewSize + (_roomViewSize + _offsetBetweenRoomView) * (float)(_roomInfos.Count + myPrivateRooms.Count - 1) + _offsetBetweenRoomView * 2f;
			int num = ((_roomInfos.Count + myPrivateRooms.Count <= 0) ? 1 : ((_roomInfos.Count + myPrivateRooms.Count) / 5));
			_scrollableArea.scrollBar.buttonUpDownScrollDistance = 1f / (float)num;
			break;
		}
		}
		_randomJoinBlock.SetActive(_currentType == RoomListType.All);
	}

	private void OnReceivedRoomListUpdate()
	{
		_roomListUpdated = true;
		UpdateRoomList();
	}

	private void OnUpdatedFriendList()
	{
		UpdateRoomList();
	}
}
