using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent (typeof(Rigidbody))]

public class RCCCarControllerV2 : MonoBehaviour {

	private Rigidbody rigid;

	//Mobile Controller.
	public bool mobileController = false;
	
	//Mobile Controller Type.
	public MobileGUIType _mobileControllerType;
	public enum MobileGUIType
	{
		NGUIController,
		UIController
	}
	
	//Dashboard Type.
	[HideInInspector]
	public bool NGUIDashboard = false, UnityDashboard = true;
	public DashboardType _dashboardType;
	public enum DashboardType
	{
		UnityDashboard,
		NGUIDashboard
	}
	
	public bool useAccelerometerForSteer = false, steeringWheelControl = false;
	public float gyroTiltMultiplier = 2.0f;
	public bool demoGUI = false;
	public bool dashBoard = false;
	private Vector3 defbrakePedalPosition;
	private bool mobileHandbrake = false;
	
	
	//NGUI Controller Elements.
	public RCCNGUIController gasPedal, brakePedal, leftArrow, rightArrow, handbrakeGui, boostGui;
	//UI Controller Elements.
	public RCCUIController gasPedalUI, brakePedalUI, leftArrowUI, rightArrowUI, handbrakeUI, boostUI;
	
	// Wheel Transforms Of The Vehicle.
	public Transform FrontLeftWheelTransform;
	public Transform FrontRightWheelTransform;
	public Transform RearLeftWheelTransform;
	public Transform RearRightWheelTransform;
	
	//Wheel Colliders Of The Vehicle.
	public WheelCollider FrontLeftWheelCollider;
	public WheelCollider FrontRightWheelCollider;
	public WheelCollider RearLeftWheelCollider;
	public WheelCollider RearRightWheelCollider;
	
	//Extra Wheels.
	public Transform[] ExtraRearWheelsTransform;
	public WheelCollider[] ExtraRearWheelsCollider;
	
	// Driver Steering Wheel.
	public Transform SteeringWheel;
	
	// Set wheel drive of the vehicle. If you are using rwd, you have to be careful with your rear wheel collider
	// settings and com of the vehicle. Otherwise, vehicle will behave like a toy. ***My advice is use fwd always***
	[HideInInspector]
	public bool rwd = false, fwd = true;
	public WheelType _wheelTypeChoise;
	public enum WheelType
	{
		FWD, RWD
	}
	
	//Center of mass.
	public Transform COM;
	
	// Drift Configurations.
	private int steeringAssistanceDivider = 5;
	private float driftAngle;
	
	//Vehicle Mecanim.
	public bool canControl = true;
	public bool driftMode = false;
	public bool autoReverse = false;
	public bool automaticGear = true;
	private bool canGoReverseNow = false;
	
	public AnimationCurve[] engineTorqueCurve;
	[HideInInspector]
	public float[] gearSpeed;
	public float engineTorque = 2500.0f;
	public float maxEngineRPM = 6000.0f;
	public float minEngineRPM = 1000.0f;
	public float steerAngle = 40.0f;
	public float highspeedsteerAngle = 10.0f;
	public float highspeedsteerAngleAtspeed = 100.0f;
	public float antiRoll = 10000.0f;
	[HideInInspector]
	public float speed;
	public float brake = 4000.0f;
	public float maxspeed = 180.0f;
	public bool useDifferantial = true;
	private float differantialRatioRight;
	private float differantialRatioLeft;
	private float differantialDifference;
	private float resetTime = 0f;
	
	//Gears.
	public int currentGear;
	public int totalGears = 6;
	private int _totalGears
	{
		get
		{
			return totalGears - 1;
		}
	}
	
	public bool changingGear = false;
	public float gearShiftRate = 10.0f;
	
	// Each Wheel Transform's Rotation Value.
	private float _rotationValueFL, _rotationValueFR, _rotationValueRL, _rotationValueRR;
	private float[] RotationValueExtra;
	
	//Wheel Stiffness.
	private float defsteerAngle;
	private float _forwardStiffnessFL;
	private float _forwardStiffnessFR;
	private float _forwardStiffnessRL;
	private float _forwardStiffnessRR;
	private float _stiffnessFL;
	private float _stiffnessFR;
	private float _stiffnessRL;
	private float _stiffnessRR;
	
	//Private Bools.
	private bool reversing = false;
	private bool headLightsOn = false;
	private float acceleration = 0f;
	private float lastVelocity = 0f;
	private float gearTimeMultiplier;
	
	//Audio.
	private AudioSource skidSound;
	public AudioClip skidClip;
	private AudioSource crashSound;
	public AudioClip[] crashClips;
	private AudioSource engineStartSound;
	public AudioClip engineStartClip;
	private AudioSource engineSound;
	public AudioClip engineClip;
	private AudioSource gearShiftingSound;
	public AudioClip[] gearShiftingClips;
	
	//Collision Force Limit.
	private int collisionForceLimit = 5;
	
	//Inputs.
	[HideInInspector]
	public float motorInput = 0f;
	[HideInInspector]
	public float steerInput = 0f;
	[HideInInspector]
	public float boostInput = 1.0f;
	[HideInInspector]
	public float EngineRPM = 0f;
	
	//UI DashBoard.
	public RCCDashboardInputs UIInputs;
	public RectTransform RPMNeedle;
	public RectTransform KMHNeedle;
	private float RPMNeedleRotation = 0.0f;
	private float KMHNeedleRotation = 0.0f;
	private float smoothedNeedleRotation = 0.0f;
	
	//NGUI Dashboard.
	public RCCDashboardInputs NGUIInputs;
	public GameObject NGUIRPMNeedle;
	public GameObject NGUIKMHNeedle;
	public float minimumRPMNeedleAngle = 20.0f;
	public float maximumRPMNeedleAngle = 160.0f;
	public float minimumKMHNeedleAngle = -25.0f;
	public float maximumKMHNeedleAngle = 155.0f;
	
	//Smokes.
	public GameObject wheelSlipPrefab;
	private String wheelSlipPrefabName;
	private List <GameObject> _wheelParticles = new List<GameObject>();
	public ParticleSystem normalExhaustGas;
	public ParticleSystem heavyExhaustGas;
	
	//Chassis Simulation.
	public GameObject chassis;
	public float chassisVerticalLean = 4.0f;
	public float chassisHorizontalLean = 4.0f;
	private float horizontalLean = 0.0f;
	private float verticalLean = 0.0f;
	
	//Lights.
	public Light[] headLights;
	public Light[] brakeLights;
	public Light[] reverseLights;
	
	//Steering Wheel Controller.
	public float steeringWheelMaximumsteerAngle = 180.0f;
	public float steeringWheelGuiScale = 256.0f;
	public float steeringWheelXOffset = 30.0f;
	public float steeringWheelYOffset = 30.0f;
	public Vector2 steeringWheelPivotPos = Vector2.zero;
	public float steeringWheelResetPosspeed = 200.0f;
	public Texture2D steeringWheelTexture;
	private float steeringWheelsteerAngle ;
	private bool  steeringWheelIsTouching;
	private Rect steeringWheelTextureRect;
	private Vector2 steeringWheelWheelCenter;
	private float steeringWheelOldAngle;
	private int touchId = -1;
	private Vector2 touchPos;
	
	
	
	void Start (){

		SoundsInitialize();
		TypeInit();
		MobileGUI();
		SteeringWheelInit();
		if(wheelSlipPrefab)
			SmokeInit();

		rigid = GetComponent<Rigidbody>();
		
		Time.fixedDeltaTime = .02f;
		rigid.maxAngularVelocity = 5f;
		RotationValueExtra = new float[ExtraRearWheelsCollider.Length];
		defsteerAngle = steerAngle;
		
	}

	public AudioSource CreateAudioSource(string audioName, float minDistance, float volume, AudioClip audioClip, bool loop, bool playNow, bool destroyAfterFinished){

		GameObject audioSource = new GameObject(audioName);
		audioSource.transform.position = transform.position;
		audioSource.transform.rotation = transform.rotation;
		audioSource.transform.parent = transform;
		audioSource.AddComponent<AudioSource>();
		audioSource.GetComponent<AudioSource>().minDistance = minDistance;
		audioSource.GetComponent<AudioSource>().volume = volume;
		audioSource.GetComponent<AudioSource>().clip = audioClip;
		audioSource.GetComponent<AudioSource>().loop = loop;
		audioSource.GetComponent<AudioSource>().spatialBlend = 1f;

		if(playNow)
			audioSource.GetComponent<AudioSource>().Play();

		if(destroyAfterFinished)
			Destroy(audioSource, audioClip.length);

		return audioSource.GetComponent<AudioSource>();

	}
	
