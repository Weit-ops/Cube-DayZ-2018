using UnityEngine;
using System.Collections;

public class RCCEnterExitCamera : MonoBehaviour {
	
	public float maxRayDistance= 2.0f; 
	private bool  showGui = false;
	
	void  Update (){
		
		Vector3 direction= gameObject.transform.TransformDirection(Vector3.forward);
		RaycastHit hit;
		if (Physics.Raycast(transform.position, direction, out hit, maxRayDistance)) {
			
			showGui = true;
			
			if(Input.GetKeyDown(KeyCode.E)) {
				hit.collider.gameObject.SendMessage("Action", SendMessageOptions.DontRequireReceiver);
			}
			
		}else{
			showGui = false;
		}
		
	}
	
	void  OnGUI (){
		
		if(showGui){
			GUI.Label( new Rect(Screen.width - (Screen.width/1.7f),Screen.height - (Screen.height/1.4f),800,100),"Press ''E'' key to Use");	
		}
		
	}
	
}