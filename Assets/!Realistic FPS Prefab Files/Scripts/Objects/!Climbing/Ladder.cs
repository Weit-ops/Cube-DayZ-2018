//Ladder.cs by Azuline Studios© All Rights Reserved
//When attached to a trigger, this script can be used to create climbable surfaces.
using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {

	public bool playClimbingAudio = true;//if false, climbing footstep sounds won't be played
	private bool _hasPlayer;

	void OnTriggerStay ( Collider other ){
		if(other.gameObject.tag == "Player"){
			GameControls.I.Walker.climbing = true;
			//on start of a collision with ladder trigger set climbing var to true on FPSRigidBodyWalker script
			if (!playClimbingAudio)
			{
				GameControls.I.Walker.noClimbingSfx = true;
			}//dont play climbing sounds if playClimbingAudio is false
			_hasPlayer = true;
		}
	}
	
	void OnTriggerExit ( Collider other2  ){
		//on exit of a collision with ladder trigger set climbing var to false on FPSRigidBodyWalker script
		if(other2.gameObject.tag == "Player"){
			GameControls.I.Walker.climbing = false;
			//prevent player from jumping when leaving surface if they did so by holding jump button
			GameControls.I.Walker.noClimbingSfx = false;
			_hasPlayer = false;
		}
	}

	private void OnDestroy()
	{
		if (_hasPlayer)
		{
			GameControls.I.Walker.climbing = false;
			GameControls.I.Walker.noClimbingSfx = false;

			_hasPlayer = false;
		}
	}
}