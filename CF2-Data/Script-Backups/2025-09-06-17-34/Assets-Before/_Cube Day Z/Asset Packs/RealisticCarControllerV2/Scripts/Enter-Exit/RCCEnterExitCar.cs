using UnityEngine;
using System.Collections;

public class RCCEnterExitCar : MonoBehaviour {

	public GameObject mainCamera;			
	public GameObject vehicle;
	public GameObject FPSPlayer;
	public Transform getOutPosition;
	private bool  opened = false;
	private float waitTime = 1f;
	private bool  temp = false;
	
	void  Start (){
		mainCamera.GetComponent<Camera>().enabled = false;
		mainCamera.GetComponent<AudioListener>().enabled = false;  
		vehicle.GetComponent<RCCCarControllerV2>().canControl = false;
	}
	
	void  Update (){
		if ((Input.GetKeyDown(KeyCode.E)) && opened && !temp){
			GetOut();
			opened = false;
			temp = false;
		}
	}
	
	IEnumerator  Action (){
		if (!opened && !temp){
			GetIn();
			opened = true;
			temp = true;
			yield return new WaitForSeconds(waitTime);
			temp = false;
		}
	}
	
	void  GetIn (){
		mainCamera.transform.GetComponent<RCCCarCamera>().playerCar = vehicle.transform;
		FPSPlayer.SetActive(false);
		FPSPlayer.transform.parent = vehicle.transform;
		FPSPlayer.transform.localPosition = Vector3.zero;
		mainCamera.GetComponent<Camera>().enabled = true;
		vehicle.GetComponent<RCCCarCameraConfig>().enabled = true;
		vehicle.GetComponent<RCCCarControllerV2>().canControl = true;
		mainCamera.GetComponent<AudioListener>().enabled = true;
		vehicle.SendMessage("KillOrStartEngine", 1);
	}
	
	void  GetOut (){
		FPSPlayer.transform.parent = null;
		FPSPlayer.transform.position = getOutPosition.transform.position;
		FPSPlayer.transform.rotation = Quaternion.identity;
		FPSPlayer.SetActive(true);
		mainCamera.GetComponent<Camera>().enabled = false;
		vehicle.GetComponent<RCCCarCameraConfig>().enabled = false;
		mainCamera.GetComponent<AudioListener>().enabled = false;
		vehicle.GetComponent<RCCCarControllerV2>().canControl = false;
	}
	
}