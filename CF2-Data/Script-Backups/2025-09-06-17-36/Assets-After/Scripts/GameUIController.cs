using System.Collections;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
	public static GameUIController I;

	[SerializeField]
	private KeyCode _inventoryMenuKey = KeyCode.I;

	[SerializeField]
	private KeyCode _craftMenuKey = KeyCode.C;

	[SerializeField]
	private KeyCode _mapMenuKey = KeyCode.M;

	[SerializeField]
	private KeyCode _menuKey = KeyCode.Tab;

	[SerializeField]
	public RespawnMenuController _respawnMenu;

	[SerializeField]
	public CharacterMenuController _characterMenuController;

	public Light[] headLights;

	[SerializeField]
	private GameObject _carsInfo;

	[SerializeField]
	private GameObject _vipCarsPanel;

	[SerializeField]
	private GameObject _pressBLbl;

	public InventoryViewController Inventory
	{
		get
		{
			return _characterMenuController.Inventory;
		}
	}

	public MapViewController Map
	{
		get
		{
			return _characterMenuController.Map;
		}
	}

	private void Awake()
	{
		I = this;
	}

	private void Update()
	{
		if (WorldController.I == null || WorldController.I.Player == null || WorldController.I.Player.IsDead || (ChatGui.I != null && ChatGui.I.chatIsFocused))
		{
			return;
		}

		if (OrbitCameraController.I != null && !OrbitCameraController.I._OrbitEnable)
		{
			_characterMenuController.ShowView(false, CharacterMenuType.Inventory);
		}
		if (ControlFreak2.CF2Input.GetKeyDown(_inventoryMenuKey) && OrbitCameraController.I != null && OrbitCameraController.I._OrbitEnable)
		{
			_characterMenuController.ShowView(ShowCharacterMenu(CharacterMenuType.Inventory), CharacterMenuType.Inventory);
		}
		if (ControlFreak2.CF2Input.GetKeyDown(_craftMenuKey))
		{
			if (JsSpeeker.I.ReskinType != 0)
			{
				return;
			}
			_characterMenuController.ShowView(ShowCharacterMenu(CharacterMenuType.Craft), CharacterMenuType.Craft);
		}
		if (ControlFreak2.CF2Input.GetKeyDown(_mapMenuKey))
		{
			_characterMenuController.ShowView(ShowCharacterMenu(CharacterMenuType.Info), CharacterMenuType.Info);
		}
		if (ControlFreak2.CF2Input.GetKeyDown(_menuKey))
		{
			_characterMenuController.ShowView(ShowCharacterMenu(CharacterMenuType.Menu), CharacterMenuType.Menu);
		}
	}

	private bool ShowCharacterMenu(CharacterMenuType type)
	{
		return !_characterMenuController.IsShown || _characterMenuController.CurrentType != type;
	}

	public void ShowRespawnMenu(bool show, string killerName = null, string weaponId = null)
	{
		_respawnMenu.gameObject.SetActive(show);
		_respawnMenu.SetKillerName(killerName, weaponId);

		if (show)
		{
			GameControls.I.MenuControls(true, false);
			return;
		}

		GameControls.I.MenuControls(false);
		GameControls.I.Player.GetComponent<Rigidbody>().freezeRotation = true;
	}

	public void ShowStorageInventory(WorldObjectStorageAction currentStorage)
	{
		_characterMenuController.ShowStorageInventory(currentStorage);
	}

	public void ShowCharacterMenu(bool show, CharacterMenuType type, bool consideringControl = true)
	{
		_characterMenuController.ShowView(show, type, consideringControl);
		if (consideringControl)
		{
			GameControls.I.MenuControls(show);
		}
	}

	public void EnableCarInfo(bool flag)
	{
		_carsInfo.SetActive(flag);
		EnableHelpLbl(flag);
		if (flag)
		{
			StartCoroutine("HideCarInfo");
		}
	}

	private IEnumerator HideCarInfo()
	{
		yield return new WaitForSeconds(15f);
		_carsInfo.SetActive(false);
	}

	private void EnableHelpLbl(bool flag)
	{
		_pressBLbl.SetActive(flag);
	}
}
