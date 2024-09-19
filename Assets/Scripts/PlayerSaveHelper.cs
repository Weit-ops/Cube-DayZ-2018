using System;
using System.Collections.Generic;
using JsonFx.Json;
using UnityEngine;

public class PlayerSaveInfo
{
	public string MapId;

	public string PlayerPosition;

	public string[] Equipment;

	public ItemSaveInfo[] Inventory;

	public byte[] StatesInfo;

	public byte AmmoInClip;

	public static void Save(GameType gameType, string mapId, Vector3 position, Dictionary<string, string> equipment, List<InventorySlot> inventory, byte ammoInMagazin, int hp, int hunger, int thirst, int sickness)
	{
		PlayerSaveInfo playerSaveInfo = new PlayerSaveInfo();
		playerSaveInfo.MapId = mapId;
		PlayerSaveInfo playerSaveInfo2 = playerSaveInfo;
		if (hp > 0)
		{
			playerSaveInfo2.AmmoInClip = ammoInMagazin;
			playerSaveInfo2.StatesInfo = new byte[4];
			playerSaveInfo2.StatesInfo[0] = (byte)hp;
			playerSaveInfo2.StatesInfo[1] = (byte)hunger;
			playerSaveInfo2.StatesInfo[2] = (byte)thirst;
			playerSaveInfo2.StatesInfo[3] = (byte)sickness;
			if (equipment != null && equipment.Count > 0)
			{
				string[] names = Enum.GetNames(typeof(ClothingBodyPart));
				playerSaveInfo2.Equipment = new string[names.Length + 1];
				if (equipment.ContainsKey("Hand"))
				{
					playerSaveInfo2.Equipment[0] = equipment["Hand"];
				}
				string[] array = names;
				foreach (string text in array)
				{
					if (equipment.ContainsKey(text))
					{
						byte b = (byte)Enum.Parse(typeof(ClothingBodyPart), text);
						playerSaveInfo2.Equipment[b] = equipment[text];
					}
				}
			}
			if (inventory != null && inventory.Count > 0)
			{
				playerSaveInfo2.Inventory = new ItemSaveInfo[inventory.Count];
				for (byte b2 = 0; b2 < inventory.Count; b2++)
				{
					if (inventory[b2].Item != null)
					{
						playerSaveInfo2.Inventory[b2] = new ItemSaveInfo(b2, inventory[b2].Item.Id, (byte)(int)inventory[b2].Count, inventory[b2].Ammo);
					}
				}
			}
			playerSaveInfo2.PlayerPosition = position.ToString();
		}
		string value = JsonWriter.Serialize(playerSaveInfo2);
		PlayerPrefs.SetString(GetPathForSave(gameType), value);
		PlayerPrefs.Save();
	}

	public static PlayerSaveInfo Load(GameType gameType)
	{
		if (PlayerPrefs.HasKey(GetPathForSave(gameType)))
		{
			return JsonReader.Deserialize<PlayerSaveInfo>(PlayerPrefs.GetString(GetPathForSave(gameType)));
		}
		if (PlayerPrefs.HasKey(gameType.ToString()))
		{
			return JsonReader.Deserialize<PlayerSaveInfo>(PlayerPrefs.GetString(gameType.ToString()));
		}
		return null;
	}

	private static string GetPathForSave(GameType gameType)
	{
		if (!string.IsNullOrEmpty(JsSpeeker.viewer_id))
		{
			return gameType.ToString() + JsSpeeker.viewer_id;
		}
		return gameType.ToString();
	}
}

public class PlayerSaveHelper
{
	public PlayerServerSave PlayerServerSaveInfo { get; private set; }
	public PlayerSaveInfo PlayerLocalSaveInfo { get; private set; }

	public bool HasSaveInfo
	{
		get
		{
			return PlayerLocalSaveInfo != null || PlayerServerSaveInfo != null;
		}
	}

	public PlayerSaveHelper()
	{
		LoadSaveInfo();
	}

	public Dictionary<byte, InventorySlot> GetInventoryInfo()
	{
		Dictionary<byte, InventorySlot> dictionary = new Dictionary<byte, InventorySlot>();
		if (PlayerServerSaveInfo != null && PlayerServerSaveInfo.I != null && PlayerServerSaveInfo.I.I != null)
		{
			List<Item> allItems = WorldController.I.Info.GetAllItems();
			for (int i = 0; i < PlayerServerSaveInfo.I.I.Length; i++)
			{
				InventorySlot inventorySlot = new InventorySlot();
				Item item = ((PlayerServerSaveInfo.I.I[i] < 0 || PlayerServerSaveInfo.I.I[i] >= allItems.Count) ? null : allItems[PlayerServerSaveInfo.I.I[i]]);
				if (item != null)
				{
					inventorySlot.AddSome(item, PlayerServerSaveInfo.I.C[i], PlayerServerSaveInfo.I.A[i]);
				}
				dictionary.Add(PlayerServerSaveInfo.I.S[i], inventorySlot);
			}
		}
		if (PlayerLocalSaveInfo != null && PlayerLocalSaveInfo.Inventory != null)
		{
			ItemSaveInfo[] inventory = PlayerLocalSaveInfo.Inventory;
			foreach (ItemSaveInfo itemSaveInfo in inventory)
			{
				if (itemSaveInfo != null)
				{
					InventorySlot inventorySlot2 = new InventorySlot();
					Item itemInfo = WorldController.I.Info.GetItemInfo(itemSaveInfo.ItemId);
					if (itemInfo != null)
					{
						Debug.Log(string.Concat("slot.AddSome ", itemInfo, "  ", itemSaveInfo.Count));
						inventorySlot2.AddSome(itemInfo, itemSaveInfo.Count, itemSaveInfo.AdditionalCount);
					}
					dictionary.Add(itemSaveInfo.SlotIndex, inventorySlot2);
				}
			}
		}
		return dictionary;
	}

