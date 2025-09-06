using System.Collections;
using System.Text;
using UnityEngine;

public class MapViewPlayersList : MonoBehaviour
{
	[SerializeField] GameObject _playersListObj;
	[SerializeField] GameObject _wallPostCallFriendsBtn;
	[SerializeField] tk2dTextMesh _playersListLabel;
	[SerializeField] tk2dTextMesh _playersListLabelShadow;
	[SerializeField] tk2dUIScrollableArea _playerListArea;
	[SerializeField] float _onePlayerRecordSize = 0.15f;
	[SerializeField] int _playerListScrollSpeed = 20;
	[SerializeField] ShadowText _roomInfo;

	public void Social_AddWallPost_InviteToServer2()
	{
		string text = "#1";
		if (PhotonNetwork.room != null)
		{
			text = PhotonNetwork.room.Name;
		}
		JsSpeeker.I.WallPostSomething("invite_friends_to_server", text);
	}

	private void OnEnable()
	{
		if (!PhotonNetwork.offlineMode)
		{
			_playersListObj.SetActive(true);
			StartCoroutine("UpdatePlayerList");
		}
		else
		{
			_playersListObj.SetActive(false);
		}
		_wallPostCallFriendsBtn.SetActive(DataKeeper.GameType == GameType.Multiplayer);
	}

	private void OnDisable()
	{
		StopCoroutine("UpdatePlayerList");
	}

	private IEnumerator UpdatePlayerList()
	{
		while (true)
		{
			SetLabel();
			yield return new WaitForSeconds(3f);
		}
	}

	private void SetLabel()
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (WorldController.I.WorldPlayers.ContainsKey(playerList[i].NickName) && WorldController.I.WorldPlayers[playerList[i].NickName] != null)
			{
				stringBuilder.Append("^CF6BB38FF").Append(1 + i).Append(". ^CFFFFFFFF")
					.Append(WorldController.I.WorldPlayers[playerList[i].NickName].Name)
					.Append("\n");
				stringBuilder2.Append(1 + i).Append(". ").Append(WorldController.I.WorldPlayers[playerList[i].NickName].Name)
					.Append("\n");
			}
		}
		if (PhotonNetwork.room != null)
		{
			string text = PhotonNetwork.room.Name + "\n";
			bool flag = false;
			if (PhotonNetwork.room.CustomProperties.ContainsKey("zombies"))
			{
				flag = (bool)PhotonNetwork.room.CustomProperties["zombies"];
			}
			text = ((DataKeeper.Language != 0) ? (text + "World type: " + ((!flag) ? "Without zombies" : "With zombies")) : (text + "Тип мира: " + ((!flag) ? "Без зомби" : "С зомби")));
			_roomInfo.SetText(text);
		}
		else
		{
			_roomInfo.SetText(string.Empty);
		}
		_playersListLabel.text = stringBuilder.ToString();
		_playersListLabelShadow.text = stringBuilder2.ToString();
		_playerListArea.ContentLength = _onePlayerRecordSize * (float)playerList.Length;
		_playerListArea.scrollBar.buttonUpDownScrollDistance = 1f / (float)playerList.Length * (float)_playerListScrollSpeed;
	}

	private void Update()
	{
		if (ControlFreak2.CF2Input.GetKeyDown(KeyCode.T))
		{
			InvitePlayersToServer();
		}
	}

	private void InvitePlayersToServer()
	{
	}
}
