using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AmmoInfo
{
	public AmmoType Type;
	public List<string> Ids;
}

public class AmmoManager : MonoBehaviour
{
	[SerializeField]
	private List<AmmoInfo> _ammoInfos;

	[SerializeField]
	private KeyCode _reloadKey;

	private bool Reload(AmmoType ammoType)
	{
		AmmoInfo ammoInfo = _ammoInfos.Find((AmmoInfo ammo) => ammo.Type == ammoType);
		if (ammoInfo != null && ammoInfo.Ids != null)
		{
			int weaponIndex = ItemsRegistry.I.GetWeaponIndex(InventoryController.Instance.Equipment["Hand"]);
			WeaponBehavior weaponBehavior = GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex];
			foreach (string id in ammoInfo.Ids)
			{
				int itemsCount = InventoryController.Instance.GetItemsCount(id);
				if (itemsCount > 0 && (int)weaponBehavior.ammo < (int)weaponBehavior.maxAmmo)
				{
					int num = Mathf.Min((int)weaponBehavior.bulletsPerClip - (int)weaponBehavior.bulletsLeft, itemsCount);
					InventoryController.Instance.RemoveItems(id, num);
					weaponBehavior.ammo = (int)weaponBehavior.ammo + num;
					weaponBehavior.ReloadWeapon();
					return true;
				}
			}
		}
		return false;
	}

	private void Update()
	{
		if (ControlFreak2.CF2Input.GetKeyDown(_reloadKey) && InventoryController.Instance.Equipment.ContainsKey("Hand"))
		{
			string id = InventoryController.Instance.Equipment["Hand"];
			Weapon weaponInfo = WorldController.I.Info.GetWeaponInfo(id);
			if (weaponInfo != null)
			{
				Reload(weaponInfo.AmmoType);
			}
		}
	}
}
