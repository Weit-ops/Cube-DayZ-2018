using System.Collections.Generic;
using UnityEngine;

namespace BattleRoyale
{
	public class BattleRoyalePropiertiesKeys
	{
		public const string TimeRoomKey = "TimeRoom";
		public const string CurrentSafetyZone = "sz";
	}

	public class BattleRoyaleSetupOptions
	{
		private static byte _maxPlayerOnServer = 45;

		public static int WaitInLobbyForFillRoomToMax = 30;

		private static float _minStartPercent = 21f;

		public static int StartingCountdown = 10;
		public static float FogAreaMovingSpeed = 15f;
		public static float FogSpeed = 25f;
		public static float DefaultFogWaveStep = 150f;

		public static Dictionary<int, int> FogConfigArray = new Dictionary<int, int>
		{
			{ 0, 140 },
			{ 1, 62 },
			{ 2, 62 },
			{ 3, 62 },
			{ 4, 62 },
			{ 5, 62 },
			{ 6, 62 },
			{ 7, 62 },
			{ 8, 62 }
		};

		public static byte MaxPlayerOnServer
		{
			get
			{
				byte result = _maxPlayerOnServer;
				if (JsSpeeker.I.ReskinType == ReskinGameType.Default)
				{
					if (Application.absoluteURL.Contains("play_fb") || Application.absoluteURL.Contains("facebook"))
					{
						result = 20;
					}
				}
				else
				{
					result = 30;
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
					result = 40f;
				}
				return result;
			}
		}

		public static int GetCurrentFogDamage(int waveNumber)
		{
			int result = 0;
			if (waveNumber == 1)
			{
				result = 1;
			}
			if (waveNumber == 2)
			{
				result = 2;
			}
			if (waveNumber == 3)
			{
				result = 3;
			}
			if (waveNumber == 4)
			{
				result = 4;
			}
			if (waveNumber == 5)
			{
				result = 5;
			}
			if (waveNumber > 5)
			{
				result = 10;
			}
			return result;
		}
	}
}
