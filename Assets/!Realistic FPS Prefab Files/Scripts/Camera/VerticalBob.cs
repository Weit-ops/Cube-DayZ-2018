//VerticalBob.cs by Azuline Studios© All Rights Reserved 
using UnityEngine;
using System.Collections;

public class VerticalBob : MonoBehaviour {
	[HideInInspector]
	public GameObject playerObj;
	//Variables for vertical aspect of sine bob of camera and weapons
	//This script also makes camera view roll and pitch with bobbing
	private float timer = 0.0f;
	private float timerRoll = 0.0f;
	[HideInInspector]
	public float bobbingSpeed = 0.0f;
	//Vars for smoothing view position
	private float dampOrg = 0.0f;//Smoothed view postion to be passed to CameraKick script
	private float dampTo = 0.0f;
	private Vector3 tempLocalEulerAngles = new Vector3(0,0,0);
	//These two vars controlled from ironsights script
	//to allow different values for sprinting/walking ect.
	[HideInInspector]
	public float bobbingAmount = 0.0f;
	[HideInInspector]
	public float rollingAmount = 0.0f;
	[HideInInspector]
	public float yawingAmount = 0.0f;
	[HideInInspector]
	public float pitchingAmount = 0.0f;
	private float midpoint = 0.0f;//Vertical position of camera during sine bobbing
	private float idleYBob = 0.0f;
	[HideInInspector]
	public float translateChange = 0.0f;
	private float translateChangeRoll = 0.0f;
	private float translateChangePitch = 0.0f;
	private float translateChangeYaw = 0.0f;
	private float waveslice = 0.0f;
	private float wavesliceRoll = 0.0f;
	private float dampVelocity = 0.0f;
	private Vector3 dampVelocity2;
	private float totalAxes;
	private float horizontal;
	private float vertical;
	private float inputSpeed;

	private bool _isInitialized;
	private FPSRigidBodyWalker _fpsWalker;
	private Ironsights _ironsights;
	private FPSPlayer _fpsPlayer;
	private CameraKick _cameraKick;
	private Footsteps _footsteps;

	private void Awake()
	{
		//Set up external script references
		_fpsWalker = playerObj.GetComponent<FPSRigidBodyWalker>();
		_ironsights = playerObj.GetComponent<Ironsights>();
		_fpsPlayer = playerObj.GetComponent<FPSPlayer>();
		_cameraKick = Camera.main.GetComponent<CameraKick>();
		_footsteps = playerObj.GetComponent<Footsteps>();
		_isInitialized = true;
	}

	void Update (){
		if(_isInitialized && Time.timeScale > 0 && Time.deltaTime > 0){//allow pausing by setting timescale to 0
	
			waveslice = 0.0f;
			if(!_fpsPlayer.useAxisInput){
				horizontal = _fpsWalker.inputX;//get input from player movement script
				vertical = _fpsWalker.inputY;
			}else{
				horizontal = _fpsWalker.inputXSmoothed;//get input from player movement script
				vertical = _fpsWalker.inputYSmoothed;	
			}
			midpoint = _fpsWalker.midPos;//Start bob from view position set in player movement script
		
			if (Mathf.Abs(horizontal) != 0 || Mathf.Abs(vertical) != 0 && _fpsWalker.grounded){//Perform bob only when moving and grounded
		
				waveslice = Mathf.Sin(timer);
				wavesliceRoll = Mathf.Sin(timerRoll);

				if(Mathf.Abs(_fpsWalker.inputY) > 0.1f){
					inputSpeed = Mathf.Abs(_fpsWalker.inputY);
				}else{
					inputSpeed = Mathf.Abs(_fpsWalker.inputX);
				}	   

				timer = timer + bobbingSpeed * inputSpeed * Time.deltaTime;
				timerRoll = timerRoll + (bobbingSpeed / 2.0f) * Time.deltaTime;//Make view roll bob half the speed of view pitch bob
				
				if (timer > Mathf.PI * 2.0f){
					timer = timer - (Mathf.PI * 2.0f);
					if(!_fpsWalker.noClimbingSfx){//dont play climbing footsteps if noClimbingSfx is true
						_footsteps.FootstepSfx();//play footstep sound effect by calling FootstepSfx() function in Footsteps.cs
					}
				}
				
				//Perform bobbing of camera roll
				if (timerRoll > Mathf.PI * 2.0f){
					timerRoll = (timerRoll - (Mathf.PI * 2.0f));
					if (!_fpsWalker.grounded){
						timerRoll = 0;//reset timer when airborne to allow soonest resume of footstep sfx
					}
				}
			   
			}else{
				//reset variables to prevent view twitching when falling
				timer = 0.0f;
				timerRoll = 0.0f;
				tempLocalEulerAngles = new Vector3(0,0,0);//reset camera angles to 0 when stationary
			}
		
			if (waveslice != 0){
				
				translateChange = waveslice * bobbingAmount;
				translateChangePitch = waveslice * pitchingAmount;
				translateChangeRoll = wavesliceRoll * rollingAmount;
				translateChangeYaw = wavesliceRoll * yawingAmount;
				totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
				totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
				//needed for smooth return to neutral view pos
				translateChange = totalAxes * translateChange;
				translateChangePitch = totalAxes * translateChangePitch;
				translateChangeRoll = totalAxes * translateChangeRoll;
				//Set position for smoothing function and add jump value
				//divide position by deltaTime for framerate independence
				dampTo = midpoint + (translateChange / Time.deltaTime * 0.01f);
				//camera roll and pitch bob
				tempLocalEulerAngles = new Vector3(translateChangePitch, translateChangeYaw,translateChangeRoll);
				
			}else{
				
				if(!_fpsWalker.swimming){
					idleYBob = Mathf.Sin(Time.time * 1.25f) * 0.015f;
				}else{
					idleYBob = Mathf.Sin(Time.time * 1.25f) * 0.05f;//increase vertical bob when swimming
				}
				
				//reset variables to prevent view twitching when falling
				dampTo = midpoint + idleYBob;//add small sine bob for camera idle movement
				totalAxes = 0;
				translateChange = 0;
			}
			//use SmoothDamp to smooth position and remove any small glitches in bob amount 
			dampOrg = Mathf.SmoothDamp(dampOrg, dampTo, ref dampVelocity, 0.1f, Mathf.Infinity, Time.deltaTime);
			//Pass bobbing amount and angles to the camera kick script in the camera object after smoothing
			_cameraKick.dampOriginY = dampOrg;
			_cameraKick.bobAngles = Vector3.SmoothDamp(_cameraKick.bobAngles, tempLocalEulerAngles, ref dampVelocity2, 0.1f, Mathf.Infinity, Time.deltaTime);
		}
	}
}