	public void CreateWheelColliders (){
		
		List <Transform> allWheelTransforms = new List<Transform>();
		allWheelTransforms.Add(FrontLeftWheelTransform); allWheelTransforms.Add(FrontRightWheelTransform); allWheelTransforms.Add(RearLeftWheelTransform); allWheelTransforms.Add(RearRightWheelTransform);
		
		if(allWheelTransforms[0] == null){
			Debug.LogError("You haven't choose your Wheel Transforms. Please select all of your Wheel Transforms before creating Wheel Colliders. Script needs to know their positions, aye?");
			return;
		}
		
		transform.rotation = Quaternion.identity;
		
		GameObject WheelColliders = new GameObject("Wheel Colliders");
		WheelColliders.transform.parent = transform;
		WheelColliders.transform.rotation = transform.rotation;
		WheelColliders.transform.localPosition = Vector3.zero;
		WheelColliders.transform.localScale = Vector3.one;
		
		foreach(Transform wheel in allWheelTransforms){
			
			GameObject wheelcollider = new GameObject(wheel.transform.name); 
			
			wheelcollider.transform.position = wheel.transform.position;
			wheelcollider.transform.rotation = transform.rotation;
			wheelcollider.transform.name = wheel.transform.name;
			wheelcollider.transform.parent = WheelColliders.transform;
			wheelcollider.transform.localScale = Vector3.one;
			wheelcollider.layer = LayerMask.NameToLayer("Wheel");
			wheelcollider.AddComponent<WheelCollider>();
			wheelcollider.GetComponent<WheelCollider>().radius = (wheel.GetComponent<MeshRenderer>().bounds.size.y / 2f) / transform.localScale.y;

			wheelcollider.AddComponent<RCCWheelSkidmarks>();
			wheelcollider.GetComponent<RCCWheelSkidmarks>().vehicle = GetComponent<RCCCarControllerV2>();
			
			JointSpring spring = wheelcollider.GetComponent<WheelCollider>().suspensionSpring;

			spring.spring = 35000f;
			spring.damper = 2000f;

			wheelcollider.GetComponent<WheelCollider>().suspensionSpring = spring;
			wheelcollider.GetComponent<WheelCollider>().suspensionDistance = .25f;
			wheelcollider.GetComponent<WheelCollider>().forceAppPointDistance = .25f;
			wheelcollider.GetComponent<WheelCollider>().mass = 100f;
			wheelcollider.GetComponent<WheelCollider>().wheelDampingRate = 1f;
			
			WheelFrictionCurve sidewaysFriction = wheelcollider.GetComponent<WheelCollider>().sidewaysFriction;
			WheelFrictionCurve forwardFriction = wheelcollider.GetComponent<WheelCollider>().forwardFriction;

			forwardFriction.extremumSlip = .4f;
			forwardFriction.extremumValue = 1;
			forwardFriction.asymptoteSlip = .8f;
			forwardFriction.asymptoteValue = .75f;
			forwardFriction.stiffness = 1.75f;

			sidewaysFriction.extremumSlip = .25f;
			sidewaysFriction.extremumValue = 1;
			sidewaysFriction.asymptoteSlip = .5f;
			sidewaysFriction.asymptoteValue = .75f;
			sidewaysFriction.stiffness = 2f;

			wheelcollider.GetComponent<WheelCollider>().sidewaysFriction = sidewaysFriction;
			wheelcollider.GetComponent<WheelCollider>().forwardFriction = forwardFriction;

		}
		
		WheelColliders.layer = LayerMask.NameToLayer("Wheel");
		
		WheelCollider[] allWheelColliders = new WheelCollider[allWheelTransforms.Count];
		allWheelColliders = GetComponentsInChildren<WheelCollider>();
		
		FrontLeftWheelCollider = allWheelColliders[0];
		FrontRightWheelCollider = allWheelColliders[1];
		RearLeftWheelCollider = allWheelColliders[2];
		RearRightWheelCollider = allWheelColliders[3];
		
	}
	
	public void SoundsInitialize (){

		engineSound = CreateAudioSource("engineSound", 5, 0, engineClip, true, true, false);
		skidSound = CreateAudioSource("skidSound", 5, 0, skidClip, true, true, false);
		
	}
	
	public void KillOrStartEngine (int i){
		
		if(i == 0){
			canControl = false;
		}else{
			canControl = true;
			StartEngineSound();
		}
		
	}
	
	public void StartEngineSound (){

		engineStartSound = CreateAudioSource("engineStartAudio", 5, 1, engineStartClip, false, true, true);
		
	}
	
	public void TypeInit (){
		
		switch(_wheelTypeChoise){
		case WheelType.FWD:
			fwd = true;
			rwd = false;
			break;
		case WheelType.RWD:
			fwd = false;
			rwd = true;
			break;
		}
		
		switch(_dashboardType){
		case DashboardType.NGUIDashboard:
			NGUIDashboard = true;
			UnityDashboard = false;
			break;
		case DashboardType.UnityDashboard:
			NGUIDashboard = false;
			UnityDashboard = true;
			break;
		}
		
	}
	
	public void SteeringWheelInit (){
		
		steeringWheelGuiScale = (Screen.width * 1.0f) / 2.7f;
		steeringWheelIsTouching = false;
		steeringWheelTextureRect = new Rect( steeringWheelXOffset + (steeringWheelGuiScale / Screen.width ), -steeringWheelYOffset + (Screen.height - (steeringWheelGuiScale)), steeringWheelGuiScale, steeringWheelGuiScale );
		steeringWheelWheelCenter = new Vector2( steeringWheelTextureRect.x + steeringWheelTextureRect.width * 0.5f, Screen.height - steeringWheelTextureRect.y - steeringWheelTextureRect.height * 0.5f );
		steeringWheelsteerAngle  = 0f;
		
	}
	
