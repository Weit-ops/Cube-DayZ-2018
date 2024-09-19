using UnityEngine;
using System.Collections;
using UnityEditor;
using CurveExtended;


[CustomEditor(typeof(RCCCarControllerV2)), CanEditMultipleObjects]
public class RCCEditor : Editor {
	
	bool nguiDashboard = false;
	bool nguiMobileController = false;
	RCCCarControllerV2 carScript;
	
	Texture2D wheelIcon;
	Texture2D steerIcon;
	Texture2D configIcon;
	Texture2D lightIcon;
	Texture2D mobileIcon;
	Texture2D soundIcon;
	Texture2D gaugeIcon;
	Texture2D smokeIcon;
	
	bool WheelSettings;
	bool SteerSettings;
	bool Configurations;
	bool LightSettings;
	bool SoundSettings;
	bool MobileSettings;
	bool DashBoardSettings;
	bool SmokeSettings;
	
	void Awake(){
		
		carScript = (RCCCarControllerV2)target;
		
		wheelIcon = Resources.Load("WheelIcon", typeof(Texture2D)) as Texture2D;
		steerIcon = Resources.Load("SteerIcon", typeof(Texture2D)) as Texture2D;
		configIcon = Resources.Load("ConfigIcon", typeof(Texture2D)) as Texture2D;
		lightIcon = Resources.Load("LightIcon", typeof(Texture2D)) as Texture2D;
		mobileIcon = Resources.Load("MobileIcon", typeof(Texture2D)) as Texture2D;
		soundIcon = Resources.Load("SoundIcon", typeof(Texture2D)) as Texture2D;
		gaugeIcon = Resources.Load("GaugeIcon", typeof(Texture2D)) as Texture2D;
		smokeIcon = Resources.Load("SmokeIcon", typeof(Texture2D)) as Texture2D;
		
	}
	
	public override void OnInspectorGUI () {
		
		serializedObject.Update();
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		EditorGUILayout.BeginHorizontal();
		
		if(WheelSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(wheelIcon))
			if(!WheelSettings)	WheelSettings = true;
		else WheelSettings = false;
		
		if(SteerSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(steerIcon))
			if(!SteerSettings)	SteerSettings = true;
		else SteerSettings = false;
		
		if(Configurations)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(configIcon))
			if(!Configurations)	Configurations = true;
		else Configurations = false;
		
		if(LightSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(lightIcon))
			if(!LightSettings)	LightSettings = true;
		else LightSettings = false;
		
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		
		if(SoundSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(soundIcon))
			if(!SoundSettings)	SoundSettings = true;
		else SoundSettings = false;
		
		if(MobileSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(mobileIcon))
			if(!MobileSettings)	MobileSettings = true;
		else MobileSettings = false;
		
		if(DashBoardSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(gaugeIcon))
			if(!DashBoardSettings)	DashBoardSettings = true;
		else DashBoardSettings = false;
		
		if(SmokeSettings)
			GUI.backgroundColor = Color.gray;
		else GUI.backgroundColor = Color.white;
		
		if(GUILayout.Button(smokeIcon))
			if(!SmokeSettings)	SmokeSettings = true;
		else SmokeSettings = false;
		
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		
		EditorGUILayout.EndHorizontal();
		
		GUI.backgroundColor = Color.gray;
		
