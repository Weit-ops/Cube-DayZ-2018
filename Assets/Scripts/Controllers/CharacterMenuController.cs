using UnityEngine;

public enum CharacterMenuType
{
	Inventory = 0,
	Craft = 1,
	Skills = 2,
	Info = 3,
	Menu = 4
}

public class CharacterMenuController : MonoBehaviour
{
	[SerializeField] tk2dUIToggleButtonGroup _viewTabs;
	[SerializeField] InventoryViewController _inventory;
	[SerializeField] MapViewController _mapView;
	[SerializeField] GameObject _vipCarMenu;

	public ChatBattlePos ChatPos;

	public InventoryViewController Inventory
	{
		get
		{
			return _inventory;
		}
	}

	public MapViewController Map
	{
		get
		{
			return _mapView;
		}
	}

	public bool IsShown
	{
		get
		{
			return base.gameObject.activeSelf;
		}
	}

	public CharacterMenuType CurrentType { get; private set; }

	public void ShowView(bool show, CharacterMenuType type, bool consideringControl = true, InventoryViewType inventoryType = InventoryViewType.Default)
	{
		CurrentType = type;
		_inventory.ViewType = inventoryType;
		if (inventoryType != InventoryViewType.Storage)
		{
			_inventory.StorageAction = null;
		}
		base.gameObject.SetActive(show);
		_viewTabs.SelectedIndex = (int)type;
		if (consideringControl)
		{
			GameControls.I.MenuControls(show);
		}
	}

	public void ShowStorageInventory(WorldObjectStorageAction currentStorage)
	{
		_inventory.StorageAction = currentStorage;
		ShowView(true, CharacterMenuType.Inventory, true, InventoryViewType.Storage);
	}

	public void OnEnable()
	{
		_vipCarMenu.SetActive(false);

		if (MouseOrbitWithZoom.I != null)
		{
			MouseOrbitWithZoom.I.isEnable = false;
		}

		if (ChatPos != null)
		{
			ChatPos.HideChat();
		}
	}

	public void OnDisable()
	{
		if (MouseOrbitWithZoom.I != null)
		{
			MouseOrbitWithZoom.I.isEnable = true;
		}

		if (ChatPos != null)
		{
			ChatPos.ShowChat();
		}
	}
}
