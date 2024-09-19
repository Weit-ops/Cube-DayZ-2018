using System.Collections.Generic;
using UnityEngine;

public class RecipeInfo
{
	public string ResultItemId;
	public int ResultItemCount;
	public string ToolId;
	public int CraftingLevel;
	public List<ItemRecipeInfo> CraftingItems;
}

public class ItemRecipeInfo
{
	public string ItemId;
	public int Count;
}

[RequireComponent(typeof(AudioSource))]
public class RecipeView : MonoBehaviour
{
	private const int LastItems = 0;

	[SerializeField] tk2dUILayoutContainerSizer _uiLayoutContainer;
	[SerializeField] tk2dUILayout _item;
	[SerializeField] tk2dUILayout _plus;
	[SerializeField] RecipeItemView _result;
	[SerializeField] AudioClip _canCraft;
	[SerializeField] AudioClip _canNotCraft;
	[SerializeField] tk2dSlicedSprite _back;

	private List<RecipeItemView> _itemsViews = new List<RecipeItemView>();
	private RecipeInfo _info;

	public void UpdateView(RecipeInfo info)
	{
		_info = info;
		ClearRecipeItem();
		foreach (ItemRecipeInfo craftingItem in _info.CraftingItems)
		{
			AddRecipeItem(craftingItem);
		}
		if (!string.IsNullOrEmpty(_info.ToolId))
		{
			AddLayoutToContainer(_plus);
			tk2dUILayout tk2dUILayout2 = AddLayoutToContainer(_item);
			if (tk2dUILayout2 != null)
			{
				RecipeItemView component = tk2dUILayout2.GetComponent<RecipeItemView>();
				component.UpdateView(info.ToolId, 0);
				_itemsViews.Add(component);
			}
		}
		_result.UpdateView(info.ResultItemId, info.ResultItemCount);
		_uiLayoutContainer.Refresh();
		SetBackColor();
	}

	public void ShowView(bool show)
	{
		base.gameObject.SetActive(show);
	}

	private void AddRecipeItem(ItemRecipeInfo info)
	{
		if (_itemsViews.Count > 0)
		{
			AddLayoutToContainer(_plus);
		}
		tk2dUILayout tk2dUILayout2 = AddLayoutToContainer(_item);
		if (tk2dUILayout2 != null)
		{
			RecipeItemView component = tk2dUILayout2.GetComponent<RecipeItemView>();
			component.UpdateView(info.ItemId, info.Count);
			_itemsViews.Add(component);
		}
	}

	private tk2dUILayout AddLayoutToContainer(tk2dUILayout layout)
	{
		tk2dUILayout tk2dUILayout2 = Object.Instantiate(layout);
		if (tk2dUILayout2 != null)
		{
			_uiLayoutContainer.AddLayoutAtIndex(tk2dUILayout2, tk2dUILayoutItem.FixedSizeLayoutItem(), _uiLayoutContainer.layoutItems.Count);
			tk2dUILayout2.transform.localPosition = Vector3.zero;
		}
		return tk2dUILayout2;
	}

	private bool CanCraft()
	{
		bool flag = true;
		foreach (ItemRecipeInfo craftingItem in _info.CraftingItems)
		{
			if (InventoryController.Instance.GetItemsCount(craftingItem.ItemId) < craftingItem.Count)
			{
				flag = false;
			}
		}
		if (!string.IsNullOrEmpty(_info.ToolId))
		{
			flag = flag && InventoryController.Instance.GetItemsCount(_info.ToolId) > 0;
		}
		return flag;
	}

	private void SetBackColor()
	{
	}

	private void ClearRecipeItem()
	{
		_itemsViews.Clear();
		while (_uiLayoutContainer.layoutItems.Count > 0)
		{
			GameObject gameObj = _uiLayoutContainer.layoutItems[0].gameObj;
			_uiLayoutContainer.RemoveLayout(_uiLayoutContainer.layoutItems[0].layout);
			Object.Destroy(gameObj);
		}
		_uiLayoutContainer.Refresh();
	}

	private void OnClick()
	{
		bool flag = false;
		if (CanCraft())
		{
			Item itemInfo = WorldController.I.Info.GetItemInfo(_info.ResultItemId);
			if (itemInfo != null)
			{
				WorldController.I.PlayerStatistic.CraftItems++;
				foreach (ItemRecipeInfo craftingItem in _info.CraftingItems)
				{
					InventoryController.Instance.RemoveItems(craftingItem.ItemId, craftingItem.Count);
				}
				int num = InventoryController.Instance.AddItems(itemInfo, _info.ResultItemCount);
				if (num > 0)
				{
					WorldController.I.Player.PlayerDropItem(GameControls.I.Player.transform.position, itemInfo.Id, num, (byte)0);
				}
				flag = true;
				SendCraftMission(itemInfo.Id);
			}
		}
		GetComponent<AudioSource>().PlayOneShot((!flag) ? _canNotCraft : _canCraft);
		SetBackColor();
	}

	private void SendCraftMission(string craftItem)
	{
		int current_mission_id = 0;
		switch (craftItem)
		{
		case "MOAB":
			current_mission_id = 36;
			break;
		case "Locker":
			current_mission_id = 35;
			break;
		case "Chest":
			current_mission_id = 34;
			break;
		case "Crate":
			current_mission_id = 33;
			break;
		case "Handlamp":
			current_mission_id = 32;
			break;
		case "mapleBow":
			current_mission_id = 31;
			break;
		case "birchBow":
			current_mission_id = 30;
			break;
		case "pineBow":
			current_mission_id = 29;
			break;
		case "Maple Barricade":
			current_mission_id = 28;
			break;
		case "Wooden Spike Trap":
			current_mission_id = 27;
			break;
		case "Electric Fence":
			current_mission_id = 26;
			break;
		case "Barbed Fence":
			current_mission_id = 25;
			break;
		case "Electric Trap":
			current_mission_id = 24;
			break;
		case "Trip Mine":
			current_mission_id = 23;
			break;
		case "Cot":
			current_mission_id = 22;
			break;
		case "Wooden Foundation":
			current_mission_id = 21;
			break;
		case "Wooden Ladder":
			current_mission_id = 20;
			break;
		case "Dressing":
			current_mission_id = 19;
			break;
		case "Campfire":
			current_mission_id = 18;
			break;
		case "Landmine":
			current_mission_id = 17;
			break;
		case "leatherJacket":
			current_mission_id = 16;
			break;
		}
	}
}
