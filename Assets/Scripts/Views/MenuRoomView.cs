using System;
using UnityEngine;

public class MenuRoomView : MonoBehaviour
{
	[SerializeField] tk2dTextMesh _name;
	[SerializeField] tk2dTextMesh _playersCount;
	[SerializeField] GameObject _zombiesIcon;
	[SerializeField] GameObject _friendsIcon;
	[SerializeField] GameObject _startButton;

	private Action<string> _onClickStart;
	private RoomInfo _photonInfo;
	private GameRoomInfo _info;

	public string RoomName
	{
		get
		{
			return (_info == null) ? _photonInfo.Name : _info.Name;
		}
	}

	public void SetBaseInfo(GameRoomInfo info)
	{
		_info = info;
	}

	public void SetInfo(int currentPlayerCount, bool hasFriend, bool showReserv, Action<string> onClickStart)
	{
		_photonInfo = null;
		_onClickStart = onClickStart;
		bool active = false;
		string text = RoomName ?? string.Empty;
		int num = 0;
		bool active2 = true;
		int num2 = 0;
		if (_info != null)
		{
			active = currentPlayerCount < _info.MaxPlayersCount;
			if (!DataKeeper.IsUserDummy && DataKeeper.BackendInfo.user.has_premium)
			{
				active = currentPlayerCount < _info.MaxPlayersCount + _info.PlayersReserveCount;
			}
			num2 = _info.PlayersReserveCount;
			num = _info.MaxPlayersCount;
			active2 = _info.WithZombies;
		}
		_startButton.SetActive(active);
		_name.text = text;
		if (showReserv && num2 > 0)
		{
			_playersCount.text = "^C00FF00FF" + currentPlayerCount + "/" + num + "^CFFBB38FF (+" + num2 + " для VIP)";
		}
		else
		{
			_playersCount.text = "^C00FF00FF" + currentPlayerCount + "/" + num;
		}
		_friendsIcon.SetActive(hasFriend);
		_zombiesIcon.SetActive(active2);
	}

	public void SetInfo(RoomInfo info, bool hasFriend, bool alwaysShowJoinButton, Action<string> onClickStart)
	{
		_info = null;
		_photonInfo = info;
		_onClickStart = onClickStart;
		_startButton.SetActive(alwaysShowJoinButton || info.PlayerCount < info.MaxPlayers);
		_name.text = info.Name;
		_playersCount.text = info.PlayerCount + "/" + info.MaxPlayers;
		_friendsIcon.SetActive(hasFriend);
		bool active = false;
		if (info.CustomProperties.ContainsKey("zombies"))
		{
			active = (bool)info.CustomProperties["zombies"];
		}
		_zombiesIcon.SetActive(active);
	}

	private void OnClickStart()
	{
		if (_onClickStart != null)
		{
			_onClickStart(RoomName);
		}
	}
}
