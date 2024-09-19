using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BattleRoyaleFramework
{
	public class BRFConnection
	{
		public static void JoinOrCreateForGameMode(GameType gameMode, int maxPlr)
		{
			List<RoomInfo> availibleRoomsForGameMode = GetAvailibleRoomsForGameMode(gameMode);
			if (availibleRoomsForGameMode != null)
			{
				if (availibleRoomsForGameMode.Count > 0)
				{
					JoinRandomInGameMode(gameMode, availibleRoomsForGameMode);
				}
				else
				{
					CreateRoomForGameMode(gameMode, maxPlr);
				}
			}
			else
			{
				CreateRoomForGameMode(gameMode, maxPlr);
			}
		}

		public static void CreateRoomForGameMode(GameType gameMode, int maxPlayers)
		{
			System.Random random = new System.Random();
			int num = random.Next(0, 99999);
			MenuConnectionController.I.CreateRoom(GetRoomPrefix(gameMode) + num, false, false, false, maxPlayers);
		}

		public static List<RoomInfo> GetAvailibleRoomsForGameMode(GameType gameMode)
		{
			List<RoomInfo> list = new List<RoomInfo>();
			int num = 0;
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			if (roomList != null)
			{
				RoomInfo[] array = roomList;
				foreach (RoomInfo roomInfo in array)
				{
					if (roomInfo.Name.Contains(GetRoomPrefix(gameMode)))
					{
						num++;
						if (roomInfo.IsOpen)
						{
							list.Add(roomInfo);
						}
					}
				}
				List<RoomInfo> list2 = list.OrderByDescending((RoomInfo count) => count.PlayerCount).ToList();
				foreach (RoomInfo item in list2)
				{
					Debug.Log("FIND! :" + item.Name + "----" + item.MaxPlayers + " ---" + item.PlayerCount);
				}
				list.Clear();
				list = list2;
			}
			return list;
		}

		public static void JoinRandomInGameMode(GameType gameMode, List<RoomInfo> rooms)
		{
			Debug.Log("Try to joing random prefix = " + GetRoomPrefix(gameMode));
			int num = UnityEngine.Random.Range(0, rooms.Count);
			num = 0;
			if (rooms[num] != null && rooms[num].IsOpen)
			{
				Debug.Log("Random tjoin to " + GetRoomPrefix(gameMode) + "--" + rooms[num].Name);
				MenuConnectionController.I.JoinRoom(rooms[num].Name, true);
			}
		}

		public static int GetPlayersCountInGameMode(GameType gameMode)
		{
			int num = 0;
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			if (roomList != null)
			{
				RoomInfo[] array = roomList;
				foreach (RoomInfo roomInfo in array)
				{
					if (roomInfo.Name.Contains(GetRoomPrefix(gameMode)))
					{
						num += roomInfo.PlayerCount;
					}
				}
			}
			return num;
		}

		private static string GetRoomPrefix(GameType gamemode)
		{
			switch (gamemode)
			{
			case GameType.BattleRoyale:
				return "hungrygame";
			case GameType.SkyWars:
				return "skywars";
			default:
				return string.Empty;
			}
		}
	}
}
