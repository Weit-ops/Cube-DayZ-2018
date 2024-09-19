using System.Collections.Generic;
using UnityEngine;

public class VipCarMenuController : MonoBehaviour
{
	[SerializeField] tk2dUIToggleButtonGroup _respawnPacks;
	[SerializeField] GameObject _buyBtn;
	[SerializeField] GameObject _selectBtn;
	[SerializeField] GameObject _characterMenu;
	[SerializeField] GameObject _shareBtn;

	private Dictionary<int, int> _toggleBtnVipCarsIds = new Dictionary<int, int>
	{
		{ 0, 101 },
		{ 1, 102 },
		{ 2, 103 },
		{ 3, 104 },
		{ 4, 105 },
		{ 5, 106 },
		{ 6, 107 },
		{ 7, 108 },
		{ 8, 109 },
		{ 9, 110 }
	};

	public BoxCollider[] BomzCars;
	public BoxCollider[] OfersCars;
	public static VipCarMenuController I;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	private void CheckEvailibleCars()
	{
		if (PHPNetwork.I != null)
		{
			switch (PHPNetwork.I.GetOfferStatus())
			{
			case 1:
				ToggleOffersCars(false);
				break;
			case 2:
				OfersCars[0].enabled = true;
				OfersCars[1].enabled = false;
				break;
			default:
				ToggleOffersCars(true);
				break;
			}
		}
	}

	private void ToggleBomzCars(bool flag)
	{
		for (int i = 0; i < BomzCars.Length; i++)
		{
			BomzCars[i].enabled = flag;
		}
	}

	private void ToggleOffersCars(bool flag)
	{
		for (int i = 0; i < OfersCars.Length; i++)
		{
			OfersCars[i].enabled = flag;
		}
	}

	private void OnEnable()
	{
		_characterMenu.SetActive(false);
		_respawnPacks.SelectedIndex = 0;
		CheckIfCarPurchased();
		CheckEvailibleCars();
	}

	private void OnSelectCarInMenu()
	{
		CheckIfCarPurchased();
	}

	private void OnClickSelect()
	{
		Debug.Log("Select vip car " + _respawnPacks.SelectedIndex);
		GameControls.I.Player.myCurentCarWrapper.StartChangeCarToVip(_respawnPacks.SelectedIndex);
	}

	private void OnCickBuyVipCar()
	{
		Debug.Log("Покупка тачки " + _toggleBtnVipCarsIds[_respawnPacks.SelectedIndex] + "---");
		JsSpeeker.I.BuySomething(_toggleBtnVipCarsIds[_respawnPacks.SelectedIndex].ToString());
		Debug.Log("Buy vip car " + _respawnPacks.SelectedIndex + "  ");
	}

	private void OnClickCancel()
	{
		Debug.Log("Cancel");
		VipCarManager.I.ShowSelectMenu(false);
	}

	private void OnClickShareCar()
	{
		JsSpeeker.I.ShareCar(_toggleBtnVipCarsIds[_respawnPacks.SelectedIndex]);
	}

	public void CheckIfCarPurchased()
	{
		PurchasedItemsBackensInfo purchasedItemsBackensInfo = DataKeeper.BackendInfo.purchased_items.Find((PurchasedItemsBackensInfo item) => item.shop_id == _toggleBtnVipCarsIds[_respawnPacks.SelectedIndex] && item.count > 0);
		if (purchasedItemsBackensInfo == null)
		{
			_selectBtn.SetActive(false);
			_buyBtn.SetActive(true);
			_shareBtn.SetActive(false);
		}
		else
		{
			_selectBtn.SetActive(true);
			_buyBtn.SetActive(false);
			_shareBtn.SetActive(true);
		}
	}
}
