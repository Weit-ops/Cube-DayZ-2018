using System.Collections;
using System.Text;
using UnityEngine;

public class ProfileInfo : MonoBehaviour
{
	[SerializeField]
	private GUIText _label;

	[SerializeField]
	private float _updateInfoTime = 3f;

	private void Awake()
	{
		StartCoroutine ("UpdateInfo");
	}

	private IEnumerator UpdateInfo()
	{
		if (DataKeeper.ShowProfileInfo)
		{
			StringBuilder str = new StringBuilder();
			if (ObjectsStatistic.instance != null)
			{
				str.Append("Console length: ").Append(ObjectsStatistic.instance.ConsoleLength).Append("\n");
			}
			if (WorldController.I != null)
			{
				str.Append("Reconnection: ").Append(WorldController.I.SuccessReconnectionCount).Append("/")
					.Append(WorldController.I.ReconnectionTryCount)
					.Append("\n");
			}
			if (PhotonNetwork.room != null)
			{
				if (PhotonNetwork.isMasterClient)
				{
					str.Append("I am MASTER!\n");
				}
				str.Append("Players in room: ").Append(PhotonNetwork.room.PlayerCount).Append("\n");
			}
			if (WorldController.I != null)
			{
				str.Append("Mobs count: ").Append(WorldController.I.ActiveMobCount).Append("\n");
			}
			str.Append("Connection state: ").Append(PhotonNetwork.connectionState);
			_label.text = str.ToString();
		}
		else
		{
			_label.text = string.Empty;
		}

		yield return new WaitForSeconds(_updateInfoTime);
		StartCoroutine("UpdateInfo");
	}
}
