using System.Collections;
using Photon;
using UnityEngine;

public class TwoTabsDetector : Photon.MonoBehaviour
{
	public static TwoTabsDetector I;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	private void OnEnable()
	{
		StartCoroutine("CheckSessionKey");
	}

	private void OnDisable()
	{
		StopCoroutine("CheckSessionKey");
	}

	private void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
	{
		if (newPlayer.NickName == DataKeeper.BackendInfo.user.user_id)
		{
			Debug.LogError("Two tabs detected! Exit to main menu! ");
			ExitFromServer();
		}
	}

	private void OnLeftRoom()
	{
		if (WorldController.I.LeaveRoomBySelf)
		{
			PhotonNetwork.LoadLevel("MainMenu");
		}
	}

	public void ExitFromServer()
	{
		RespawnMenuController.SetDieFlagFalse();
		WorldController.I.SavePlayer(DataKeeper.GameType == GameType.Single);
		if (PhotonNetwork.isMasterClient && WorldController.I.MultiplayerWorldLoaded && PhotonNetwork.room != null)
		{
			WorldController.I.SaveWorldInMultiplayer();
		}
		WorldController.I.LeaveRoomBySelf = true;
		WorldController.I.StopCoroutine("AddMobsForPulling");
		WorldController.I.StopCoroutine("AutoSaveMultiplayerWorld");
		if (PhotonNetwork.room != null)
		{
			PhotonNetwork.LeaveRoom();
		}
	}

	private IEnumerator CheckSessionKey()
	{
		while (true)
		{
			yield return new WaitForSeconds(10f);
			if (WorldController.I != null && JsSpeeker.I.CanGetKey)
			{
				JsSpeeker.I.VkStorageGet("ses_key");
			}
		}
	}
}
