using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsAndClanPanelView : MonoBehaviour
{
	private const int ScrollSpeed = 5;

	public GameObject thisPanel;

	[SerializeField] GameObject _friendsItem;
	[SerializeField] GameObject _friendsParentObj;
	[SerializeField] float _roomViewSize;
	[SerializeField] float _offsetBetweenRoomView;
	[SerializeField] tk2dUIScrollableArea _scrollableArea;

	private List<FriendItemView> _allFriendItems = new List<FriendItemView>();

	[SerializeField] GameObject _invateFriendBtn;

	public static FriendsAndClanPanelView I;

	private bool _isFill;

	public List<FriendItemView> GetAllFriendsItems()
	{
		return _allFriendItems;
	}

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	public void ActivateInvateFriendBtn(bool flag)
	{
		_invateFriendBtn.SetActive(flag);
	}

	public IEnumerator Start()
	{
		if (JsSpeeker.FriendsInfos != null)
		{
			if (JsSpeeker.FriendsInfos.Count > 0)
			{
				yield return new WaitForSeconds(0.5f);
				ActivateFriendPanel(true);
			}
		}
		else
		{
			yield return new WaitForSeconds(5f);
			ActivateFriendPanel(true);
		}
	}

	public void ActivateFriendPanel(bool state)
	{
		if (state && !_isFill && _allFriendItems.Count == 0)
		{
			FillFriendsList();
			_isFill = true;
		}
	}

	public void FillFriendsList()
	{
		if (JsSpeeker.FriendsInfos != null)
		{
			if (JsSpeeker.FriendsInfos.Count > 0)
			{
				thisPanel.SetActive(true);
				ActivateInvateFriendBtn(false);
				_allFriendItems = CubezStatic.FillFriendsList(_friendsItem, _friendsParentObj, _roomViewSize, _scrollableArea, 5, _offsetBetweenRoomView);
				_friendsParentObj.SetActive(true);
			}
			else if (JsSpeeker.FriendsInfos.Count <= 0)
			{
				ActivateInvateFriendBtn(true);
			}
		}
		else
		{

			ActivateInvateFriendBtn(true);
		}

	}
	public void ClearFriendsList()
	{
		foreach (FriendItemView vv in FindObjectsOfType<FriendItemView>()) {
			Destroy (vv.gameObject);
		}
		_allFriendItems.Clear ();
		thisPanel.gameObject.SetActive (false);
	}

	public void OnToogleBtn()
	{
	}

	private void InvateFriends()
	{
		Debug.Log("Invate friends");
		JsSpeeker.I.InviteFriendsNative();
	}
}
