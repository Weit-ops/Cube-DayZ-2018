using System.Collections.Generic;
using UnityEngine;

public class MenuFriendsListViewController : MonoBehaviour
{
	[SerializeField] GameObject _shopMenu;
	[SerializeField]
	public GameObject _friendsMenu;

	public static MenuFriendsListViewController I;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	private void BackToShop()
	{
		_friendsMenu.SetActive(false);
		MainMenuController.I.gameObject.SetActive(true);
	}

	private void FriendsBuy()
	{
		List<string> list = new List<string>();
		list = FriendsListView.I.GetFriendsIdToBuy();
		if (list.Count > 0)
		{
			string text = string.Empty;
			for (int i = 0; i < list.Count; i++)
			{
				text = ((i != 0) ? (text + "," + list[i]) : (text + list[i]));
			}
			if (ShopViewController.I.callWindow == 1)
			{
				switch (SpecialPacksRegistry.SpecialPacks[ShopViewController.I.GetCurrentPackIndex()].PackType)
				{
				case SpecialPackType.ItemsPack:
					JsSpeeker.I.BuySomething(SpecialPacksRegistry.SpecialPacks[ShopViewController.I.GetCurrentPackIndex()].Id + "#" + text);
					break;
				case SpecialPackType.Subscription:
				{
					int num = SpecialPacksRegistry.SpecialPacks[ShopViewController.I.GetCurrentPackIndex()].Id + 200;
					JsSpeeker.I.BuySomething(num + "#" + text);
					break;
				}
				case SpecialPackType.Car:
					JsSpeeker.I.BuySomething(SpecialPacksRegistry.SpecialPacks[ShopViewController.I.GetCurrentPackIndex()].Id + "#" + text);
					break;
				}
			}
			if (ShopViewController.I.callWindow == 2)
			{
				switch (SpecialPacksRegistry.SpecialPacks[ShopViewController.I.GetCurrentPackIndex()].PackType)
				{
				case SpecialPackType.ItemsPack:
				{
					int num2 = SpecialPacksRegistry.SpecialPacks[ShopViewController.I.GetCurrentPackIndex()].Id + 600;
					JsSpeeker.I.BuySomething(num2 + "#" + text);
					break;
				}
				case SpecialPackType.Subscription:
					JsSpeeker.I.BuySomething(SpecialPacksRegistry.SpecialPacks[ShopViewController.I.GetCurrentPackIndex()].Id + "#" + text);
					break;
				}
			}
		}
		else
		{
			Debug.LogError("Friends Selected is empty!");
		}
	}

	private void InvateFriends()
	{
		FriendsListView.I.InviteFriendsMobile();
	}
}