	public void SmokeInit (){
		
		wheelSlipPrefabName = wheelSlipPrefab.name+"(Clone)";
		
		for(int i = 0; i < 4; i++){
			Instantiate(wheelSlipPrefab, transform.position, transform.rotation);
		}
		
		foreach(GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
		{
			if(go.name == wheelSlipPrefabName)
				_wheelParticles.Add (go);
		} 
		
		_wheelParticles[0].transform.position = FrontRightWheelCollider.transform.position;
		_wheelParticles[1].transform.position = FrontLeftWheelCollider.transform.position;
		_wheelParticles[2].transform.position = RearRightWheelCollider.transform.position;
		_wheelParticles[3].transform.position = RearLeftWheelCollider.transform.position;
		
		_wheelParticles[0].transform.parent = FrontRightWheelCollider.transform;
		_wheelParticles[1].transform.parent = FrontLeftWheelCollider.transform;
		_wheelParticles[2].transform.parent = RearRightWheelCollider.transform;
		_wheelParticles[3].transform.parent = RearLeftWheelCollider.transform;
		
	}
	
	public void MobileGUI (){

		if(mobileController){
			if(_mobileControllerType == MobileGUIType.NGUIController)
				defbrakePedalPosition = brakePedal.transform.position;
			else
				defbrakePedalPosition = brakePedalUI.transform.position;
		}
		
	}
	
	void Update (){
		
		if(canControl){
			if(mobileController){
				if(_mobileControllerType == MobileGUIType.NGUIController)
					NGUIControlling();
				if(_mobileControllerType == MobileGUIType.UIController)
					UIControlling();
				MobileSteeringInputs();
				if(steeringWheelControl)
					SteeringWheelControlling();
			}else{
				KeyboardControlling();
			}
			Lights();
			ResetCar();
			ShiftGears();


		}
		

		WheelAlign();
		SkidAudio();
		WheelCamber();
		if(chassis)
			Chassis();
		
		if(dashBoard && canControl)
			DashboardGUI();
		
	}
	
	void FixedUpdate (){

		Braking();
		Differantial();
		AntiRollBars();
		
		if(wheelSlipPrefab)
			SmokeInstantiateRate();
		
		if(canControl){
			Engine();
		}else{
			RearLeftWheelCollider.motorTorque = 0;
			RearRightWheelCollider.motorTorque = 0;
			FrontLeftWheelCollider.motorTorque = 0;
			FrontRightWheelCollider.motorTorque = 0;
			RearLeftWheelCollider.brakeTorque = brake/12f;
			RearRightWheelCollider.brakeTorque = brake/12f;
			FrontLeftWheelCollider.brakeTorque = brake/12f;
			FrontRightWheelCollider.brakeTorque = brake/12f;
			engineSound.GetComponent<AudioSource>().volume = Mathf.Lerp(engineSound.GetComponent<AudioSource>().volume, 0f, Time.deltaTime);
			engineSound.GetComponent<AudioSource>().pitch = Mathf.Lerp(engineSound.GetComponent<AudioSource>().pitch, 0f, Time.deltaTime);
		}
		
	}
	
	public void Engine (){
		
		//speed.
		speed = rigid.velocity.magnitude * 3.0f;
		
		//Acceleration Calculation.
		acceleration = 0f;
		acceleration = (transform.InverseTransformDirection(rigid.velocity).z - lastVelocity) / Time.fixedDeltaTime;
		lastVelocity = transform.InverseTransformDirection(rigid.velocity).z;
		
		//Drag Limit Depends On Vehicle Acceleration.
		rigid.drag = Mathf.Clamp((acceleration / 50f), 0f, 1f);
		
		//Steer Limit.
		steerAngle = Mathf.Lerp(defsteerAngle, highspeedsteerAngle, (speed / highspeedsteerAngleAtspeed));
		
		//Engine RPM.
		EngineRPM = Mathf.Clamp((((Mathf.Abs((RearLeftWheelCollider.rpm + RearRightWheelCollider.rpm)) * gearShiftRate) + minEngineRPM)) / (currentGear+1), minEngineRPM, maxEngineRPM);
		
		//Reversing Bool.
		if(motorInput <= 0  && RearLeftWheelCollider.rpm < 20 && canGoReverseNow)
			reversing = true;
		else
			reversing = false;
		
		//Auto Reverse Bool.
		if(autoReverse){
			canGoReverseNow = true;
		}else{
			if(motorInput >= -.1f && speed < 5)
				canGoReverseNow = true;
			else if(motorInput < 0 && transform.InverseTransformDirection(rigid.velocity).z > 1) 
				canGoReverseNow = false;
		}
		
		//Engine Audio Volume.
		if(engineSound){
		if(!reversing)
			engineSound.GetComponent<AudioSource>().volume = Mathf.Lerp (engineSound.GetComponent<AudioSource>().volume, Mathf.Clamp (motorInput, .35f, .85f), Time.deltaTime*  5);
		else
			engineSound.GetComponent<AudioSource>().volume = Mathf.Lerp (engineSound.GetComponent<AudioSource>().volume, Mathf.Clamp (Mathf.Abs(motorInput), .35f, .85f), Time.deltaTime*  5);
		
		engineSound.GetComponent<AudioSource>().pitch = Mathf.Lerp ( engineSound.GetComponent<AudioSource>().pitch, Mathf.Lerp (1f, 2f, (EngineRPM - minEngineRPM / 1.5f) / (maxEngineRPM + minEngineRPM)), Time.deltaTime * 5);
		}
		
		#region Wheel Type Motor Torque.
		
		if(rwd){
			
			if(speed > maxspeed || Mathf.Abs(RearLeftWheelCollider.rpm) > 3000 || Mathf.Abs(RearRightWheelCollider.rpm) > 3000){
				RearLeftWheelCollider.motorTorque = 0;
				RearRightWheelCollider.motorTorque = 0;
			}else if(!reversing){
				RearLeftWheelCollider.motorTorque = (engineTorque) * (Mathf.Clamp(motorInput * differantialRatioLeft, 0f, 1f) * boostInput) * engineTorqueCurve[currentGear].Evaluate(speed);
				RearRightWheelCollider.motorTorque = (engineTorque) * (Mathf.Clamp(motorInput * differantialRatioRight, 0f, 1f) * boostInput) * engineTorqueCurve[currentGear].Evaluate(speed);
			}
			if(reversing){
				if(speed < 30 && Mathf.Abs(RearLeftWheelCollider.rpm) < 3000 && Mathf.Abs(RearRightWheelCollider.rpm) < 3000){
					RearLeftWheelCollider.motorTorque = ((engineTorque)  * motorInput);
					RearRightWheelCollider.motorTorque = ((engineTorque)  * motorInput);
				}else{
					RearLeftWheelCollider.motorTorque = 0;
					RearRightWheelCollider.motorTorque = 0;
				}
			}
			
		}
		
		if(fwd){
			
			if(speed > maxspeed || Mathf.Abs(FrontLeftWheelCollider.rpm) > 3000 || Mathf.Abs(FrontRightWheelCollider.rpm) > 3000){
				FrontLeftWheelCollider.motorTorque = 0;
				FrontRightWheelCollider.motorTorque = 0;
			}else if(!reversing){
				FrontLeftWheelCollider.motorTorque = (engineTorque) * (Mathf.Clamp(motorInput * differantialRatioLeft, 0f, 1f) * boostInput) * engineTorqueCurve[currentGear].Evaluate(speed);
				FrontRightWheelCollider.motorTorque = (engineTorque) * (Mathf.Clamp(motorInput * differantialRatioRight, 0f, 1f) * boostInput) * engineTorqueCurve[currentGear].Evaluate(speed);
			}
			if(reversing){
				if(speed < 30 && Mathf.Abs(FrontLeftWheelCollider.rpm) < 3000 && Mathf.Abs(FrontRightWheelCollider.rpm) < 3000){
					FrontLeftWheelCollider.motorTorque = ((engineTorque)  * motorInput);
					FrontRightWheelCollider.motorTorque = ((engineTorque)  * motorInput);
				}else{
					FrontLeftWheelCollider.motorTorque = 0;
					FrontRightWheelCollider.motorTorque = 0;
				}
			}
			
		}
		
		#endregion Wheel Type
		
	}
	
	public void Braking (){
		
		//Handbrake
		if(ControlFreak2.CF2Input.GetButton("Jump") || mobileHandbrake){

			//FrontLeftWheelCollider.brakeTorque = (brake / 2.5f);
			//FrontRightWheelCollider.brakeTorque = (brake / 2.5f);
			RearLeftWheelCollider.brakeTorque = (brake);
			RearRightWheelCollider.brakeTorque = (brake);
			
		//Normal brake
		}else{
			
			// Deacceleration.
			if(Mathf.Abs (motorInput) <= .05f && !changingGear){
				RearLeftWheelCollider.brakeTorque = (brake) / 25f;
				RearRightWheelCollider.brakeTorque = (brake) / 25f;
				FrontLeftWheelCollider.brakeTorque = (brake) / 25f;
				FrontRightWheelCollider.brakeTorque = (brake) / 25f;
			// Braking.
			}else if(motorInput < 0 && !reversing){
				FrontLeftWheelCollider.brakeTorque = (brake) * (Mathf.Abs(motorInput));
				FrontRightWheelCollider.brakeTorque = (brake) * (Mathf.Abs(motorInput));
				RearLeftWheelCollider.brakeTorque = (brake) * (Mathf.Abs(motorInput / 2f));
				RearRightWheelCollider.brakeTorque = (brake) * (Mathf.Abs(motorInput / 2f));
			}else{
				RearLeftWheelCollider.brakeTorque = 0;
				RearRightWheelCollider.brakeTorque = 0;
				FrontLeftWheelCollider.brakeTorque = 0;
				FrontRightWheelCollider.brakeTorque = 0;
			}
			
		}
		
	}
	
	public void Differantial (){
		
		if(useDifferantial){
			
			if(fwd){
				differantialDifference = Mathf.Clamp ( Mathf.Abs (FrontRightWheelCollider.rpm) - Mathf.Abs (FrontLeftWheelCollider.rpm), -50f, 50f );
				differantialRatioRight = Mathf.Lerp ( 0f, 1f, ( (((Mathf.Abs (FrontRightWheelCollider.rpm) + Mathf.Abs (FrontLeftWheelCollider.rpm)) + 10 / 2 ) + differantialDifference) /  (Mathf.Abs (FrontRightWheelCollider.rpm) + Mathf.Abs (FrontLeftWheelCollider.rpm))) );
				differantialRatioLeft = Mathf.Lerp ( 0f, 1f, ( (((Mathf.Abs (FrontRightWheelCollider.rpm) + Mathf.Abs (FrontLeftWheelCollider.rpm)) + 10 / 2 ) - differantialDifference) /  (Mathf.Abs (FrontRightWheelCollider.rpm) + Mathf.Abs (FrontLeftWheelCollider.rpm))) );
			}
			if(rwd){
				differantialDifference = Mathf.Clamp ( Mathf.Abs (RearRightWheelCollider.rpm) - Mathf.Abs (RearLeftWheelCollider.rpm), -50f, 50f );
				differantialRatioRight = Mathf.Lerp ( 0f, 1f, ( (((Mathf.Abs (RearRightWheelCollider.rpm) +  Mathf.Abs (RearLeftWheelCollider.rpm)) + 10 / 2 ) + differantialDifference) /  (Mathf.Abs (RearRightWheelCollider.rpm) + Mathf.Abs (RearLeftWheelCollider.rpm))) );
				differantialRatioLeft = Mathf.Lerp ( 0f, 1f, ( (((Mathf.Abs (RearRightWheelCollider.rpm) +  Mathf.Abs (RearLeftWheelCollider.rpm)) + 10 / 2 ) - differantialDifference) /  (Mathf.Abs (RearRightWheelCollider.rpm) + Mathf.Abs (RearLeftWheelCollider.rpm))) );
			}
			
		}else{
			
			differantialRatioRight = 1;
			differantialRatioLeft = 1;
			
		}
		
	}
	
	public void AntiRollBars (){
		
		WheelHit FrontWheelHit;
		
		float travelFL = 1.0f;
		float travelFR = 1.0f;
		
		bool groundedFL= FrontLeftWheelCollider.GetGroundHit(out FrontWheelHit);
		
		if (groundedFL)
			travelFL = (-FrontLeftWheelCollider.transform.InverseTransformPoint(FrontWheelHit.point).y - FrontLeftWheelCollider.radius) / FrontLeftWheelCollider.suspensionDistance;
		
		bool groundedFR= FrontRightWheelCollider.GetGroundHit(out FrontWheelHit);
		
		if (groundedFR)
			travelFR = (-FrontRightWheelCollider.transform.InverseTransformPoint(FrontWheelHit.point).y - FrontRightWheelCollider.radius) / FrontRightWheelCollider.suspensionDistance;
		
		float antiRollForceFront= (travelFL - travelFR) * antiRoll;
		
		if (groundedFL)
			rigid.AddForceAtPosition(FrontLeftWheelCollider.transform.up * -antiRollForceFront, FrontLeftWheelCollider.transform.position); 
		if (groundedFR)
			rigid.AddForceAtPosition(FrontRightWheelCollider.transform.up * antiRollForceFront, FrontRightWheelCollider.transform.position); 
		
		WheelHit RearWheelHit;
		
		float travelRL = 1.0f;
		float travelRR = 1.0f;
		
		bool groundedRL= RearLeftWheelCollider.GetGroundHit(out RearWheelHit);
		
		if (groundedRL)
			travelRL = (-RearLeftWheelCollider.transform.InverseTransformPoint(RearWheelHit.point).y - RearLeftWheelCollider.radius) / RearLeftWheelCollider.suspensionDistance;
		
		bool groundedRR= RearRightWheelCollider.GetGroundHit(out RearWheelHit);
		
		if (groundedRR)
			travelRR = (-RearRightWheelCollider.transform.InverseTransformPoint(RearWheelHit.point).y - RearRightWheelCollider.radius) / RearRightWheelCollider.suspensionDistance;
		
		float antiRollForceRear= (travelRL - travelRR) * antiRoll;
		
		if (groundedRL)
			rigid.AddForceAtPosition(RearLeftWheelCollider.transform.up * -antiRollForceRear, RearLeftWheelCollider.transform.position); 
		if (groundedRR)
			rigid.AddForceAtPosition(RearRightWheelCollider.transform.up * antiRollForceRear, RearRightWheelCollider.transform.position);

		if (groundedRR && groundedRL)
			rigid.AddRelativeTorque((Vector3.up * (steerInput)) * 5000f);
		
	}
	
	public void MobileSteeringInputs (){
		
		//Accelerometer Inputs.
		if(useAccelerometerForSteer){
			
			steerInput = Input.acceleration.x * gyroTiltMultiplier;
			
			if(!driftMode){
				FrontLeftWheelCollider.steerAngle = Mathf.Clamp((steerAngle * steerInput), -steerAngle, steerAngle);
				FrontRightWheelCollider.steerAngle = Mathf.Clamp((steerAngle * steerInput), -steerAngle, steerAngle);
			}else{
				FrontLeftWheelCollider.steerAngle = Mathf.Clamp((steerAngle * steerInput), -steerAngle, steerAngle);
				FrontRightWheelCollider.steerAngle = Mathf.Clamp((steerAngle * steerInput), -steerAngle, steerAngle);
			}
			
		}else{
			
			//TouchScreen Inputs.
			if(!steeringWheelControl){
				
				if(!driftMode){
					FrontLeftWheelCollider.steerAngle = Mathf.Clamp((steerAngle * steerInput), -steerAngle, steerAngle);
					FrontRightWheelCollider.steerAngle = Mathf.Clamp((steerAngle * steerInput), -steerAngle, steerAngle);
				}else{
					FrontLeftWheelCollider.steerAngle = Mathf.Clamp((steerAngle * steerInput), -steerAngle, steerAngle);
					FrontRightWheelCollider.steerAngle = Mathf.Clamp((steerAngle * steerInput), -steerAngle, steerAngle);
				}
				
				//SteeringWheel Inputs.
			}else{
				
				if(!driftMode){
					FrontLeftWheelCollider.steerAngle = (steerAngle * (-steeringWheelsteerAngle / steeringWheelMaximumsteerAngle));
					FrontRightWheelCollider.steerAngle = (steerAngle * (-steeringWheelsteerAngle / steeringWheelMaximumsteerAngle));
				}else{
					FrontLeftWheelCollider.steerAngle = (steerAngle * (-steeringWheelsteerAngle / steeringWheelMaximumsteerAngle));
					FrontRightWheelCollider.steerAngle = (steerAngle * (-steeringWheelsteerAngle / steeringWheelMaximumsteerAngle));
				}
				
			}
			
		}
		
	}
	
	public void SteeringWheelControlling (){
		
		if( steeringWheelIsTouching ){
			
			foreach(ControlFreak2.InputRig.Touch touch in ControlFreak2.CF2Input.touches )
			{
				if( touch.fingerId == touchId ){
					touchPos = touch.position;
					
					if( touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled ){
						steeringWheelIsTouching = false; 
					}
				}
			}
			
			float newsteerAngle = Vector2.Angle( Vector2.up, touchPos - steeringWheelWheelCenter );
			
			if( Vector2.Distance( touchPos, steeringWheelWheelCenter ) > 20f ){
				if( touchPos.x > steeringWheelWheelCenter.x )
					steeringWheelsteerAngle -= newsteerAngle - steeringWheelOldAngle;
				else
					steeringWheelsteerAngle += newsteerAngle - steeringWheelOldAngle;
			}
			
			if( steeringWheelsteerAngle > steeringWheelMaximumsteerAngle )
				steeringWheelsteerAngle = steeringWheelMaximumsteerAngle;
			else if( steeringWheelsteerAngle < -steeringWheelMaximumsteerAngle )
				steeringWheelsteerAngle = -steeringWheelMaximumsteerAngle;
			
			steeringWheelOldAngle = newsteerAngle;
			
		}else{
			
			foreach( ControlFreak2.InputRig.Touch touch in ControlFreak2.CF2Input.touches ){
				if( touch.phase == TouchPhase.Began ){
					if( steeringWheelTextureRect.Contains( new Vector2( touch.position.x, Screen.height - touch.position.y ) ) ){
						steeringWheelIsTouching = true;
						steeringWheelOldAngle = Vector2.Angle( Vector2.up, touch.position - steeringWheelWheelCenter );
						touchId = touch.fingerId;
					}
				}
			}
			
			if( !Mathf.Approximately( 0f, steeringWheelsteerAngle  ) ){
				float deltaAngle = steeringWheelResetPosspeed * Time.deltaTime;
				
				if( Mathf.Abs( deltaAngle ) > Mathf.Abs( steeringWheelsteerAngle  ) ){
					steeringWheelsteerAngle  = 0f;
					return;
				}
				
				if( steeringWheelsteerAngle > 0f )
					steeringWheelsteerAngle -= deltaAngle;
				else
					steeringWheelsteerAngle += deltaAngle;
			}
			
		}
		
	}
	
	public void KeyboardControlling (){
		
		//Motor Input.
		if(!changingGear)
			motorInput = (ControlFreak2.CF2Input.GetAxis("Vertical"));
		else
			motorInput = Mathf.Clamp(ControlFreak2.CF2Input.GetAxis("Vertical"), -1f, 0f);
		
		//Steering Input.
		if(Mathf.Abs (ControlFreak2.CF2Input.GetAxis("Horizontal")) > .05f)
			steerInput = Mathf.Lerp (steerInput, ControlFreak2.CF2Input.GetAxis("Horizontal"), Time.deltaTime * 10);
		else
			steerInput = Mathf.Lerp (steerInput, ControlFreak2.CF2Input.GetAxis("Horizontal"), Time.deltaTime * 10);
		
		//Boost Input.
		if(ControlFreak2.CF2Input.GetButton("Fire2"))
			boostInput = 1.25f;
		else
			boostInput = 1f;
		
		FrontLeftWheelCollider.steerAngle = (steerAngle * steerInput);
		FrontRightWheelCollider.steerAngle = (steerAngle * steerInput);
		
	}
	
	public void NGUIControlling (){
		
		//Motor Input.
		if(!changingGear)
			motorInput = gasPedal.input + (-brakePedal.input);
		else
			motorInput = (-brakePedal.input);
		
		//Steer Input.
		if(!useAccelerometerForSteer && !steeringWheelControl)
			steerInput = rightArrow.input + (-leftArrow.input);
		
		//Handbrake Input.
		if(handbrakeGui.input > .1f)
			mobileHandbrake = true;
		else
			mobileHandbrake = false;
		
		//Boost Input.
		if(boostGui)
			boostInput = Mathf.Clamp(boostGui.input * 2f, 1f, 1.25f);
		
	}

	public void UIControlling (){
		
		//Motor Input.
		if(!changingGear)
			motorInput = gasPedalUI.input + (-brakePedalUI.input);
		else
			motorInput = (-brakePedalUI.input);
		
		//Steer Input.
		if(!useAccelerometerForSteer && !steeringWheelControl)
			steerInput = rightArrowUI.input + (-leftArrowUI.input);
		
		//Handbrake Input.
		if(handbrakeUI.input > .1f)
			mobileHandbrake = true;
		else
			mobileHandbrake = false;
		
		//Boost Input.
		if(boostUI)
			boostInput = Mathf.Clamp(boostUI.input * 2f, 1f, 1.25f);
		
	}
	
	public void ShiftGears (){
		
		if(automaticGear){
			
			if(currentGear < _totalGears && !changingGear){
				if(speed > gearSpeed[currentGear + 1] && RearLeftWheelCollider.rpm >= 0){
					StartCoroutine("ChangingGear", currentGear + 1);
				}
			}
			
			if(currentGear > 0){
				if(EngineRPM < minEngineRPM + 500 && !changingGear){

					for(int i = 0; i < gearSpeed.Length; i++){
						if(speed > gearSpeed[i])
							StartCoroutine("ChangingGear", i);
					}

				}
			}
			
		}else{
			
			if(currentGear < _totalGears && !changingGear){
				if(ControlFreak2.CF2Input.GetButtonDown("RCCShiftUp")){
					StartCoroutine("ChangingGear", currentGear + 1);
				}
			}
			
			if(currentGear > 0){
				if(ControlFreak2.CF2Input.GetButtonDown("RCCShiftDown")){
					StartCoroutine("ChangingGear", currentGear - 1);
				}
			}
			
		}
		
	}
	
	IEnumerator ChangingGear(int gear){
		
		changingGear = true;
		
		if(gearShiftingClips.Length >= 1){

			gearShiftingSound = CreateAudioSource("gearShiftingAudio", 5f, .3f, gearShiftingClips[UnityEngine.Random.Range(0, gearShiftingClips.Length)], false, true, true);
			
//			gearShiftingAudio = new GameObject("GearShiftingSound");
//			gearShiftingAudio.transform.parent = transform;
//			gearShiftingAudio.transform.localPosition = Vector3.zero;
//			gearShiftingAudio.transform.rotation = transform.rotation;
//			
//			gearShiftingAudio.AddComponent<AudioSource>();
//			gearShiftingAudio.GetComponent<AudioSource>().minDistance = 7.5f;
//			
//			gearShiftingAudio.GetComponent<AudioSource>().clip = gearShiftingClips[UnityEngine.Random.Range(0, gearShiftingClips.Length)];
//			gearShiftingAudio.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range (.9f, 1.1f);
//			gearShiftingAudio.GetComponent<AudioSource>().Play ();
//			Destroy(gearShiftingAudio, gearShiftingAudio.GetComponent<AudioSource>().clip.length);
			
		}
		
		yield return new WaitForSeconds(.5f);
		changingGear = false;
		currentGear = gear;
		
	}
	
	public void WheelAlign (){
		
		RaycastHit hit;
		WheelHit CorrespondingGroundHit;
		
		
		//Front Left Wheel Transform.
		Vector3 ColliderCenterPointFL = FrontLeftWheelCollider.transform.TransformPoint( FrontLeftWheelCollider.center );
		FrontLeftWheelCollider.GetGroundHit( out CorrespondingGroundHit );
		
		if ( Physics.Raycast( ColliderCenterPointFL, -FrontLeftWheelCollider.transform.up, out hit, (FrontLeftWheelCollider.suspensionDistance + FrontLeftWheelCollider.radius) * transform.localScale.y) ) {
			if(hit.transform.gameObject.layer != LayerMask.NameToLayer("Vehicle")){
				FrontLeftWheelTransform.transform.position = hit.point + (FrontLeftWheelCollider.transform.up * FrontLeftWheelCollider.radius) * transform.localScale.y;
				float extension = (-FrontLeftWheelCollider.transform.InverseTransformPoint(CorrespondingGroundHit.point).y - FrontLeftWheelCollider.radius) / FrontLeftWheelCollider.suspensionDistance;
				Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point + FrontLeftWheelCollider.transform.up * (CorrespondingGroundHit.force / rigid.mass), extension <= 0.0 ? Color.magenta : Color.white);
				Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - FrontLeftWheelCollider.transform.forward * CorrespondingGroundHit.forwardSlip, Color.green);
				Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - FrontLeftWheelCollider.transform.right * CorrespondingGroundHit.sidewaysSlip, Color.red);
			}
		}else{
			FrontLeftWheelTransform.transform.position = ColliderCenterPointFL - (FrontLeftWheelCollider.transform.up * FrontLeftWheelCollider.suspensionDistance) * transform.localScale.y;
		}

		_rotationValueFL += FrontLeftWheelCollider.rpm * ( 6 ) * Time.deltaTime;
		
		FrontLeftWheelTransform.transform.rotation = FrontLeftWheelCollider.transform.rotation * Quaternion.Euler( _rotationValueFL, FrontLeftWheelCollider.steerAngle + (driftAngle / steeringAssistanceDivider), FrontLeftWheelCollider.transform.rotation.z);
		
		
		//Front Right Wheel Transform.
		Vector3 ColliderCenterPointFR = FrontRightWheelCollider.transform.TransformPoint( FrontRightWheelCollider.center );
		FrontRightWheelCollider.GetGroundHit( out CorrespondingGroundHit );
		
		if ( Physics.Raycast( ColliderCenterPointFR, -FrontRightWheelCollider.transform.up, out hit, (FrontRightWheelCollider.suspensionDistance + FrontRightWheelCollider.radius) * transform.localScale.y ) ) {
			if(hit.transform.gameObject.layer != LayerMask.NameToLayer("Vehicle")){
				FrontRightWheelTransform.transform.position = hit.point + (FrontRightWheelCollider.transform.up * FrontRightWheelCollider.radius) * transform.localScale.y;
				float extension = (-FrontRightWheelCollider.transform.InverseTransformPoint(CorrespondingGroundHit.point).y - FrontRightWheelCollider.radius) / FrontRightWheelCollider.suspensionDistance;
				Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point + FrontRightWheelCollider.transform.up * (CorrespondingGroundHit.force / rigid.mass), extension <= 0.0 ? Color.magenta : Color.white);
				Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - FrontRightWheelCollider.transform.forward * CorrespondingGroundHit.forwardSlip, Color.green);
				Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - FrontRightWheelCollider.transform.right * CorrespondingGroundHit.sidewaysSlip, Color.red);
			}
		}else{
			FrontRightWheelTransform.transform.position = ColliderCenterPointFR - (FrontRightWheelCollider.transform.up * FrontRightWheelCollider.suspensionDistance) * transform.localScale.y;
		}

		_rotationValueFR += FrontRightWheelCollider.rpm * ( 6 ) * Time.deltaTime;
		
		FrontRightWheelTransform.transform.rotation = FrontRightWheelCollider.transform.rotation * Quaternion.Euler( _rotationValueFR, FrontRightWheelCollider.steerAngle + (driftAngle / steeringAssistanceDivider), FrontRightWheelCollider.transform.rotation.z);
		
		
		//Rear Left Wheel Transform.
		Vector3 ColliderCenterPointRL = RearLeftWheelCollider.transform.TransformPoint( RearLeftWheelCollider.center );
		RearLeftWheelCollider.GetGroundHit( out CorrespondingGroundHit );
		
		if ( Physics.Raycast( ColliderCenterPointRL, -RearLeftWheelCollider.transform.up, out hit, (RearLeftWheelCollider.suspensionDistance + RearLeftWheelCollider.radius) * transform.localScale.y ) ) {
			if(hit.transform.gameObject.layer != LayerMask.NameToLayer("Vehicle")){
				RearLeftWheelTransform.transform.position = hit.point + (RearLeftWheelCollider.transform.up * RearLeftWheelCollider.radius) * transform.localScale.y;
				float extension = (-RearLeftWheelCollider.transform.InverseTransformPoint(CorrespondingGroundHit.point).y - RearLeftWheelCollider.radius) / RearLeftWheelCollider.suspensionDistance;
				Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point + RearLeftWheelCollider.transform.up * (CorrespondingGroundHit.force / rigid.mass), extension <= 0.0 ? Color.magenta : Color.white);
				Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - RearLeftWheelCollider.transform.forward * CorrespondingGroundHit.forwardSlip, Color.green);
				Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - RearLeftWheelCollider.transform.right * CorrespondingGroundHit.sidewaysSlip, Color.red);
			}
		}else{
			RearLeftWheelTransform.transform.position = ColliderCenterPointRL - (RearLeftWheelCollider.transform.up * RearLeftWheelCollider.suspensionDistance) * transform.localScale.y;
		}
		RearLeftWheelTransform.transform.rotation = RearLeftWheelCollider.transform.rotation * Quaternion.Euler( _rotationValueRL, 0, RearLeftWheelCollider.transform.rotation.z);

		_rotationValueRL += RearLeftWheelCollider.rpm * ( 6 ) * Time.deltaTime;
		
		
		//Rear Right Wheel Transform.
		Vector3 ColliderCenterPointRR = RearRightWheelCollider.transform.TransformPoint( RearRightWheelCollider.center );
		RearRightWheelCollider.GetGroundHit( out CorrespondingGroundHit );
		
		if ( Physics.Raycast( ColliderCenterPointRR, -RearRightWheelCollider.transform.up, out hit, (RearRightWheelCollider.suspensionDistance + RearRightWheelCollider.radius) * transform.localScale.y ) ) {
			if(hit.transform.gameObject.layer != LayerMask.NameToLayer("Vehicle")){
				RearRightWheelTransform.transform.position = hit.point + (RearRightWheelCollider.transform.up * RearRightWheelCollider.radius) * transform.localScale.y;
				float extension = (-RearRightWheelCollider.transform.InverseTransformPoint(CorrespondingGroundHit.point).y - RearRightWheelCollider.radius) / RearRightWheelCollider.suspensionDistance;
				Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point + RearRightWheelCollider.transform.up * (CorrespondingGroundHit.force / rigid.mass), extension <= 0.0 ? Color.magenta : Color.white);
				Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - RearRightWheelCollider.transform.forward * CorrespondingGroundHit.forwardSlip, Color.green);
				Debug.DrawLine(CorrespondingGroundHit.point, CorrespondingGroundHit.point - RearRightWheelCollider.transform.right * CorrespondingGroundHit.sidewaysSlip, Color.red);
			}
		}else{
			RearRightWheelTransform.transform.position = ColliderCenterPointRR - (RearRightWheelCollider.transform.up * RearRightWheelCollider.suspensionDistance) * transform.localScale.y;
		}
		RearRightWheelTransform.transform.rotation = RearRightWheelCollider.transform.rotation * Quaternion.Euler( _rotationValueRR, 0, RearRightWheelCollider.transform.rotation.z);

		_rotationValueRR += RearRightWheelCollider.rpm * ( 6 ) * Time.deltaTime;
		
		if(ExtraRearWheelsCollider.Length > 0){
			
			for(int i = 0; i < ExtraRearWheelsCollider.Length; i++){
				
				Vector3 ColliderCenterPointExtra = ExtraRearWheelsCollider[i].transform.TransformPoint( ExtraRearWheelsCollider[i].center );
				
				if ( Physics.Raycast( ColliderCenterPointExtra, -ExtraRearWheelsCollider[i].transform.up, out hit, (ExtraRearWheelsCollider[i].suspensionDistance + ExtraRearWheelsCollider[i].radius) * transform.localScale.y ) ) {
					ExtraRearWheelsTransform[i].transform.position = hit.point + (ExtraRearWheelsCollider[i].transform.up * ExtraRearWheelsCollider[i].radius) * transform.localScale.y;
				}else{
					ExtraRearWheelsTransform[i].transform.position = ColliderCenterPointExtra - (ExtraRearWheelsCollider[i].transform.up * ExtraRearWheelsCollider[i].suspensionDistance) * transform.localScale.y;
					//ExtraRearWheelsCollider[i].brakeTorque = brake / 15f;
				}
				ExtraRearWheelsTransform[i].transform.rotation = ExtraRearWheelsCollider[i].transform.rotation * Quaternion.Euler( RotationValueExtra[i], 0, ExtraRearWheelsCollider[i].transform.rotation.z);
				RotationValueExtra[i] += ExtraRearWheelsCollider[i].rpm * ( 6 ) * Time.deltaTime;
				
			}
			
		}
		
		//Drift Angle Calculation.
		WheelHit CorrespondingGroundHit5;
		RearRightWheelCollider.GetGroundHit(out CorrespondingGroundHit5);
		driftAngle = Mathf.Lerp ( driftAngle, (Mathf.Clamp (CorrespondingGroundHit5.sidewaysSlip, -35, 35)), Time.deltaTime * 2 );
		
		//Driver SteeringWheel Transform.
		if(SteeringWheel)
			SteeringWheel.transform.rotation = transform.rotation * Quaternion.Euler( 0, 0, (FrontLeftWheelCollider.steerAngle + (driftAngle / steeringAssistanceDivider)) * -6);
		
	}
	
	public void WheelCamber (){
		
		WheelHit CorrespondingGroundHit;
		
		FrontLeftWheelCollider.GetGroundHit(out CorrespondingGroundHit); 
		float FLHandling = Mathf.Lerp (-1, 1, CorrespondingGroundHit.force / 8000f);
		FrontRightWheelCollider.GetGroundHit(out CorrespondingGroundHit); 
		float FRHandling = Mathf.Lerp (-1, 1, CorrespondingGroundHit.force / 8000f);
		RearLeftWheelCollider.GetGroundHit(out CorrespondingGroundHit); 
		float RLHandling = Mathf.Lerp (-1, 1, CorrespondingGroundHit.force / 8000f);
		RearRightWheelCollider.GetGroundHit(out CorrespondingGroundHit); 
		float RRHandling = Mathf.Lerp (-1, 1, CorrespondingGroundHit.force / 8000f);
		
		FrontLeftWheelCollider.transform.localEulerAngles = new Vector3(FrontLeftWheelCollider.transform.localEulerAngles.x, FrontLeftWheelCollider.transform.localEulerAngles.y, (-FLHandling));
		FrontRightWheelCollider.transform.localEulerAngles = new Vector3(FrontRightWheelCollider.transform.localEulerAngles.x, FrontRightWheelCollider.transform.localEulerAngles.y, (FRHandling));
		RearLeftWheelCollider.transform.localEulerAngles = new Vector3(RearLeftWheelCollider.transform.localEulerAngles.x, RearLeftWheelCollider.transform.localEulerAngles.y, (-RLHandling));
		RearRightWheelCollider.transform.localEulerAngles = new Vector3(RearRightWheelCollider.transform.localEulerAngles.x, RearRightWheelCollider.transform.localEulerAngles.y, (RRHandling));
		
	}
	
	public void DashboardGUI (){

		if(_dashboardType == DashboardType.NGUIDashboard){
		
			if(!NGUIInputs){
				Debug.LogError("If you gonna use NGUI Dashboard, your NGUI Root (Dashboard GameObject) must have ''RCCNGUIDashboardInputs.cs''. First be sure your Dashboard gameobject has ''RCCNGUIDashboardInputs.cs'', then select your ''NGUI Inputs'' in editor script.");
				return;
			}
			
			NGUIInputs.RPM = EngineRPM;
			NGUIInputs.KMH = speed;
			NGUIInputs.Gear = FrontLeftWheelCollider.rpm > -10 ? currentGear : -1f;
			
			RPMNeedleRotation = Mathf.Lerp (minimumRPMNeedleAngle, maximumRPMNeedleAngle, (EngineRPM - minEngineRPM / 1.5f) / (maxEngineRPM + minEngineRPM));
			KMHNeedleRotation = Mathf.Lerp (minimumKMHNeedleAngle, maximumKMHNeedleAngle, speed / maxspeed);
			smoothedNeedleRotation = Mathf.Lerp (smoothedNeedleRotation, RPMNeedleRotation, Time.deltaTime * 5);
			
			NGUIRPMNeedle.transform.eulerAngles = new Vector3(NGUIRPMNeedle.transform.eulerAngles.x ,NGUIRPMNeedle.transform.eulerAngles.y, -smoothedNeedleRotation);
			NGUIKMHNeedle.transform.eulerAngles = new Vector3(NGUIKMHNeedle.transform.eulerAngles.x ,NGUIKMHNeedle.transform.eulerAngles.y, -KMHNeedleRotation);

		}

		if(_dashboardType == DashboardType.UnityDashboard){
			
			if(!UIInputs){
				Debug.LogError("If you gonna use UI Dashboard, your Canvas Root must have ''RCCUIDashboardInputs.cs''. First be sure your Canvas Root has ''RCCUIDashboardInputs.cs'', then select your ''UI Inputs'' in script.");
				return;
			}
			
			UIInputs.RPM = EngineRPM;
			UIInputs.KMH = speed;
			UIInputs.Gear = FrontLeftWheelCollider.rpm > -10 ? currentGear : -1f;
			
			RPMNeedleRotation = Mathf.Lerp (minimumRPMNeedleAngle, maximumRPMNeedleAngle, (EngineRPM - minEngineRPM / 1.5f) / (maxEngineRPM + minEngineRPM));
			KMHNeedleRotation = Mathf.Lerp (minimumKMHNeedleAngle, maximumKMHNeedleAngle, speed / maxspeed);
			smoothedNeedleRotation = Mathf.Lerp (smoothedNeedleRotation, RPMNeedleRotation, Time.deltaTime * 5);
			
			RPMNeedle.transform.eulerAngles = new Vector3(RPMNeedle.transform.eulerAngles.x ,RPMNeedle.transform.eulerAngles.y, -smoothedNeedleRotation);
			KMHNeedle.transform.eulerAngles = new Vector3(KMHNeedle.transform.eulerAngles.x ,KMHNeedle.transform.eulerAngles.y, -KMHNeedleRotation);
			
		}
		
	}
	
	public void SmokeInstantiateRate () {
		
		WheelHit CorrespondingGroundHit0;
		WheelHit CorrespondingGroundHit1;
		WheelHit CorrespondingGroundHit2;
		WheelHit CorrespondingGroundHit3;
		
		if ( _wheelParticles.Count > 0 ) {
				
			FrontRightWheelCollider.GetGroundHit( out CorrespondingGroundHit0 );
			if(Mathf.Abs(CorrespondingGroundHit0.sidewaysSlip) > .25f || Mathf.Abs(CorrespondingGroundHit0.forwardSlip) > .5f ){
				_wheelParticles[0].GetComponent<ParticleSystem>().Play();
			}else{ 
				_wheelParticles[0].GetComponent<ParticleSystem>().Stop();
			}
			
			FrontLeftWheelCollider.GetGroundHit( out CorrespondingGroundHit1 );
			if(Mathf.Abs(CorrespondingGroundHit1.sidewaysSlip) > .25f || Mathf.Abs(CorrespondingGroundHit1.forwardSlip) > .5f ){
				_wheelParticles[1].GetComponent<ParticleSystem>().Play();
			}else{ 
				_wheelParticles[1].GetComponent<ParticleSystem>().Stop();
			}
			
			RearRightWheelCollider.GetGroundHit( out CorrespondingGroundHit2 );
			if(Mathf.Abs(CorrespondingGroundHit2.sidewaysSlip) > .25f || Mathf.Abs(CorrespondingGroundHit2.forwardSlip) > .5f ){
				_wheelParticles[2].GetComponent<ParticleSystem>().Play();
			}else{
				_wheelParticles[2].GetComponent<ParticleSystem>().Stop();
			}
			
			RearLeftWheelCollider.GetGroundHit( out CorrespondingGroundHit3 );
			if(Mathf.Abs(CorrespondingGroundHit3.sidewaysSlip) > .25f || Mathf.Abs(CorrespondingGroundHit3.forwardSlip) > .5f ){
				_wheelParticles[3].GetComponent<ParticleSystem>().Play();
			}else{ 
				_wheelParticles[3].GetComponent<ParticleSystem>().Stop();
			}

		}
		
		if(normalExhaustGas && canControl){
			if(speed < 20)
				normalExhaustGas.Play();
			else normalExhaustGas.Stop();
		}
		
		if(heavyExhaustGas && canControl){
			if(speed < 10 && motorInput > .1f)
				heavyExhaustGas.Play();
			else heavyExhaustGas.Stop();
		}
		
		if(!canControl){
			if(heavyExhaustGas)
				heavyExhaustGas.Stop();
			if(normalExhaustGas)
				normalExhaustGas.Stop();
		}
		
	}
	
	public void SkidAudio (){

		if(!skidSound)
			return;
		
		WheelHit CorrespondingGroundHitF;
		FrontRightWheelCollider.GetGroundHit( out CorrespondingGroundHitF );
		
		WheelHit CorrespondingGroundHitR;
		RearRightWheelCollider.GetGroundHit( out CorrespondingGroundHitR );
		
		if(Mathf.Abs(CorrespondingGroundHitF.sidewaysSlip) > .25f || Mathf.Abs(CorrespondingGroundHitR.forwardSlip) > .5f || Mathf.Abs(CorrespondingGroundHitF.forwardSlip) > .5f){
			if(rigid.velocity.magnitude > 1f)
				skidSound.volume = Mathf.Abs(CorrespondingGroundHitF.sidewaysSlip) + ((Mathf.Abs(CorrespondingGroundHitF.forwardSlip) + Mathf.Abs(CorrespondingGroundHitR.forwardSlip)) / 4f);
			else
				skidSound.volume -= Time.deltaTime;
		}else{
			skidSound.volume -= Time.deltaTime;
		}

	}
	
	public void ResetCar (){
		
		if(speed < 5 && !rigid.isKinematic){
			
			if( transform.eulerAngles.z < 300 && transform.eulerAngles.z > 60){
				resetTime += Time.deltaTime;
				if(resetTime > 4){
					transform.rotation = Quaternion.identity;
					transform.position = new Vector3( transform.position.x, transform.position.y + 3, transform.position.z );
					resetTime = 0f;
				}
			}
			
			if( transform.eulerAngles.x < 300 && transform.eulerAngles.x > 60){
				resetTime += Time.deltaTime;
				if(resetTime > 4){
					transform.rotation = Quaternion.identity;
					transform.position = new Vector3( transform.position.x, transform.position.y + 3, transform.position.z );
					resetTime = 0f;
				}
			}
			
		}
		
	}


	private void OnCollisionEnter(Collision collision)
	{
		if (collision.contacts.Length <= 0)
			return;

		if (collision.relativeVelocity.magnitude > 4f && collision.contacts[0].thisCollider.gameObject.transform != base.transform.parent && collision.gameObject.name.Contains("Player(Clone)")){

			float magnitude = base.transform.GetComponent<Rigidbody>().velocity.magnitude;
			float magnitude2 = collision.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

			if (magnitude > magnitude2 && magnitude > 5f)
			{
				PhotonMan component = collision.gameObject.GetComponent<PhotonMan>();

				if (component != null) {
					Debug.Log(collision.relativeVelocity.magnitude + "  " + magnitude + "  " + magnitude2);
					component.HitPlayer((short)300, (byte)0, WorldController.I.MyId);
					crashSound = CreateAudioSource("crashSound", 5f, 0.4f, crashClips [UnityEngine.Random.Range (0, crashClips.Length - 4)], false, true, true);
				}
			}
		}

		if (collision.relativeVelocity.magnitude > 1f && collision.contacts[0].thisCollider.gameObject.transform != base.transform.parent){
			PhotonMob component2 = collision.gameObject.GetComponent<PhotonMob>();

			if ((bool)component2) {
				component2.Hit(1000, -component2.transform.forward, component2.transform.position, false, null);
				crashSound = CreateAudioSource("crashSound", 5f, 0.4f, crashClips [UnityEngine.Random.Range (0, crashClips.Length - 4)], false, true, true);
			}
		}

		if (collision.relativeVelocity.magnitude > (float)collisionForceLimit && crashClips.Length > 0 && collision.contacts [0].thisCollider.gameObject.transform != base.transform.parent && !collision.gameObject.name.Contains ("Player")) {
			crashSound = CreateAudioSource ("crashSound", 5f, 0.2f, crashClips [UnityEngine.Random.Range (0, crashClips.Length - 4)], false, true, true);
		}
	}
	
	public void Chassis (){
		
		verticalLean = Mathf.Clamp(Mathf.Lerp (verticalLean, rigid.angularVelocity.x * chassisVerticalLean, Time.deltaTime * 3f), -3.0f, 3.0f);

		WheelHit CorrespondingGroundHit;
		RearRightWheelCollider.GetGroundHit(out CorrespondingGroundHit);

		float normalizedLeanAngle = Mathf.Clamp(CorrespondingGroundHit.sidewaysSlip, -1f, 1f);
		
		if(normalizedLeanAngle > 0f)
			normalizedLeanAngle = 1;
		else
			normalizedLeanAngle = -1;

		horizontalLean = Mathf.Clamp(Mathf.Lerp (horizontalLean, (Mathf.Abs (transform.InverseTransformDirection(rigid.angularVelocity).y) * -normalizedLeanAngle) * chassisHorizontalLean, Time.deltaTime * 3f), -3.0f, 3.0f);
		
		Quaternion target = Quaternion.Euler(verticalLean, chassis.transform.localRotation.y + (rigid.angularVelocity.z), horizontalLean);
		chassis.transform.localRotation = target;
		
		rigid.centerOfMass = new Vector3((COM.localPosition.x) * transform.localScale.x , (COM.localPosition.y) * transform.localScale.y , (COM.localPosition.z) * transform.localScale.z);
		
	}
	
	public void Lights (){
		
		float brakeLightInput;
		brakeLightInput = Mathf.Clamp(-motorInput * 2, 0.0f, 1.0f);
		
		if(ControlFreak2.CF2Input.GetKeyDown(KeyCode.L)){
			if(headLightsOn)
				headLightsOn = false;
			else headLightsOn = true;
		}
		
		for(int i = 0; i < brakeLights.Length; i++){
			
			if(!reversing)
				brakeLights[i].intensity = brakeLightInput;
			else
				brakeLights[i].intensity = 0f;
			
		}
		
		for(int i = 0; i < headLights.Length; i++){
			
			if(headLightsOn)
				headLights[i].enabled = true;
			else
				headLights[i].enabled = false;
			
		}
		
		for(int i = 0; i < reverseLights.Length; i++){
			
			if(!reversing)
				reverseLights[i].intensity = Mathf.Lerp (reverseLights[i].intensity, 0.0f, Time.deltaTime*5);
			else
				reverseLights[i].intensity = brakeLightInput;
			
		}
		
	}
	
	void OnGUI (){

		GUI.skin.label.fontSize = 12;
		GUI.skin.box.fontSize = 12;
		Matrix4x4 orgRotation = GUI.matrix;
		
		if(canControl){
			
			if(useAccelerometerForSteer && mobileController){
				if(_mobileControllerType == MobileGUIType.NGUIController){
					leftArrow.gameObject.SetActive(false);
					rightArrow.gameObject.SetActive(false);
					handbrakeGui.gameObject.SetActive(true);
					brakePedal.transform.position = leftArrow.transform.position;
				}
				if(_mobileControllerType == MobileGUIType.UIController){
					leftArrowUI.gameObject.SetActive(false);
					rightArrowUI.gameObject.SetActive(false);
					handbrakeUI.gameObject.SetActive(true);
					brakePedalUI.transform.position = leftArrowUI.transform.position;
				}
				steeringWheelControl = false;
			}else if(mobileController){
				if(_mobileControllerType == MobileGUIType.NGUIController){
					gasPedal.gameObject.SetActive(true);
					leftArrow.gameObject.SetActive(true);
					rightArrow.gameObject.SetActive(true);
					handbrakeGui.gameObject.SetActive(true);
					brakePedal.transform.position = defbrakePedalPosition;
				}
				if(_mobileControllerType == MobileGUIType.UIController){
					gasPedalUI.gameObject.SetActive(true);
					leftArrowUI.gameObject.SetActive(true);
					rightArrowUI.gameObject.SetActive(true);
					handbrakeUI.gameObject.SetActive(true);
					brakePedalUI.gameObject.SetActive(true);
					brakePedalUI.transform.position = defbrakePedalPosition;
				}
			}
			
			if(steeringWheelControl && mobileController){

				if(_mobileControllerType == MobileGUIType.NGUIController){
					leftArrow.gameObject.SetActive(false);
					rightArrow.gameObject.SetActive(false);
				}
				if(_mobileControllerType == MobileGUIType.UIController){
					leftArrowUI.gameObject.SetActive(false);
					rightArrowUI.gameObject.SetActive(false);
				}

				GUIUtility.RotateAroundPivot( -steeringWheelsteerAngle , steeringWheelTextureRect.center + steeringWheelPivotPos );
				GUI.DrawTexture( steeringWheelTextureRect, steeringWheelTexture );
				GUI.matrix = orgRotation;

			}
			
			if( demoGUI ) {

				GUI.backgroundColor = Color.black;
				float guiWidth = Screen.width/2 - 200;
				
				GUI.Box(new Rect(Screen.width-410 - guiWidth, 10, 400, 220), "");
				GUI.Label(new Rect(Screen.width-400 - guiWidth, 10, 400, 150), "Engine RPM : " + Mathf.CeilToInt(EngineRPM));
				GUI.Label(new Rect(Screen.width-400 - guiWidth, 30, 400, 150), "speed : " + Mathf.CeilToInt(speed));
				GUI.Label(new Rect(Screen.width-400 - guiWidth, 190, 400, 150), "Horizontal Tilt : " + Input.acceleration.x);
				GUI.Label(new Rect(Screen.width-400 - guiWidth, 210, 400, 150), "Vertical Tilt : " + Input.acceleration.y);
				if(fwd){
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 50, 400, 150), "Left Wheel RPM : " + Mathf.CeilToInt(FrontLeftWheelCollider.rpm));
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 70, 400, 150), "Right Wheel RPM : " + Mathf.CeilToInt(FrontRightWheelCollider.rpm));
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 90, 400, 150), "Left Wheel Torque : " + Mathf.CeilToInt(FrontLeftWheelCollider.motorTorque));
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 110, 400, 150), "Right Wheel Torque : " + Mathf.CeilToInt(FrontRightWheelCollider.motorTorque));
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 130, 400, 150), "Left Wheel brake : " + Mathf.CeilToInt(FrontLeftWheelCollider.brakeTorque));
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 150, 400, 150), "Right Wheel brake : " + Mathf.CeilToInt(FrontRightWheelCollider.brakeTorque));
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 170, 400, 150), "Steer Angle : " + Mathf.CeilToInt(FrontLeftWheelCollider.steerAngle));
				}
				if(rwd){
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 50, 400, 150), "Left Wheel RPM : " + Mathf.CeilToInt(RearLeftWheelCollider.rpm));
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 70, 400, 150), "Right Wheel RPM : " + Mathf.CeilToInt(RearRightWheelCollider.rpm));
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 90, 400, 150), "Left Wheel Torque : " + Mathf.CeilToInt(RearLeftWheelCollider.motorTorque));
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 110, 400, 150), "Right Wheel Torque : " + Mathf.CeilToInt(RearRightWheelCollider.motorTorque));
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 130, 400, 150), "Left Wheel brake : " + Mathf.CeilToInt(RearLeftWheelCollider.brakeTorque));
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 150, 400, 150), "Right Wheel brake : " + Mathf.CeilToInt(RearRightWheelCollider.brakeTorque));
					GUI.Label(new Rect(Screen.width-400 - guiWidth, 170, 400, 150), "Steer Angle : " + Mathf.CeilToInt(FrontLeftWheelCollider.steerAngle));
				}
				
				GUI.backgroundColor = Color.blue;
				GUI.Button (new Rect(Screen.width-30 - guiWidth, 165, 10, Mathf.Clamp((-motorInput * 100), -100, 0)), "");

				if(mobileController){

					if(GUI.Button(new Rect(Screen.width - 275, 200, 250, 50), "Use Accelerometer \n For Steer")){
						if(useAccelerometerForSteer)
							useAccelerometerForSteer = false;
						else useAccelerometerForSteer = true;
					}
					
					if(GUI.Button(new Rect(Screen.width - 275, 275, 250, 50), "Use Steering Wheel")){
						if(steeringWheelControl)
							steeringWheelControl = false;
						else steeringWheelControl = true;
					}

				}
				
				GUI.backgroundColor = Color.red;
				GUI.Button (new Rect(Screen.width-45 - guiWidth, 165, 10, Mathf.Clamp((motorInput * 100), -100, 0)), "");
				
			}
			
		}
		
	}
	
}