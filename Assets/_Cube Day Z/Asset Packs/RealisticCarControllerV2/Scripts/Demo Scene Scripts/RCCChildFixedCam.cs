using UnityEngine;
using System.Collections;

public class RCCChildFixedCam : MonoBehaviour {

	[HideInInspector]
	public Transform player;

	void Start () {
	
	}

	void Update () {
		transform.LookAt(player);
	}

}
