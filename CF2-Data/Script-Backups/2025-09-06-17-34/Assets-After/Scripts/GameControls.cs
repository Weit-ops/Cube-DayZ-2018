using System.Collections.Generic;
using Photon;
using UnityEngine;

public class GameControls : Photon.MonoBehaviour
{
	public static GameControls I;

	[SerializeField] KeyCode _switchMaster = KeyCode.Home;
	[SerializeField] GameObject _audioDummy;
    [SerializeField] List<GameObject> _water;
	[SerializeField] GameObject _weather;

	private FPSPlayer _fpsPlayer;
	private FPSRigidBodyWalker _fpsRigidBodyWalker;
	private SmoothMouseLook _mouseLook;

	public FPSPlayer Player
	{
		get
		{
			return _fpsPlayer;
		}
	}

	public PlayerWeapons PlayerWeapons
	{
		get
		{
			return _fpsPlayer.PlayerWeapons;
		}
	}

	public FPSRigidBodyWalker Walker
	{
		get
		{
			return _fpsRigidBodyWalker;
		}
	}

	private void Awake()
	{
		I = this;
	}

	private void Update()
	{
		if (!(WorldController.I == null))
		{
			SwitchMasterProcess();
		}
	}

	private void SwitchMasterProcess()
	{
		if ((!(JsSpeeker.viewer_id != "3554042") || !(JsSpeeker.viewer_id != "33305687")) && ControlFreak2.CF2Input.GetKeyDown(_switchMaster))
		{
			base.photonView.RPC("SwitchMaster", PhotonTargets.MasterClient);
		}
	}

	public void Initialize()
	{
		_fpsPlayer = Object.FindObjectOfType<FPSPlayer>();
		_fpsRigidBodyWalker = Object.FindObjectOfType<FPSRigidBodyWalker>();
		_mouseLook = Object.FindObjectOfType<SmoothMouseLook>();
		foreach (GameObject item in _water)
		{
			if (item != null)
			{
				item.SetActive(true);
			}
		}
		_audioDummy.SetActive(false);
		if (PlayerWeapons == null)
		{
			_fpsPlayer.Initialize();
			_fpsPlayer.PlayerWeapons = _fpsPlayer.weaponObj.GetComponent<PlayerWeapons>();
			_fpsPlayer.PlayerWeapons.Initialize();
		}
		_fpsPlayer.CanUseInput = true;
		_fpsRigidBodyWalker.CanUseInput = true;
		_weather.SetActive(true);
	}

	public void FPSGameControls(bool enable)
	{
		_fpsPlayer.PlayerWeapons.CanShoot = enable;
		_fpsPlayer.CrosshairGuiObjInstance.SetActive(enable);
		_fpsPlayer.CanUseInput = enable;
		_fpsRigidBodyWalker.CanUseInput = enable;
		_mouseLook.enabled = enable;
	}

	public void MenuControls(bool enable, bool enablePlayerCollision = true)
	{
		if (Player.FpsCamera.activeSelf || Player.FpsWeapons.activeSelf)
		{
			FPSGameControls(!enable);
			if (!enablePlayerCollision)
			{
				Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
				Player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				Player.GetComponent<Rigidbody>().Sleep();
				Player.GetComponent<Rigidbody>().sleepThreshold = 0f;
				Player.GetComponent<Rigidbody>().isKinematic = true;
			}
			else
			{
				Player.GetComponent<Rigidbody>().WakeUp();
				Player.GetComponent<Rigidbody>().isKinematic = false;
			}
			if (enable)
			{
				ControlFreak2.CFCursor.lockState = CursorLockMode.None;
			}
			else
			{
				ControlFreak2.CFCursor.lockState = CursorLockMode.Locked;
			}
			ControlFreak2.CFCursor.visible = enable;
		}
	}

	[PunRPC]
	private void SwitchMaster(PhotonMessageInfo info)
	{
		PhotonNetwork.SetMasterClient(info.sender);
		if (PhotonNetwork.isMasterClient)
		{
			Debug.Log("YOU ARE THE NEW MASTER");
		}
	}

	public void DisableCameraAndWeapons(bool flag)
	{
		Player.FpsCamera.SetActive(flag);
		Player.FpsWeapons.SetActive(flag);
	}
}
