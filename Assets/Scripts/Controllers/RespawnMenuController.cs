using System.Collections.Generic;
using UnityEngine;

public class RespawnMenuController : MonoBehaviour
{
	private Dictionary<int, int> _toggleBtnPackIds = new Dictionary<int, int>
	{
		{ 1, 1 },
		{ 2, 2 },
		{ 3, 3 },
		{ 4, 4 },
		{ 5, 5 },
		{ 6, 6 },
		{ 7, 7 },
		{ 8, 8 },
		{ 9, 9 }
	};

	[SerializeField] tk2dTextMesh _dieMessage;
	[SerializeField] GameObject _respawnAtBedBtn;
	[SerializeField] GameObject _wallPostNeedHelpBtn;
	[SerializeField] tk2dUIToggleButtonGroup _respawnPacks;
	[SerializeField] GameObject _respawnBtn;

	private bool _canRespawn = true;
	private bool _respawnAtBed;

	[SerializeField] List<RespawnPackCountView> ShopPackCount;

	public void SetKillerName(string killerName, string weaponId)
	{
		if (!string.IsNullOrEmpty(killerName))
		{
			Weapon weapon = (string.IsNullOrEmpty(weaponId) ? null : WorldController.I.Info.GetWeaponInfo(weaponId));
			if (DataKeeper.Language == Language.Russian)
			{
				_dieMessage.text = "Вас убил(а): " + killerName + "\nОружие: " + ((weapon == null) ? "Кулаки" : weapon.RussianName);
			}
			else
			{
				_dieMessage.text = "Killed by: " + killerName + "\nWeapon: " + ((weapon == null) ? "Fists" : weapon.Name);
			}
		}
		else
		{
			_dieMessage.text = string.Empty;
		}
	}

	public void Social_AddWallPost_InviteToServer()
	{
		JsSpeeker.I.InviteFriendsNative();
	}

	private void OnEnable()
	{
		_respawnPacks.SelectedIndex = 0;
		SetPacksToggleButtons();
		_respawnAtBedBtn.SetActive(PlayerSpawnsController.I.CanRespawnAtBed);
		//_wallPostNeedHelpBtn.SetActive(DataKeeper.GameType == GameType.Multiplayer);
	}

	private void SetPacksToggleButtons()
	{
		int i;
		for (i = 0; i < _respawnPacks.ToggleBtns.Length; i++)
		{
			if (_toggleBtnPackIds.ContainsKey(i))
			{
				if (!DataKeeper.IsUserDummy)
				{
					PurchasedItemsBackensInfo purchasedItemsBackensInfo = DataKeeper.BackendInfo.purchased_items.Find((PurchasedItemsBackensInfo item) => item.shop_id == _toggleBtnPackIds[i] && item.count > 0);
				}
				else
				{
					_respawnPacks.ToggleBtns[i].GetComponent<Collider>().enabled = false;
				}
			}
			else
			{
				_respawnPacks.ToggleBtns[i].GetComponent<Collider>().enabled = true;
			}
		}
	}



	private void AddCurrentPack()
	{
		if (_toggleBtnPackIds.ContainsKey(_respawnPacks.SelectedIndex))
		{
			if (!DataKeeper.IsUserDummy)
			{
				PurchasedItemsBackensInfo purchasedItemsBackensInfo = DataKeeper.BackendInfo.purchased_items.Find((PurchasedItemsBackensInfo item) => item.shop_id == _toggleBtnPackIds[_respawnPacks.SelectedIndex] && item.count > 0);
				if (purchasedItemsBackensInfo != null)
				{
					DataKeeper.SelectedSpecialPackId = purchasedItemsBackensInfo.shop_id;
					PHPNetwork.I.UseSpecialPack (DataKeeper.SelectedSpecialPackId - 1);
					AddPackAndRespawn(true);
				}
				else
				{
					AddPackAndRespawn(false);
				}
				Debug.Log("Contains AddCurrentPack: " + _respawnPacks.SelectedIndex + "----" + purchasedItemsBackensInfo.shop_id + "---");
			}
			else
			{
				Debug.Log("FALSE DUMMY Contains AddCurrentPack: " + _respawnPacks.SelectedIndex);
				AddPackAndRespawn(false);
			}
		}
		else
		{
			Debug.Log("FALSE ! _toggleBtnPackIds.ContainsKey: " + _respawnPacks.SelectedIndex);
			AddPackAndRespawn(false);
		}
	}

	private void Respawn()
	{
		if (_respawnAtBed)
		{
			PlayerSpawnsController.I.RespawnAtBed();
		}
		else
		{
			PlayerSpawnsController.I.Respawn(false);
		}
	}

	private void AddPackAndRespawn(bool addPack)
	{
		if (addPack) {
			if (GameUIController.I != null) {
				GameUIController.I._respawnMenu.UpdateUiOnBuy ();
			}
		} else 
		{
			DataKeeper.SelectedSpecialPackId = -1;
		}
		PlayerPrefs.SetInt("SpecialPack", DataKeeper.SelectedSpecialPackId);
		PlayerPrefs.Save();
		Respawn();
	}

	public static void SetDieFlagFalse()
	{
		if (PlayerPrefs.HasKey("Die"))
		{
			PlayerPrefs.SetString("Die", "false");
		}
	}

	private void OnClickRespawn()
	{
		if (_canRespawn)
		{
			SetDieFlagFalse();
			_canRespawn = false;
			_respawnAtBed = false;
			AddCurrentPack();
		}
	}

	private void OnClickRespawnAtBed()
	{
		if (_canRespawn)
		{
			SetDieFlagFalse();
			_canRespawn = false;
			_respawnAtBed = true;
			AddCurrentPack();
		}
	}

	private void OnDisable()
	{
		_canRespawn = true;
	}

	private void ByuSomething()
	{
		int num = _respawnPacks.SelectedIndex;
		num += 600;
		Debug.Log("Покупка товара " + num + "---");
		JsSpeeker.I.BuySomething(num.ToString());
	}

	public void OnSelectPackInMenu()
	{
		if (_respawnPacks.SelectedIndex == 0)
		{
			return;
		}
		int index = _respawnPacks.SelectedIndex;
		PurchasedItemsBackensInfo purchasedItemsBackensInfo = DataKeeper.BackendInfo.purchased_items.Find((PurchasedItemsBackensInfo item) => item.shop_id == index && item.count > 0);
		if (purchasedItemsBackensInfo == null)
		{
			_respawnBtn.SetActive(true);
			_respawnAtBedBtn.SetActive(false);
			return;
		}
		_respawnBtn.SetActive(true);
		if (PlayerSpawnsController.I.CanRespawnAtBed)
		{
			_respawnAtBedBtn.SetActive(true);
		}
	}

	public void UpdateUiOnBuy()
	{
		for (int i = 0; i < ShopPackCount.Count; i++)
		{
			ShopPackCount[i].UpdateCount();
		}
		OnSelectPackInMenu();
	}

	public void OnClickExitBattleRoyale()
	{
		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars)
		{
			SetDieFlagFalse();
			WorldController.I.LeaveRoomBySelf = true;
			WorldController.I.StopCoroutine("AddMobsForPulling");
			WorldController.I.StopCoroutine("AutoSaveMultiplayerWorld");
			if (PhotonNetwork.room != null)
			{
				PhotonNetwork.LeaveRoom();
			}
		}
	}
}
