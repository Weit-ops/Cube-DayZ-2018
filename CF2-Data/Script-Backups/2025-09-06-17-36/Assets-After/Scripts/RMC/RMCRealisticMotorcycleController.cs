using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RMCRealisticMotorcycleController : MonoBehaviour
{
	private Rigidbody rigid;

	public WheelCollider FrontWheelCollider;

	public WheelCollider RearWheelCollider;

	public Transform FrontWheelTransform;

	public Transform RearWheelTransform;

	public Transform Fender;

	public Transform SteeringHandlebar;

	public Transform COM;

	public bool changingGear;

	public float gearShiftRate = 10f;

	[HideInInspector]
	public float[] gearSpeed;

	public int currentGear;

	public int totalGears = 6;

	public GameObject chassis;

	public float chassisVerticalLean = 4f;

	public float chassisHorizontalLean = 4f;

	private float horizontalLean;

	private float verticalLean;

	[HideInInspector]
	public AnimationCurve[] engineTorqueCurve;

	public float EngineTorque = 1500f;

	public float MaxEngineRPM = 6000f;

	public float MinEngineRPM = 1000f;

	public float SteerAngle = 40f;

	[HideInInspector]
	public float Speed;

	public float highSpeedSteerAngle = 5f;

	public float highSpeedSteerAngleAtSpeed = 80f;

	public float maxSpeed = 180f;

	public float Brake = 2500f;

	private float EngineRPM;

	private float motorInput;

	private float defsteerAngle;

	private float RotationValue1;

	private float RotationValue2;

	[HideInInspector]
	public bool brakingNow;

	[HideInInspector]
	public float steerInput;

	[HideInInspector]
	public bool crashed;

	private bool reversing;

	private AudioSource engineStartAudio;

	public AudioClip engineStartClip;

	private AudioSource engineAudio;

	public AudioClip engineClip;

	private AudioSource skidAudio;

	public AudioClip skidClip;

	private AudioSource crashAudio;

	public AudioClip[] crashClips;

	private AudioSource gearShiftingSound;

	public AudioClip[] gearShiftingClips;

	public GameObject WheelSlipPrefab;

	private List<GameObject> WheelParticles = new List<GameObject>();

	public ParticleSystem[] normalExhaustGas;

	public ParticleSystem[] heavyExhaustGas;

	public bool canControl;

	private bool headLightsOn;

	public Vector3 WheelRotation;

	private int _totalGears
	{
		get
		{
			return totalGears - 1;
		}
	}

	private void Start()
	{
		WheelRotation = FrontWheelTransform.localEulerAngles;
		headLightsOn = GetComponent<RMCPoliceLights>().GetHeadLightsOn();
		SoundsInitialize();
		if ((bool)WheelSlipPrefab)
		{
			SmokeInit();
		}
		rigid = GetComponent<Rigidbody>();
		rigid.constraints = RigidbodyConstraints.FreezeRotationZ;
		rigid.centerOfMass = new Vector3(COM.localPosition.x * base.transform.localScale.x, COM.localPosition.y * base.transform.localScale.y, COM.localPosition.z * base.transform.localScale.z);
		rigid.maxAngularVelocity = 2f;
		defsteerAngle = SteerAngle;
		StartCoroutine("WaitAndOff");
	}

	public AudioSource CreateAudioSource(string audioName, float minDistance, float volume, AudioClip audioClip, bool loop, bool playNow, bool destroyAfterFinished)
	{
		GameObject gameObject = new GameObject(audioName);
		gameObject.transform.position = base.transform.position;
		gameObject.transform.rotation = base.transform.rotation;
		gameObject.transform.parent = base.transform;
		gameObject.AddComponent<AudioSource>();
		gameObject.GetComponent<AudioSource>().minDistance = minDistance;
		gameObject.GetComponent<AudioSource>().volume = volume;
		gameObject.GetComponent<AudioSource>().clip = audioClip;
		gameObject.GetComponent<AudioSource>().loop = loop;
		gameObject.GetComponent<AudioSource>().spatialBlend = 1f;
		if (playNow)
		{
			gameObject.GetComponent<AudioSource>().Play();
		}
		if (destroyAfterFinished)
		{
			Object.Destroy(gameObject, audioClip.length);
		}
		return gameObject.GetComponent<AudioSource>();
	}

	public void SoundsInitialize()
	{
		engineAudio = CreateAudioSource("engineSound", 5f, 0f, engineClip, true, true, false);
		skidAudio = CreateAudioSource("skidSound", 5f, 0f, skidClip, true, true, false);
		engineStartAudio = CreateAudioSource("engineStartSound", 5f, 0.7f, engineStartClip, false, true, true);
	}

	public void SmokeInit()
	{
		string text = WheelSlipPrefab.name + "(Clone)";
		for (int i = 0; i < 2; i++)
		{
			Object.Instantiate(WheelSlipPrefab, base.transform.position, base.transform.rotation);
		}
		Object[] array = Object.FindObjectsOfType(typeof(GameObject));
		for (int j = 0; j < array.Length; j++)
		{
			GameObject gameObject = (GameObject)array[j];
			if (gameObject.name == text)
			{
				WheelParticles.Add(gameObject);
			}
		}
		WheelParticles[0].transform.position = FrontWheelCollider.transform.position;
		WheelParticles[1].transform.position = RearWheelCollider.transform.position;
		WheelParticles[0].transform.parent = FrontWheelCollider.transform;
		WheelParticles[1].transform.parent = RearWheelCollider.transform;
	}

	private void FixedUpdate()
	{
		if (canControl)
		{
			Inputs();
			Engine();
			Braking();
			ShiftGears();
			SkidAudio();
			Smoke();
		}
	}

	private void Update()
	{
		WheelRotation = FrontWheelTransform.localEulerAngles;
		if (canControl)
		{
			WheelAlign();
			Lean();
		}
	}

	private void Inputs()
	{
		Speed = rigid.velocity.magnitude * 3.6f;
		base.transform.eulerAngles = new Vector3(base.transform.eulerAngles.x, base.transform.eulerAngles.y, 0f);
		if (!crashed)
		{
			if (!changingGear)
			{
				motorInput = ControlFreak2.CF2Input.GetAxis("Vertical");
			}
			else
			{
				motorInput = Mathf.Clamp(ControlFreak2.CF2Input.GetAxis("Vertical"), -1f, 0f);
			}
			steerInput = ControlFreak2.CF2Input.GetAxis("Horizontal");
		}
		else
		{
			motorInput = 0f;
			steerInput = 0f;
		}
		if (motorInput < 0f && base.transform.InverseTransformDirection(rigid.velocity).z < 0f)
		{
			reversing = true;
		}
		else
		{
			reversing = false;
		}
	}

	private void Engine()
	{
		SteerAngle = Mathf.Lerp(defsteerAngle, highSpeedSteerAngle, Speed / highSpeedSteerAngleAtSpeed);
		FrontWheelCollider.steerAngle = SteerAngle * steerInput;
		EngineRPM = Mathf.Clamp((Mathf.Abs(FrontWheelCollider.rpm + RearWheelCollider.rpm) * gearShiftRate + MinEngineRPM) / (float)(currentGear + 1), MinEngineRPM, MaxEngineRPM);
		engineAudio.volume = Mathf.Lerp(engineAudio.volume, Mathf.Clamp(motorInput, 0.35f, 0.85f), Time.deltaTime * 5f);
		engineAudio.pitch = Mathf.Lerp(engineAudio.pitch, Mathf.Lerp(1f, 2f, (EngineRPM - MinEngineRPM / 1.5f) / (MaxEngineRPM + MinEngineRPM)), Time.deltaTime * 5f);
		if ((bool)engineStartAudio)
		{
			engineStartAudio.GetComponent<AudioSource>().volume -= Time.deltaTime / 5f;
		}
		if (Speed > maxSpeed)
		{
			RearWheelCollider.motorTorque = 0f;
		}
		else if (!reversing && !changingGear)
		{
			RearWheelCollider.motorTorque = EngineTorque * Mathf.Clamp(motorInput, 0f, 1f) * engineTorqueCurve[currentGear].Evaluate(Speed);
		}
		if (reversing)
		{
			if (Speed < 10f)
			{
				RearWheelCollider.motorTorque = EngineTorque * motorInput / 5f;
			}
			else
			{
				RearWheelCollider.motorTorque = 0f;
			}
		}
	}

	public void Braking()
	{
		if (Mathf.Abs(motorInput) <= 0.05f)
		{
			brakingNow = false;
			FrontWheelCollider.brakeTorque = Brake / 25f;
			RearWheelCollider.brakeTorque = Brake / 25f;
		}
		else if (motorInput < 0f && !reversing)
		{
			brakingNow = true;
			FrontWheelCollider.brakeTorque = Brake * (Mathf.Abs(motorInput) / 5f);
			RearWheelCollider.brakeTorque = Brake * Mathf.Abs(motorInput);
		}
		else
		{
			brakingNow = false;
			FrontWheelCollider.brakeTorque = 0f;
			RearWheelCollider.brakeTorque = 0f;
		}
	}

	private void WheelAlign()
	{
		Vector3 vector = FrontWheelCollider.transform.TransformPoint(FrontWheelCollider.center);
		WheelHit hit;
		FrontWheelCollider.GetGroundHit(out hit);
		RaycastHit hitInfo;
		if (Physics.Raycast(vector, -FrontWheelCollider.transform.up, out hitInfo, (FrontWheelCollider.suspensionDistance + FrontWheelCollider.radius) * base.transform.localScale.y))
		{
			if (hitInfo.transform.gameObject.layer != LayerMask.NameToLayer("Bike"))
			{
				FrontWheelTransform.transform.position = hitInfo.point + FrontWheelCollider.transform.up * FrontWheelCollider.radius * base.transform.localScale.y;
				if ((bool)Fender)
				{
					Fender.transform.position = hitInfo.point + FrontWheelCollider.transform.up * (FrontWheelCollider.radius + FrontWheelCollider.suspensionDistance) * base.transform.localScale.y;
				}
				float num = (0f - FrontWheelCollider.transform.InverseTransformPoint(hit.point).y - FrontWheelCollider.radius) / FrontWheelCollider.suspensionDistance;
				Debug.DrawLine(hit.point, hit.point + FrontWheelCollider.transform.up * (hit.force / 8000f), (!(num <= 0f)) ? Color.white : Color.magenta);
				Debug.DrawLine(hit.point, hit.point - FrontWheelCollider.transform.forward * hit.forwardSlip, Color.green);
				Debug.DrawLine(hit.point, hit.point - FrontWheelCollider.transform.right * hit.sidewaysSlip, Color.red);
			}
		}
		else
		{
			FrontWheelTransform.transform.position = vector - FrontWheelCollider.transform.up * FrontWheelCollider.suspensionDistance * base.transform.localScale.y;
		}
		RotationValue1 += FrontWheelCollider.rpm * 6f * Time.deltaTime;
		FrontWheelTransform.transform.rotation = FrontWheelCollider.transform.rotation * Quaternion.Euler(RotationValue1, FrontWheelCollider.steerAngle, FrontWheelCollider.transform.rotation.z);
		Vector3 vector2 = RearWheelCollider.transform.TransformPoint(RearWheelCollider.center);
		RearWheelCollider.GetGroundHit(out hit);
		if (Physics.Raycast(vector2, -RearWheelCollider.transform.up, out hitInfo, (RearWheelCollider.suspensionDistance + RearWheelCollider.radius) * base.transform.localScale.y))
		{
			if (hitInfo.transform.gameObject.layer != LayerMask.NameToLayer("Bike"))
			{
				RearWheelTransform.transform.position = hitInfo.point + RearWheelCollider.transform.up * RearWheelCollider.radius * base.transform.localScale.y;
				float num2 = (0f - RearWheelCollider.transform.InverseTransformPoint(hit.point).y - RearWheelCollider.radius) / RearWheelCollider.suspensionDistance;
				Debug.DrawLine(hit.point, hit.point + RearWheelCollider.transform.up * (hit.force / 8000f), (!(num2 <= 0f)) ? Color.white : Color.magenta);
				Debug.DrawLine(hit.point, hit.point - RearWheelCollider.transform.forward * hit.forwardSlip, Color.green);
				Debug.DrawLine(hit.point, hit.point - RearWheelCollider.transform.right * hit.sidewaysSlip, Color.red);
			}
		}
		else
		{
			RearWheelTransform.transform.position = vector2 - RearWheelCollider.transform.up * RearWheelCollider.suspensionDistance * base.transform.localScale.y;
		}
		RotationValue2 += RearWheelCollider.rpm * 6f * Time.deltaTime;
		RearWheelTransform.transform.rotation = RearWheelCollider.transform.rotation * Quaternion.Euler(RotationValue2, RearWheelCollider.steerAngle, RearWheelCollider.transform.rotation.z);
		if ((bool)SteeringHandlebar)
		{
			SteeringHandlebar.transform.rotation = FrontWheelCollider.transform.rotation * Quaternion.Euler(0f, FrontWheelCollider.steerAngle, FrontWheelCollider.transform.rotation.z);
		}
		if ((bool)Fender)
		{
			Fender.rotation = FrontWheelCollider.transform.rotation * Quaternion.Euler(0f, FrontWheelCollider.steerAngle, FrontWheelCollider.transform.rotation.z);
		}
	}

	public void ShiftGears()
	{
		if (currentGear < _totalGears && !changingGear && EngineRPM > MaxEngineRPM - 500f && RearWheelCollider.rpm >= 0f)
		{
			StartCoroutine("ChangingGear", currentGear + 1);
		}
		if (currentGear <= 0 || !(EngineRPM < MinEngineRPM + 500f) || changingGear)
		{
			return;
		}
		for (int i = 0; i < gearSpeed.Length; i++)
		{
			if (Speed > gearSpeed[i])
			{
				StartCoroutine("ChangingGear", i);
			}
		}
	}

	private IEnumerator ChangingGear(int gear)
	{
		changingGear = true;
		if (gearShiftingClips.Length >= 1)
		{
			gearShiftingSound = CreateAudioSource("gearShiftingAudio", 5f, 0.3f, gearShiftingClips[UnityEngine.Random.Range(0, gearShiftingClips.Length)], false, true, true);
		}
		yield return new WaitForSeconds(0.5f);
		changingGear = false;
		currentGear = gear;
	}

	private void Lean()
	{
		verticalLean = Mathf.Clamp(Mathf.Lerp(verticalLean, base.transform.InverseTransformDirection(rigid.angularVelocity).x * chassisVerticalLean, Time.deltaTime * 5f), -10f, 10f);
		WheelHit hit;
		FrontWheelCollider.GetGroundHit(out hit);
		float num = Mathf.Clamp(hit.sidewaysSlip, -1f, 1f);
		num = ((!(base.transform.InverseTransformDirection(rigid.velocity).z > 0f)) ? 1f : (-1f));
		horizontalLean = Mathf.Clamp(Mathf.Lerp(horizontalLean, base.transform.InverseTransformDirection(rigid.angularVelocity).y * num * chassisHorizontalLean, Time.deltaTime * 3f), -50f, 50f);
		Quaternion localRotation = Quaternion.Euler(verticalLean, chassis.transform.localRotation.y + rigid.angularVelocity.z, horizontalLean);
		chassis.transform.localRotation = localRotation;
		rigid.centerOfMass = new Vector3(COM.localPosition.x * base.transform.localScale.x, COM.localPosition.y * base.transform.localScale.y, COM.localPosition.z * base.transform.localScale.z);
	}

	public void Smoke()
	{
		if (WheelParticles.Count > 0)
		{
			WheelHit hit;
			FrontWheelCollider.GetGroundHit(out hit);
			if (Mathf.Abs(hit.sidewaysSlip) > 0.25f || Mathf.Abs(hit.forwardSlip) > 0.5f)
			{
				WheelParticles[0].GetComponent<ParticleSystem>().Play();
			}
			else
			{
				WheelParticles[0].GetComponent<ParticleSystem>().Stop();
			}
			WheelHit hit2;
			RearWheelCollider.GetGroundHit(out hit2);
			if (Mathf.Abs(hit2.sidewaysSlip) > 0.25f || Mathf.Abs(hit2.forwardSlip) > 0.5f)
			{
				WheelParticles[1].GetComponent<ParticleSystem>().Play();
			}
			else
			{
				WheelParticles[1].GetComponent<ParticleSystem>().Stop();
			}
		}
		if (normalExhaustGas.Length > 0)
		{
			ParticleSystem[] array = normalExhaustGas;
			foreach (ParticleSystem particleEmitter in array)
			{
				if (Speed < 20f)
				{
					particleEmitter.Play();
				}
				else
				{
					particleEmitter.Stop();
				}
			}
		}
		if (heavyExhaustGas.Length <= 0)
		{
			return;
		}
		ParticleSystem[] array2 = heavyExhaustGas;
		foreach (ParticleSystem particleEmitter2 in array2)
		{
			if (Speed < 20f && motorInput > 0.5f)
			{
				particleEmitter2.Play();
			}
			else
			{
				particleEmitter2.Stop();
			}
		}
	}

	public void SkidAudio()
	{
		if (!skidAudio)
		{
			return;
		}
		WheelHit hit;
		FrontWheelCollider.GetGroundHit(out hit);
		WheelHit hit2;
		RearWheelCollider.GetGroundHit(out hit2);
		if (Mathf.Abs(hit.sidewaysSlip) > 0.25f || Mathf.Abs(hit2.forwardSlip) > 0.5f || Mathf.Abs(hit.forwardSlip) > 0.5f)
		{
			if (rigid.velocity.magnitude > 1f)
			{
				skidAudio.volume = Mathf.Abs(hit.sidewaysSlip) + (Mathf.Abs(hit.forwardSlip) + Mathf.Abs(hit2.forwardSlip)) / 4f;
			}
			else
			{
				skidAudio.volume -= Time.deltaTime;
			}
		}
		else
		{
			skidAudio.volume -= Time.deltaTime;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.contacts.Length > 0 && collision.relativeVelocity.magnitude > 10f && crashClips.Length > 0 && collision.contacts[0].thisCollider.gameObject.transform != base.transform.parent)
		{
			crashAudio = CreateAudioSource("crashSound", 5f, 1f, crashClips[UnityEngine.Random.Range(0, crashClips.Length)], false, true, true);
		}
	}

	private IEnumerator WaitAndOff()
	{
		yield return new WaitForSeconds(2f);
		base.enabled = false;
		GetComponent<Rigidbody>().isKinematic = true;
	}

	public bool GetLightState()
	{
		return headLightsOn;
	}
}
