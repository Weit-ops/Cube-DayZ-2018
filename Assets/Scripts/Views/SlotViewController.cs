using System.Collections.Generic;
using UnityEngine;

public class SlotViewController : MonoBehaviour
{
	private const string DefaultStorageIcon = "storage";

	[SerializeField] tk2dSprite _icon;
	[SerializeField] tk2dTextMesh _count;
	[SerializeField] tk2dTextMesh _buttonText;
	[SerializeField] bool _isStorageSlot;

	private InventorySlot _slotInfo;
	private int _hotKeyNumber;

	public void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}

	public void UpdateView(InventorySlot slot, int slotIndex = -1)
	{
		_slotInfo = slot;
		if (slotIndex >= 0)
		{
			bool flag = false;
			foreach (KeyValuePair<KeyCode, int> hotKey in InventoryController.Instance.HotKeys)
			{
				if (hotKey.Value == slotIndex)
				{
					_hotKeyNumber = InventoryController.GetNumberByKey(hotKey.Key);
					_buttonText.text = _hotKeyNumber.ToString();
					flag = true;
				}
			}
			if (!flag)
			{
				_hotKeyNumber = 0;
				_buttonText.text = string.Empty;
			}
		}
		else
		{
			_hotKeyNumber = 0;
			_buttonText.text = string.Empty;
		}
		if (_slotInfo.Item != null)
		{
			if (_icon.GetSpriteIdByName(_slotInfo.Item.Icon) == 0)
			{
				_icon.SetSprite(DataKeeper.NoIcon);
			}
			else
			{
				_icon.SetSprite(_slotInfo.Item.Icon);
			}
			_icon.gameObject.SetActive(true);
			_count.text = "x" + _slotInfo.Count.ToString();
		}
		else
		{
			if (_isStorageSlot)
			{
				_icon.SetSprite("storage");
				_icon.gameObject.SetActive(true);
			}
			else
			{
				_icon.gameObject.SetActive(false);
			}
			_count.text = string.Empty;
		}
		Show(true);
	}

	private void OnClick()
	{
		if (_slotInfo.Item != null)
		{
			InventoryViewType viewType = GameUIController.I.Inventory.ViewType;
			if (viewType == InventoryViewType.Storage)
			{
				GameUIController.I.Inventory.StorageView.ShowHowManyWindow(_slotInfo, _slotInfo.Ammo, _isStorageSlot);
			}
			else
			{
				GameUIController.I.Inventory.SelectSlot(_slotInfo, _hotKeyNumber);
			}
		}
	}
}
