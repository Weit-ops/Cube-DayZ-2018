using CodeStage.AntiCheat.Detectors;
using UnityEngine;

public class AntiCheatsController : MonoBehaviour
{
	private void Start()
	{
		SpeedHackDetector.StartDetection(OnCheatDetected);
	}

	private void OnCheatDetected()
	{
		Debug.Log("Cheat detected!");
		if (WorldController.I != null)
		{
			WorldController.I.SavePlayer(DataKeeper.GameType == GameType.Single);
			WorldController.I.LeaveRoomBySelf = true;
			PhotonNetwork.LeaveRoom();
			PhotonNetwork.LoadLevel("MainMenu");
		}
		else
		{
			PhotonNetwork.LeaveRoom();
			PhotonNetwork.LoadLevel("MainMenu");
		}
	}

	private void OnDestroy()
	{
		SpeedHackDetector.StopDetection();
	}
}