		if(MobileSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Mobile Settings");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("mobileController"),false);
			
			if(carScript.mobileController){
				EditorGUILayout.PropertyField(serializedObject.FindProperty("_mobileControllerType"));
				EditorGUILayout.PropertyField(serializedObject.FindProperty("useAccelerometerForSteer"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelControl"),false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("gyroTiltMultiplier"),false);
				
				if(carScript._mobileControllerType == RCCCarControllerV2.MobileGUIType.NGUIController)
					nguiMobileController = true;
				else
					nguiMobileController = false;
				
				if(nguiMobileController){
					
					EditorGUILayout.Space();
					GUI.color = Color.cyan;
					EditorGUILayout.LabelField("NGUI Elements");
					GUI.color = Color.white;
					EditorGUILayout.Space();
					
					EditorGUILayout.PropertyField(serializedObject.FindProperty("gasPedal"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("brakePedal"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("leftArrow"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("rightArrow"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("handbrakeGui"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("boostGui"),false);
					EditorGUILayout.Space();
					
				}else{
					
					EditorGUILayout.Space();
					GUI.color = Color.cyan;
					EditorGUILayout.LabelField("Unity GUI Elements");
					GUI.color = Color.white;
					EditorGUILayout.Space();
					
					EditorGUILayout.PropertyField(serializedObject.FindProperty("gasPedalUI"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("brakePedalUI"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("leftArrowUI"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("rightArrowUI"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("handbrakeUI"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("boostUI"),false);
					EditorGUILayout.Space();
					
				}
				
				if(carScript.steeringWheelControl){
					
					EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelMaximumsteerAngle"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelXOffset"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelYOffset"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelGuiScale"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelPivotPos"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelResetPosspeed"),false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringWheelTexture"),false);
					
				}
				
			}
			
		}
		
		if(WheelSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Wheel Settings");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Create Necessary Gameobject Groups")){
				
				carScript.GetComponent<Rigidbody>().mass = 1500f;
				carScript.GetComponent<Rigidbody>().angularDrag = .5f;
				carScript.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
				
				carScript.transform.gameObject.layer = LayerMask.NameToLayer("Vehicle");
				
				Transform[] objects = carScript.gameObject.GetComponentsInChildren<Transform>();
				bool didWeHaveThisObject = false;
				
				foreach(Transform g in objects){
					if(g.name == "Chassis")
						didWeHaveThisObject = true;
				}
				
				if(!didWeHaveThisObject){
					
					GameObject chassis = new GameObject("Chassis");
					chassis.transform.parent = carScript.transform;
					chassis.transform.localPosition = Vector3.zero;
					chassis.transform.localScale = Vector3.one;
					chassis.transform.rotation = carScript.transform.rotation;
					carScript.chassis = chassis;
					GameObject wheelTransforms = new GameObject("Wheel Transforms");
					wheelTransforms.transform.parent = carScript.transform;
					wheelTransforms.transform.localPosition = Vector3.zero;
					wheelTransforms.transform.localScale = Vector3.one;
					wheelTransforms.transform.rotation = carScript.transform.rotation;
					GameObject COM = new GameObject("COM");
					COM.transform.parent = carScript.transform;
					COM.transform.localPosition = Vector3.zero;
					COM.transform.localScale = Vector3.one;
					COM.transform.rotation = carScript.transform.rotation;
					carScript.COM = COM.transform;
					
				}else{
					
					Debug.LogError("Your Vehicle has these groups already!");
					
				}
				
			}
			
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("FrontLeftWheelTransform"),false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("FrontRightWheelTransform"),false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("RearLeftWheelTransform"),false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("RearRightWheelTransform"),false);
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Create Wheel Colliders")){
				
				WheelCollider[] wheelColliders = carScript.gameObject.GetComponentsInChildren<WheelCollider>();
				
				if(wheelColliders.Length >= 1)
					Debug.LogError("Your Vehicle has Wheel Colliders already!");
				else
					carScript.CreateWheelColliders();
				
			}
			
			if(carScript.FrontLeftWheelTransform == null || carScript.FrontRightWheelTransform == null || carScript.RearLeftWheelTransform == null || carScript.RearRightWheelTransform == null  )
				EditorGUILayout.HelpBox("Select all of your Wheel Transforms before creating Wheel Colliders", MessageType.Warning);
			
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("FrontLeftWheelCollider"),false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("FrontRightWheelCollider"),false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("RearLeftWheelCollider"),false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("RearRightWheelCollider"),false);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ExtraRearWheelsCollider"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("ExtraRearWheelsTransform"), true);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("SteeringWheel"), false);
			
		}
		
		if(SteerSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Steer Settings");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("steerAngle"),false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("highspeedsteerAngle"),false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("highspeedsteerAngleAtspeed"),false);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("antiRoll"));
			EditorGUILayout.Space();
			
		}
		
		if(Configurations){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Configurations");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("canControl"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("driftMode"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("autoReverse"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("useDifferantial"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("automaticGear"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("_wheelTypeChoise"));
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("COM"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("totalGears"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("currentGear"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("gearShiftRate"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("engineTorque"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxspeed"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("maxEngineRPM"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("minEngineRPM"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("brake"), false);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("chassis"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("chassisVerticalLean"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("chassisHorizontalLean"), false);
			
		}
		
		if(SoundSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Sound Settings");
			GUI.color = Color.white;
			
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("engineClip"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("engineStartClip"), false); 
			EditorGUILayout.PropertyField(serializedObject.FindProperty("skidClip"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("crashClips"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("gearShiftingClips"), true);
			EditorGUILayout.Space();
			
			
		}
		
		if(DashBoardSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("DashBoard Settings");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("demoGUI"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("dashBoard"), false);
			
			if(carScript.dashBoard){
				
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("_dashboardType"), false);
				EditorGUILayout.Space();
				
				if(carScript._dashboardType == RCCCarControllerV2.DashboardType.NGUIDashboard)
					nguiDashboard = true;
				else
					nguiDashboard = false;
				
				if(nguiDashboard){

					EditorGUILayout.PropertyField(serializedObject.FindProperty("NGUIInputs"), false);
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("NGUIRPMNeedle"), false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("NGUIKMHNeedle"), false);
					EditorGUILayout.Space();

				}else{

					EditorGUILayout.PropertyField(serializedObject.FindProperty("UIInputs"), false);
					EditorGUILayout.Space();
					EditorGUILayout.PropertyField(serializedObject.FindProperty("RPMNeedle"), false);
					EditorGUILayout.PropertyField(serializedObject.FindProperty("KMHNeedle"), false);
					EditorGUILayout.Space();

				}

				EditorGUILayout.PropertyField(serializedObject.FindProperty("minimumRPMNeedleAngle"), false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumRPMNeedleAngle"), false);
				EditorGUILayout.Space();
				EditorGUILayout.PropertyField(serializedObject.FindProperty("minimumKMHNeedleAngle"), false);
				EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumKMHNeedleAngle"), false);
				
			}
			
		}
		
		if(SmokeSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Smoke Settings");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("wheelSlipPrefab"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("normalExhaustGas"), false);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("heavyExhaustGas"), false);
			
		}
		
		if(LightSettings){
			
			EditorGUILayout.Space();
			GUI.color = Color.cyan;
			EditorGUILayout.LabelField("Light Settings");
			GUI.color = Color.white;
			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("headLights"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeLights"), true);
			EditorGUILayout.PropertyField(serializedObject.FindProperty("reverseLights"), true);
			
		}
		
		EditorGUILayout.Space();
		
		EditorGUILayout.Space();
		GUI.color = Color.cyan;
		EditorGUILayout.LabelField("System Overall Check");
		GUI.color = Color.white;
		EditorGUILayout.Space();
		
		EditorGUILayout.BeginHorizontal();
		
		if(carScript.FrontLeftWheelCollider == null || carScript.FrontRightWheelCollider == null || carScript.RearLeftWheelCollider == null || carScript.RearRightWheelCollider == null)
			EditorGUILayout.HelpBox("Wheel Colliders = NOT OK", MessageType.Error);
		else
			EditorGUILayout.HelpBox("Wheel Colliders = OK", MessageType.None);
		
		if(carScript.FrontLeftWheelTransform == null || carScript.FrontRightWheelTransform == null || carScript.RearLeftWheelTransform == null || carScript.RearRightWheelTransform == null)
			EditorGUILayout.HelpBox("Wheel Transforms = NOT OK", MessageType.Error);
		else
			EditorGUILayout.HelpBox("Wheel Transforms = OK", MessageType.None);
		
		if(carScript.COM){
			
			if(carScript.COM == null)
				EditorGUILayout.HelpBox("COM = NOT OK", MessageType.Error);
			else
				EditorGUILayout.HelpBox("COM = OK", MessageType.None);
			
		}

		Collider[] cols = carScript.gameObject.GetComponentsInChildren<Collider>();

		if(cols.Length <= 4)
			EditorGUILayout.HelpBox("Your vehicle MUST have Box Collider, or Mesh Collider.", MessageType.Error);
		else
			EditorGUILayout.HelpBox("Colliders = OK", MessageType.None);
		
		EditorGUILayout.EndHorizontal();
		
		if(carScript.COM){
			
			if(Mathf.Approximately(carScript.COM.transform.localPosition.y, 0f))
				EditorGUILayout.HelpBox("You haven't changed COM position of the vehicle yet. Keep in that your mind, COM is most extremely important for realistic behavior.", MessageType.Warning);
			else
				EditorGUILayout.HelpBox("COM position = OK", MessageType.None);
			
		}else{
			
			EditorGUILayout.HelpBox("You haven't created COM of the vehicle yet. Just hit ''Create Necessary Gameobject Groups'' under ''Wheel'' tab for creating this too.", MessageType.Error);
			
		}
		
		if(carScript.mobileController){
			if(carScript._mobileControllerType == RCCCarControllerV2.MobileGUIType.NGUIController){
				if(carScript.gasPedal == null || carScript.brakePedal == null || carScript.handbrakeGui == null || carScript.leftArrow == null || carScript.rightArrow == null)
					EditorGUILayout.HelpBox("Select all of your NGUI Controller Elements in script.", MessageType.Error);
				else
					EditorGUILayout.HelpBox("NGUI Elements = OK", MessageType.None);
			}
			if(carScript._mobileControllerType == RCCCarControllerV2.MobileGUIType.UIController){
				if(carScript.gasPedalUI == null || carScript.brakePedalUI == null || carScript.handbrakeUI == null || carScript.leftArrowUI == null || carScript.rightArrowUI == null)
					EditorGUILayout.HelpBox("Select all of your GUI Controller Elements in script.", MessageType.Error);
				else
					EditorGUILayout.HelpBox("GUI Elements = OK", MessageType.None);
			}
		}
		
		EditorGUILayout.Space();
		
		if(GUI.changed){
			EngineCurveInit();
			EditorUtility.SetDirty(carScript);
		}
		
		serializedObject.ApplyModifiedProperties();
		
	}
	
	void EngineCurveInit (){
		
		if(carScript.totalGears <= 0){
			Debug.LogError("You are trying to set your vehicle gear to 0 or below! Why you trying to do this???");
			return;
		}
		
		carScript.gearSpeed = new float[carScript.totalGears];
		carScript.engineTorqueCurve = new AnimationCurve[carScript.totalGears];
		carScript.currentGear = 0;
		
		for(int i = 0; i < carScript.engineTorqueCurve.Length; i ++){
			carScript.engineTorqueCurve[i] = new AnimationCurve(new Keyframe(0, 1));
		}
		
		for(int i = 0; i < carScript.totalGears; i ++){
			
			carScript.gearSpeed[i] = Mathf.Lerp(0, carScript.maxspeed, ((float)i/(float)(carScript.totalGears - 0)));
			
			if(i != 0){
				carScript.engineTorqueCurve[i].MoveKey(0, new Keyframe(0, .25f));
				carScript.engineTorqueCurve[i].AddKey(KeyframeUtil.GetNew(Mathf.Lerp(0, carScript.maxspeed, ((float)i/(float)(carScript.totalGears - 0))), 1f, CurveExtended.TangentMode.Smooth));
				carScript.engineTorqueCurve[i].AddKey(carScript.maxspeed, 0);
				carScript.engineTorqueCurve[i].postWrapMode = WrapMode.Clamp;
				carScript.engineTorqueCurve[i].UpdateAllLinearTangents();
			}else{
				carScript.engineTorqueCurve[i].AddKey(KeyframeUtil.GetNew(Mathf.Lerp(25, carScript.maxspeed, ((float)i/(float)(carScript.totalGears - 1))), 1.25f, TangentMode.Linear));
				carScript.engineTorqueCurve[i].AddKey(KeyframeUtil.GetNew(Mathf.Lerp(25f, carScript.maxspeed, ((float)(i+1)/(float)(carScript.totalGears - 1))), 0, TangentMode.Linear));
				carScript.engineTorqueCurve[i].AddKey(carScript.maxspeed, 0);
				carScript.engineTorqueCurve[i].UpdateAllLinearTangents();
			}
			
		}
		
	}
	
}
