using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsListView : MonoBehaviour
{
	private const int ScrollSpeed = 5;

	[SerializeField] GameObject _friendsItemView;
	[SerializeField] GameObject _allFriendsContent;
	[SerializeField] float _roomViewSize;
	[SerializeField] float _offsetBetweenRoomView;
	[SerializeField] tk2dUIScrollableArea _scrollableArea;
	[SerializeField] GameObject _sendGiftToFriendBtn;
	[SerializeField] GameObject _invateFriendBtn;
	[SerializeField] tk2dTextMesh _selFriendsCount;

	private List<FriendItemView> _allFriendItems = new List<FriendItemView>();

	public static FriendsListView I;

	private bool isFill;
	private int _maxSelectedFriends = 6;
	private int _selectedFriendscount;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	private void OnEnable()
	{
		if (JsSpeeker.FriendsInfos.Count > 0)
		{
			_allFriendsContent.SetActive(true);
			_invateFriendBtn.SetActive(false);
		}
		else
		{
			_sendGiftToFriendBtn.SetActive(false);
			_invateFriendBtn.SetActive(true);
		}
		if (!isFill)
		{
			ShowFriendsPanel();
			isFill = true;
		}
	}

	private void OnDisable()
	{
		ResetAllBtn();
		_invateFriendBtn.SetActive(false);
		_invateFriendBtn.SetActive(false);
	}

	private void ShowFriendsPanel()
	{
		if (JsSpeeker.FriendsInfos.Count > 0)
		{
			_allFriendItems = CubezStatic.FillFriendsList(_friendsItemView, _allFriendsContent, _roomViewSize, _scrollableArea, 5, _offsetBetweenRoomView);
			CubezStatic.SetFriendsPhotoInMainMenu (_allFriendItems);
		}
	}

	public void SetUpSelectedFriendsCount()
	{
		Debug.Log("setuppppppppppppppppp");
		_selectedFriendscount = GetSelectedFriendsCount();
		if (_selectedFriendscount > 0)
		{
			_invateFriendBtn.SetActive(true);
		}
		if (_selectedFriendscount >= _maxSelectedFriends)
		{
			DisableInActiveBtn();
			_selFriendsCount.text = _selectedFriendscount + "  (max!) ";
		}
		else
		{
			_selFriendsCount.text = _selectedFriendscount.ToString();
		}
	}

	public void SetDownSelectedFriendsCount()
	{
		_selectedFriendscount = GetSelectedFriendsCount();
		if (_selectedFriendscount <= 0)
		{
			_invateFriendBtn.SetActive(false);
		}
		EnableAllBtn();
		_selFriendsCount.text = _selectedFriendscount.ToString();
	}

	private int GetSelectedFriendsCount()
	{
		int num = 0;
		for (int i = 0; i < _allFriendItems.Count; i++)
		{
			if (_allFriendItems[i].GetToogle())
			{
				num++;
			}
		}
		return num;
	}

	private void DisableInActiveBtn()
	{
		for (int i = 0; i < _allFriendItems.Count; i++)
		{
			if (!_allFriendItems[i].GetToogle())
			{
				_allFriendItems[i].DisavtiveBtn();
			}
		}
	}

	private void EnableAllBtn()
	{
		for (int i = 0; i < _allFriendItems.Count; i++)
		{
			_allFriendItems[i].AvtivateBtn();
		}
	}

	public List<string> GetFriendsIdToBuy()
	{
		List<string> list = new List<string>();
		if (_allFriendItems.Count > 0)
		{
			for (int i = 0; i < _allFriendItems.Count; i++)
			{
				if (_allFriendItems[i].GetToogle() && !string.IsNullOrEmpty(_allFriendItems[i].GetFriendId()))
				{
					list.Add(_allFriendItems[i].GetFriendId());
				}
			}
		}
		return list;
	}

	public void ResetAllBtn()
	{
		_selectedFriendscount = 0;
		_selFriendsCount.text = _selectedFriendscount.ToString();
		for (int i = 0; i < _allFriendItems.Count; i++)
		{
			_allFriendItems[i].AvtivateBtn();
			_allFriendItems[i].SetToogle(false);
		}
	}

	public void InviteFriendsMobile()
	{
		List<string> friendsIdToBuy = GetFriendsIdToBuy();
		for (int i = 0; i < friendsIdToBuy.Count; i++)
		{
			StartCoroutine("StartInvitingFriendsMobile", friendsIdToBuy[i]);
		}
		MenuFriendsListViewController.I._friendsMenu.SetActive(false);
		MainMenuController.I.gameObject.SetActive(true);
	}

	private IEnumerator StartInvitingFriendsMobile(string userid)
	{
		yield return new WaitForSeconds(0.1f);
		if (Controller.I != null)
		{
			Controller.I.InviteFriend(Convert.ToInt32(userid), null);
		}
	}
}
