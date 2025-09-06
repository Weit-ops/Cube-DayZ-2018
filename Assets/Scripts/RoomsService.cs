using System.Collections.Generic;

public class GameRoomInfo
{
	public string Id;
	public string Name;
	public bool WithZombies;
	public int MaxPlayersCount;
	public int PlayersReserveCount;
}

public class RoomsService
{
    private const int RoomsWithZombiesCount = 60;
    private const int RoomsWithoutZombiesCount = 40;
    private const int MaxPlayersCount = 30;
    private const int PlayersReserveCount = 20;
    private const int PrivatePlayersReserveCount = 5;

    private static RoomsService _instance;
	private string RoomsName = "Survival PvP #";
	private List<GameRoomInfo> _gameRoomsInfos;
	private List<GameRoomInfo> _myPrivateRooms;

	public static RoomsService Instance
	{
		get
		{
			return _instance ?? (_instance = new RoomsService());
		}
	}

	public List<GameRoomInfo> GameRoomsInfos
	{
		get
		{
			return _gameRoomsInfos;
		}
	}

	public List<GameRoomInfo> MyPrivateRooms
	{
		get
		{
			return _myPrivateRooms;
		}
	}

	private RoomsService()
	{
		_myPrivateRooms = new List<GameRoomInfo>();
		_gameRoomsInfos = new List<GameRoomInfo>();
		_gameRoomsInfos.Add(new GameRoomInfo
		{
			Id = "SurvPvp_E1",
			Name = "Survival PvP Extreme #1",
			WithZombies = true,
			MaxPlayersCount = 20 /*80*/,
			PlayersReserveCount = 20
		});
		_gameRoomsInfos.Add(new GameRoomInfo
		{
			Id = "SurvPvp_E2",
			Name = "Survival PvP Extreme #2",
			WithZombies = true,
			MaxPlayersCount = 20 /*60*/,
			PlayersReserveCount = 20
		});
		int num = 0;
		for (int i = 0; i < 60; i++)
		{
			num++;
			_gameRoomsInfos.Add(new GameRoomInfo
			{
				Id = "SurvPvpZ_" + (i + 1),
				Name = RoomsName + num,
				WithZombies = true,
				MaxPlayersCount = 20 /*30*/,
				PlayersReserveCount = 20
			});
		}
		for (int j = 0; j < 40; j++)
		{
			num++;
			_gameRoomsInfos.Add(new GameRoomInfo
			{
				Id = "SurvPvp_" + (j + 1),
				Name = RoomsName + num,
				MaxPlayersCount = 20 /*30*/,
				PlayersReserveCount = 20
			});
		}
	}

	public void SetMyPrivateRooms()
	{
		_myPrivateRooms.Clear();
		_myPrivateRooms.Add(new GameRoomInfo
		{
			Id = "PrivateZ_" + JsSpeeker.viewer_id,
			Name = "private(zombie)_" + JsSpeeker.viewer_id,
			WithZombies = true,
			MaxPlayersCount = 20/*30*/,
			PlayersReserveCount = 5
		});
		_myPrivateRooms.Add(new GameRoomInfo
		{
			Id = "Private_" + JsSpeeker.viewer_id,
			Name = "private_" + JsSpeeker.viewer_id,
			MaxPlayersCount = 20/*30*/,
			PlayersReserveCount = 5
		});
	}

	public List<GameRoomInfo> GetEmptyRoomsInfos(List<RoomInfo> photonRooms)
	{
		List<GameRoomInfo> list = new List<GameRoomInfo>();
		GameRoomInfo gameRoomsInfo;
		foreach (GameRoomInfo gameRoomsInfo2 in _gameRoomsInfos)
		{
			gameRoomsInfo = gameRoomsInfo2;
			if (photonRooms.Find((RoomInfo photonRoom) => photonRoom.Name == gameRoomsInfo.Name) == null)
			{
				list.Add(gameRoomsInfo);
			}
		}
		return list;
	}
}
