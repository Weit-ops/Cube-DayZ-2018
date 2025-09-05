using System.Collections.Generic;
using UnityEngine;

namespace SkyWars
{
	public class SkyWarsSetupOptions
	{
		private static byte _maxPlayerOnServer = 8;

		public static int WaitInLobbyForFillRoomToMax = 10;

		private static float _minStartPercent = 4f;

		public static int StartingCountdown = 10;

		public static float BridgeMovingSpeed = 0.1f;

		public static float BridgeMaxScale = 120f;

		public static float BridgeDelayBeforeStart = 30f;

		public static float HitForce = 2500f;

		public static int RoundDuration = 300;

		public static int RedBtnStartTime = 30;

		public static byte MaxPlayerOnServer
		{
			get
			{
				byte result = _maxPlayerOnServer;
				if (JsSpeeker.I.ReskinType == ReskinGameType.Default)
				{
					if (Application.absoluteURL.Contains("play_fb") || Application.absoluteURL.Contains("facebook"))
					{
						result = 8;
					}
				}
				else
				{
					result = 8;
				}
				return result;
			}
		}

		public static float MinStartPercent
		{
			get
			{
				float result = _minStartPercent;
				if (JsSpeeker.I.ReskinType != 0)
				{
					result = 99f;
				}
				return result;
			}
		}

		public static void SetTestMinPercent(float newMin)
		{
			_minStartPercent = newMin;
		}

		public static void AddWoodenBlocksToInventary()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add("Wooden Foundation SW", 500);
			dictionary.Add("Rucksack", 1);

			PremiumClothingPack premiumClothingPack = CustomizationRegistry.Instance.PremiumClothings[DataKeeper.PremiumPackIndex];
			dictionary.Add(premiumClothingPack.BodyClothingId, 1);
			dictionary.Add(premiumClothingPack.LegsClothingId, 1);

			foreach (KeyValuePair<string, int> item in dictionary)
			{
				Item itemInfo = WorldController.I.Info.GetItemInfo(item.Key);
				if (itemInfo != null)
				{
					if (itemInfo.Type == ItemType.Consumables)
					{
						InventoryController.Instance.AddItems(itemInfo, item.Value);
					}
					else
					{
						InventoryController.Instance.EquipFromPack(itemInfo, 0);
					}
				}
			}
		}
	}
}
