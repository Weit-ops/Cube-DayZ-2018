using System.Collections.Generic;
using JsonFx.Json;
using UnityEngine;

public class InventorySaveInfo
{
	public short[] I;
	public byte[] C;
	public byte[] A;
	public byte[] S;
	public short[] E;
	public byte AM;

	public InventorySaveInfo()
	{
		
	}

	public InventorySaveInfo(int count)
	{
		I = new short[count];
		C = new byte[count];
		A = new byte[count];
		S = new byte[count];
	}
}

public class PlayerServerSave
{
	public Vector3Short P;
	public byte[] S;
	public InventorySaveInfo I;

	public static void Save(Vector3 position, Dictionary<string, string> equipment, List<InventorySlot> inventory, byte ammoInMagazin, int hp, int hunger, int thirst, int sickness)
	{
		PlayerServerSave playerServerSave = new PlayerServerSave();
		playerServerSave.P = new Vector3Short((short)(position.x * 10f), (short)(position.y * 10f), (short)(position.z * 10f));
		playerServerSave.S = new byte[4];
		playerServerSave.S[0] = (byte)hp;
		playerServerSave.S[1] = (byte)hunger;
		playerServerSave.S[2] = (byte)thirst;
		playerServerSave.S[3] = (byte)sickness;
		List<InventorySlot> list = inventory.FindAll((InventorySlot slot) => slot.Item != null && (int)slot.Count > 0);
		playerServerSave.I = new InventorySaveInfo(list.Count);
		playerServerSave.I.AM = ammoInMagazin;
		List<Item> allItems = WorldController.I.Info.GetAllItems();
		if (equipment != null && equipment.Count > 0)
		{
			List<string> equpmentValues = new List<string>(equipment.Values).FindAll((string v) => !string.IsNullOrEmpty(v));
			playerServerSave.I.E = new short[equpmentValues.Count];
			for (int j = 0; j < equpmentValues.Count; j++)
			{
				playerServerSave.I.E[j] = (short)allItems.FindIndex((Item item) => item.Id == equpmentValues[j]);
			}
		}
		int num = 0;
		for (byte i = 0; i < inventory.Count; i++)
		{
			if (inventory[i].Item != null && (int)inventory[i].Count > 0)
			{
				playerServerSave.I.I[num] = (short)allItems.FindIndex((Item item) => item.Id == inventory[i].Item.Id);
				playerServerSave.I.C[num] = (byte)(int)inventory[i].Count;
				playerServerSave.I.A[num] = inventory[i].Ammo;
				playerServerSave.I.S[num] = i;
				num++;
			}
		}
		string items = JsonWriter.Serialize(playerServerSave);
		DataKeeper.BackendInfo.user.items = items;
	}

	public static PlayerServerSave SaveByLocalInfo(string position, string[] equipment, ItemSaveInfo[] inventory, byte ammoInMagazin, byte[] statesInfo)
	{
		PlayerServerSave playerServerSave = new PlayerServerSave();
		Vector3 vector = new Vector3(200f, 0f, -700f);
		if (position != null)
		{
			vector = ParseUtils.Vector3FromString(position);
		}
		playerServerSave.P = new Vector3Short((short)(vector.x * 10f), (short)(vector.y * 10f), (short)(vector.z * 10f));
		if (statesInfo == null)
		{
			Debug.LogError("statesInfo == null, not resaving");
		}
		else
		{
			playerServerSave.S = new byte[statesInfo.Length];
			for (int k = 0; k < statesInfo.Length; k++)
			{
				playerServerSave.S[k] = statesInfo[k];
			}
		}
		if (inventory == null)
		{
			Debug.LogError("INVENTORY == null, not resaving");
		}
		else
		{
			List<ItemSaveInfo> playerInventory = new List<ItemSaveInfo>(inventory).FindAll((ItemSaveInfo info) => info != null);
			playerServerSave.I = new InventorySaveInfo(playerInventory.Count);
			playerServerSave.I.AM = ammoInMagazin;
			List<Item> allItems = WorldController.I.Info.GetAllItems();
			if (equipment == null)
			{
				Debug.LogError("equipment == null, not resaving");
			}
			if (equipment != null)
			{
				List<string> equipedItemsIds = new List<string>(equipment).FindAll((string id) => !string.IsNullOrEmpty(id));
				playerServerSave.I.E = new short[equipedItemsIds.Count];
				for (int j = 0; j < equipedItemsIds.Count; j++)
				{
					playerServerSave.I.E[j] = (short)allItems.FindIndex((Item item) => item.Id == equipedItemsIds[j]);
				}
			}
			for (byte i = 0; i < playerInventory.Count; i++)
			{
				playerServerSave.I.I[i] = (short)allItems.FindIndex((Item item) => item.Id == playerInventory[i].ItemId);
				playerServerSave.I.C[i] = playerInventory[i].Count;
				playerServerSave.I.A[i] = playerInventory[i].AdditionalCount;
				playerServerSave.I.S[i] = playerInventory[i].SlotIndex;
			}
		}
		string items = JsonWriter.Serialize(playerServerSave);
		DataKeeper.BackendInfo.user.items = items;
		return playerServerSave;
	}
}
