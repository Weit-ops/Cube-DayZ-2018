using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
	[SerializeField] tk2dSprite _icon;
	[SerializeField] string _key;
	[SerializeField] string _defaultSprite;

	private string _id;

	public string Key
	{
		get
		{
			return _key;
		}
	}

	public void UpdateView(string id = null)
	{
		_id = id;
		if (_defaultSprite == null)
		{
			_defaultSprite = _icon.CurrentSprite.name;
		}
		string text = null;
		if (!string.IsNullOrEmpty(id))
		{
			text = WorldController.I.Info.GetItemInfo(id).Icon;
			if (_icon.GetSpriteIdByName(text) == 0)
			{
				text = DataKeeper.NoIcon;
			}
		}
		_icon.SetSprite(text ?? _defaultSprite);
	}

	private void OnClick()
	{
		byte ammo = 0;
		if (WorldController.I.Info.GetWeaponInfo(_id) != null)
		{
			int weaponIndex = ItemsRegistry.I.GetWeaponIndex(_id);
			if (weaponIndex < GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count)
			{
				ammo = (byte)(int)GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex].bulletsLeft;
			}
		}
		Item itemInfo = WorldController.I.Info.GetItemInfo(_id);
		if ((itemInfo.Type != 0 && itemInfo.Type != ItemType.Consumables) || !(GameControls.I != null) || !(GameControls.I.Walker != null) || !GameControls.I.Walker.climbing)
		{
			InventoryController.Instance.TakeOff(_key, true, false, false, ammo);
			UpdateView();
		}
	}
}