	public byte[] GetPlayerStates()
	{
		if (PlayerServerSaveInfo != null)
		{
			return PlayerServerSaveInfo.S;
		}
		if (PlayerLocalSaveInfo != null)
		{
			return PlayerLocalSaveInfo.StatesInfo;
		}
		return null;
	}

	public byte GetAmmoForWeaponInHand()
	{
		if (PlayerServerSaveInfo != null && PlayerServerSaveInfo.I != null)
		{
			return PlayerServerSaveInfo.I.AM;
		}
		if (PlayerLocalSaveInfo != null)
		{
			return PlayerLocalSaveInfo.AmmoInClip;
		}
		return 0;
	}

	public Vector3? GetPlayerPosition()
	{
		if (PlayerServerSaveInfo != null)
		{
			return new Vector3((float)PlayerServerSaveInfo.P.X / 10f, (float)PlayerServerSaveInfo.P.Y / 10f, (float)PlayerServerSaveInfo.P.Z / 10f);
		}
		if (PlayerLocalSaveInfo != null)
		{
			if (PlayerLocalSaveInfo.PlayerPosition == null)
			{
				Debug.Log("GetPlayerPosition() null, ignoring and choosing default respawn");
				return new Vector3(200f, 0f, -700f);
			}
			return ParseUtils.Vector3FromString(PlayerLocalSaveInfo.PlayerPosition);
		}
		return null;
	}

	public string[] GetEquipedItems()
	{
		if (PlayerServerSaveInfo != null && PlayerServerSaveInfo.I != null && PlayerServerSaveInfo.I.E != null)
		{
			return WorldController.I.GetItemsIdsByIndices(PlayerServerSaveInfo.I.E);
		}
		if (PlayerLocalSaveInfo != null)
		{
			return PlayerLocalSaveInfo.Equipment;
		}
		return null;
	}

	public void ClearInventoryInfo()
	{
		if (PlayerServerSaveInfo != null)
		{
			PlayerServerSaveInfo.I = null;
		}
		if (PlayerLocalSaveInfo != null)
		{
			PlayerLocalSaveInfo.Equipment = null;
			PlayerLocalSaveInfo.Inventory = null;
		}
		RemoveInfoIfAllNull();
	}

	public void ClearPosition()
	{
		if (PlayerServerSaveInfo != null)
		{
			PlayerServerSaveInfo.P = null;
		}
		if (PlayerLocalSaveInfo != null)
		{
			PlayerLocalSaveInfo.PlayerPosition = null;
		}
		RemoveInfoIfAllNull();
	}

	public void ClearStatesInfo()
	{
		if (PlayerServerSaveInfo != null)
		{
			PlayerServerSaveInfo.S = null;
		}
		if (PlayerLocalSaveInfo != null)
		{
			PlayerLocalSaveInfo.StatesInfo = null;
		}
		RemoveInfoIfAllNull();
	}

	private void RemoveInfoIfAllNull()
	{
		if (PlayerServerSaveInfo != null && PlayerServerSaveInfo.S == null && PlayerServerSaveInfo.I == null && PlayerServerSaveInfo.P == null)
		{
			PlayerServerSaveInfo = null;
		}
		if (PlayerLocalSaveInfo != null && PlayerLocalSaveInfo.Inventory == null && PlayerLocalSaveInfo.Equipment == null && PlayerLocalSaveInfo.StatesInfo == null && string.IsNullOrEmpty(PlayerLocalSaveInfo.PlayerPosition))
		{
			PlayerLocalSaveInfo = null;
		}
	}

	private void LoadSaveInfo()
	{
		if (DataKeeper.GameType == GameType.Multiplayer)
		{
			if (!string.IsNullOrEmpty (DataKeeper.BackendInfo.user.items)) {
				PlayerServerSaveInfo = JsonReader.Deserialize<PlayerServerSave> (DataKeeper.BackendInfo.user.items);
				return;
			} else {
			}

			PlayerSaveInfo playerSaveInfo = PlayerSaveInfo.Load(DataKeeper.GameType);
			if (playerSaveInfo != null)
			{
				PlayerServerSaveInfo = PlayerServerSave.SaveByLocalInfo(playerSaveInfo.PlayerPosition, playerSaveInfo.Equipment, playerSaveInfo.Inventory, playerSaveInfo.AmmoInClip, playerSaveInfo.StatesInfo);
			}
		}
		else if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
		{
			PlayerSaveInfo playerLocalSaveInfo = PlayerSaveInfo.Load(DataKeeper.GameType);
			PlayerLocalSaveInfo = playerLocalSaveInfo;
		}
	}
}
