using UnityEngine;
using System.Collections;

public class RCCCamManager: MonoBehaviour {

	private RCCCarCamera carCamera;
	private RCCCameraOrbit orbitScript;
	public GameObject mainFixedCamera;
	[HideInInspector]
	public RCCMainFixedCam fixedCamScript;
	private RCCCockpit_Camera cockpitCamera;
	private RCCWheel_Camera wheelCamera;
	[HideInInspector]
	public float dist = 10f;
	[HideInInspector]
	public float height = 5f;
	[HideInInspector]
	public int cameraChangeCount = 0;
	[HideInInspector]
	public GameObject target;

	void Awake () {

		cameraChangeCount = 5;
	
		carCamera = GetComponent<RCCCarCamera>();
		orbitScript = GetComponent<RCCCameraOrbit>();

		if(mainFixedCamera)
			fixedCamScript = mainFixedCamera.GetComponent<RCCMainFixedCam>();

	}

	void Update () {

		if(Input.GetKeyDown(KeyCode.C))
			ChangeCamera();

	}

	public void ChangeCamera(){

		cameraChangeCount++;
		if(cameraChangeCount == 6)
			cameraChangeCount = 0;

		if(!target)
			return;

		carCamera.playerCar = target.transform;

		dist = target.GetComponent<RCCCarCameraConfig>().distance;
		height = target.GetComponent<RCCCarCameraConfig>().height;

		carCamera.distance = dist;
		carCamera.height = height;
		
		orbitScript.target = target.transform;
		orbitScript.distance = dist;

		cockpitCamera = target.GetComponentInChildren<RCCCockpit_Camera>();
		wheelCamera = target.GetComponentInChildren<RCCWheel_Camera>();
		
		if(fixedCamScript)
			fixedCamScript.player = target.transform;

		switch(cameraChangeCount){

		case 0:
			orbitScript.enabled = false;
			carCamera.enabled = true;
			carCamera.transform.parent = null;
			if(fixedCamScript)
				fixedCamScript.canTrackNow = false;
			break;
		case 1:
			orbitScript.enabled = true;
			carCamera.enabled = false;
			carCamera.transform.parent = null;
			if(fixedCamScript)
				fixedCamScript.canTrackNow = false;
			break;
		case 2:
			orbitScript.enabled = false;
			carCamera.enabled = false;
			carCamera.transform.parent = null;
			if(fixedCamScript)
				fixedCamScript.canTrackNow = false;
			break;
		case 3:
			orbitScript.enabled = false;
			carCamera.enabled = false;
			carCamera.transform.parent = target.transform;
			carCamera.transform.localPosition = cockpitCamera.transform.localPosition;
			carCamera.transform.localRotation = cockpitCamera.transform.localRotation;
			carCamera.GetComponent<Camera>().fieldOfView = 60;
			if(fixedCamScript)
				fixedCamScript.canTrackNow = false;
			break;
		case 4:
			orbitScript.enabled = false;
			carCamera.enabled = false;
			carCamera.transform.parent = target.transform;
			carCamera.transform.localPosition = wheelCamera.transform.localPosition;
			carCamera.transform.localRotation = wheelCamera.transform.localRotation;
			carCamera.GetComponent<Camera>().fieldOfView = 60;
			if(fixedCamScript)
				fixedCamScript.canTrackNow = false;
			break;
		case 5:
			orbitScript.enabled = false;
			carCamera.enabled = false;
			carCamera.transform.parent = null;
			if(fixedCamScript)
				fixedCamScript.canTrackNow = true;
			break;
		
		}

	}

}
