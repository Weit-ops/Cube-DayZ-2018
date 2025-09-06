using UnityEngine;

public class CarCameraManager : MonoBehaviour
{
	public RCCCarCamera RCCCarCam;

	public static CarCameraManager I;

	public GameObject CarCamera;

	public MouseOrbitWithZoom orbitCamera;

	public GameObject motoCamera;

	private bool _OrbitEnable = true;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	private void Update()
	{
		if ((!(GameControls.I != null) || !(GameControls.I.Player != null) || !(RCCCarCam.playerCar != null) || GameControls.I.Player.inTheCar) && Input.GetKeyUp(KeyCode.O))
		{
			SwitchOrbitCamera(_OrbitEnable);
		}
	}

	public void CarCameraEnable(bool flag, GameObject bikeCamera, bool isBike = false)
	{
		if (!isBike)
		{
			CarCamera.SetActive(flag);
			return;
		}
		motoCamera = bikeCamera;
		motoCamera.SetActive(flag);
	}

	public void SetTargetCar(Transform target)
	{
		RCCCarCam.playerCar = target;
	}

	public void SwitchOrbitCamera(bool isOrbit)
	{
		if (isOrbit)
		{
			orbitCamera.enabled = true;
			RCCCarCam.enabled = false;
			_OrbitEnable = false;
		}
		else
		{
			RCCCarCam.enabled = true;
			orbitCamera.enabled = false;
			_OrbitEnable = true;
		}
	}
}
