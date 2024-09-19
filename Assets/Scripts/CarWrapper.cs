using System;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;
using Photon;
using UnityEngine;

[Serializable]
public class VehicleSeat
{
	public Transform seat_transform;
	public bool occupied;
	public string playerId = "0";
	public CarDoor door;
}

public enum CarDoorsType
{
	FL = 0,
	BL = 1,
	FR = 2,
	BR = 3,
	Bag = 4
}

public class CarWrapper : Photon.MonoBehaviour
{
	[Serializable]
	private class SyncCarPlayer
	{
		public string playerId;
		public byte seatsId;
	}

	public VehicleSeat[] seats;

	[SerializeField] RCCCarControllerV2 carController;
	[SerializeField] AudioSource audioS;
	[SerializeField] RMCRealisticMotorcycleController bikeController;

	public bool IsVip;
	public int VipId;
	public bool isBike;
	public int VkMissionId;
	public GameObject bikeCamera;
	public int _oldCarID;

	public void GetInTheCar(CarDoorsType seatId)
	{
		if (!IsSeatOccupied(seatId))
		{
			if (IsVip && VipId > 0 && seatId == CarDoorsType.FL)
			{
				PurchasedItemsBackensInfo purchasedItemsBackensInfo = DataKeeper.BackendInfo.purchased_items.Find((PurchasedItemsBackensInfo item) => item.shop_id == VipId && item.count > 0);
				if (purchasedItemsBackensInfo == null)
				{
					VipCarAlarm component = GetComponent<VipCarAlarm>();
					if (component != null)
					{
						component.StartAlarm();
					}
				}
				else
				{
					SwitchToCar(true, seatId);
				}
			}
			else
			{
				SwitchToCar(true, seatId);
			}
		}
		else
		{
			Debug.LogError("This seat id occupied  " + seatId);
		}
	}

	public void ExitFromCar(CarDoorsType seatId)
	{
		SwitchToCar(false, seatId);
		GameControls.I.Player.canUseExit = false;
		Debug.Log("Exit from car! " + seatId);
	}

	private void SwitchToCar(bool carActivate, CarDoorsType doorType)
	{
		if (carActivate)
		{
			GameControls.I.Player.StartCorWaitExit();
			base.photonView.RPC("SeatOccupied", PhotonTargets.All, true, (byte)doorType, JsSpeeker.viewer_id);
			GameControls.I.Player.transform.parent = seats[(int)doorType].seat_transform;
			GameControls.I.Player.GetComponent<CapsuleCollider>().enabled = false;
			GameControls.I.Player.transform.localPosition = new Vector3(0f, 0f, 0f);
			GameControls.I.Player.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			GameControls.I.Player.transform.Find("Player(Clone)").transform.GetChild(0).gameObject.SetActive(true);
			DisAblePlayerController(true);
			if (doorType == CarDoorsType.FL)
			{
				if (base.photonView.ownerId != PhotonNetwork.player.ID) {
					base.photonView.RequestOwnership ();
				} 
				GetComponent<Rigidbody>().isKinematic = false;
				ToggleCarController(true);
				if (!isBike)
				{
					carController.canControl = true;
				}
				else
				{
					bikeController.canControl = true;
				}
				audioS.enabled = false;
				JsSpeeker.I.PlaySong();
				GameUIController.I.EnableCarInfo(true);
				SetDrivingMission(VkMissionId);
				VipCarManager.I.InTheCar = true;
			}
			EnableCarCamera(true);
			GameControls.I.Player.inTheCar = true;
			PhotonMan component = GameControls.I.Player.transform.Find("Player(Clone)").GetComponent<PhotonMan>();
			if (component != null)
			{
				component.EnterTheCar(true, GlobalCarManager.I.GetCarId(this), (byte)doorType);
			}
			return;
		}

		base.photonView.RPC("SeatOccupied", PhotonTargets.All, false, (byte)doorType, "0");
		Vector3 localPosition = GameControls.I.Player.transform.localPosition;

		if (doorType == CarDoorsType.FL || doorType == CarDoorsType.BL)
		{
			localPosition.y += 35f;
		}

		if (doorType == CarDoorsType.FR || doorType == CarDoorsType.BR)
		{
			localPosition.y += 35f;
		}

		GameControls.I.Player.transform.localPosition = localPosition;
		GameControls.I.Player.transform.parent = null;
		GameControls.I.Player.GetComponent<CapsuleCollider>().enabled = true;
		GameControls.I.Player.transform.Find("Player(Clone)").transform.GetChild(0).gameObject.SetActive(false);

		if (doorType == CarDoorsType.FL)
		{
			GetComponent<Rigidbody>().isKinematic = true;
			if (!isBike)
			{
				carController.canControl = false;
			}
			else
			{
				bikeController.canControl = false;
			}
			ToggleCarController(false);
			StopDrivingMission();
		}

		EnableCarCamera(false);
		DisAblePlayerController(false);
		GameControls.I.Player.myCurrentDoor = null;
		GameControls.I.Player.myCurentCarWrapper = null;
		GameControls.I.Player.inTheCar = false;
		PhotonMan component2 = GameControls.I.Player.transform.Find("Player(Clone)").GetComponent<PhotonMan>();

		if (component2 != null)
		{
			component2.EnterTheCar(false, GlobalCarManager.I.GetCarId(this), (byte)doorType);
		}
		GameUIController.I.EnableCarInfo(false);
		VipCarManager.I.InTheCar = false;
		VipCarManager.I.ShowSelectMenu(false);
	}

	public void EnableCarCamera(bool flag)
	{
		if (CarCameraManager.I != null)
		{
			if (!isBike)
			{
				CarCameraManager.I.CarCameraEnable(flag, null);
				CarCameraManager.I.SetTargetCar(base.transform);
				CarCameraManager.I.orbitCamera.SetTargetCar(base.transform);
			}
			else
			{
				CarCameraManager.I.CarCameraEnable(flag, bikeCamera, true);
			}
		}
	}

	public void DisAblePlayerController(bool flag)
	{
		if (flag)
		{
			GameControls.I.MenuControls(flag, false);
			GameControls.I.DisableCameraAndWeapons(!flag);
		}
		else
		{
			GameControls.I.DisableCameraAndWeapons(!flag);
			GameControls.I.MenuControls(flag);
		}
	}

	public bool IsSeatOccupied(CarDoorsType doorId)
	{
		return false || seats[(int)doorId].occupied;
	}

	public void ToggleCarController(bool flag)
	{
		if (!isBike)
		{
			carController.enabled = flag;
		}
		else
		{
			bikeController.enabled = flag;
		}
	}

	public void CallClearSeats(CarDoorsType seatId)
	{
		base.photonView.RPC("SeatOccupied", PhotonTargets.All, false, (byte)seatId, "0");
	}

	[PunRPC]
	public void SeatOccupied(bool flag, byte seatNumber, string playerId)
	{
		if (flag)
		{
			seats[seatNumber].occupied = true;
			seats[seatNumber].playerId = playerId;
			if (seatNumber == 0)
			{
				if (GameControls.I.Player.inTheCar && GameControls.I.Player.myCurentCarWrapper == this)
				{
					JsSpeeker.I.PlaySong();
				}
				audioS.enabled = true;
			}
			for (int i = 0; i < GetComponentsInChildren<AudioSource> ().Length; i++) {
				GetComponentsInChildren<AudioSource> () [i].Play ();
			}
			return;
		}
		Debug.Log("Seat clear " + seatNumber + "  " + seats[seatNumber]);
		seats[seatNumber].occupied = false;
		seats[seatNumber].playerId = "0";
		if (seatNumber == 0)
		{
			JsSpeeker.I.StopPlayingSong();
			audioS.enabled = false;
		}
		for (int i = 0; i < GetComponentsInChildren<AudioSource> ().Length; i++) {
			GetComponentsInChildren<AudioSource> () [i].Stop ();
		}
	}

	private IEnumerator WaitAndOffCarSynch()
	{
		yield return new WaitForSeconds(1f);
	}

	private byte IfAnybodyInCar()
	{
		byte b = 0;
		for (int i = 0; i < seats.Length; i++)
		{
			if (seats[i].occupied)
			{
				b++;
			}
		}
		return b;
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (!PhotonNetwork.isMasterClient || IfAnybodyInCar() <= 0)
		{
			return;
		}
		List<SyncCarPlayer> list = new List<SyncCarPlayer>();
		for (byte b = 0; b < seats.Length; b++)
		{
			if (seats[b].occupied)
			{
				SyncCarPlayer syncCarPlayer = new SyncCarPlayer();
				syncCarPlayer.seatsId = b;
				syncCarPlayer.playerId = seats[b].playerId;
				list.Add(syncCarPlayer);
			}
		}
		string text = JsonWriter.Serialize(list);
		base.photonView.RPC("SyncPlayersInCar", player, text);
	}

	[PunRPC]
	public void SyncPlayersInCar(string param)
	{
		if (param != null && param.Length > 0)
		{
			SynchThisCar(param);
		}
	}

	private void SynchThisCar(string param)
	{
		SyncCarPlayer[] array = JsonReader.Deserialize<SyncCarPlayer[]>(param);
		for (int i = 0; i < array.Length; i++)
		{
			seats[array[i].seatsId].occupied = true;
			seats[array[i].seatsId].playerId = array[i].playerId;
			PhotonMan photonMan = WorldController.I.WorldPlayers[array[i].playerId];
			if (photonMan != null)
			{
				photonMan.SeatInCarOnStart(GlobalCarManager.I.GetCarId(this), array[i].seatsId);
			}
		}
	}

	public void SwitchSound(bool flag)
	{
		audioS.enabled = flag;
	}

	private void SetDrivingMission(int missionId)
	{
		StopDrivingMission();
		if (missionId > 0)
		{
			StartCoroutine("UnlockDrivingMission", missionId);
		}
	}

	private void StopDrivingMission()
	{
		StopCoroutine("UnlockDrivingMission");
	}

	[PunRPC]
	private void ExitMeFromCar()
	{
		GameControls.I.Player.ExitFromMyCar();
	}

	public void StartChangeCarToVip(int vipCarId)
	{
		StartCoroutine("ChangeCarToVip", vipCarId);
	}

	private IEnumerator ChangeCarToVip(int vipCarId)
	{
		DropThePassengers();
		Vector3 carPos = base.gameObject.transform.localPosition;
		PhotonView vipPhotonView = VipCarManager.I.VipCars[vipCarId].transform.GetComponent<PhotonView>();
		if (!(vipPhotonView != null) || !(base.photonView != null))
		{
			yield break;
		}
		if (vipPhotonView.ownerId != PhotonNetwork.player.ID)
		{
			vipPhotonView.RequestOwnership();
		}
		if (base.photonView.ownerId != PhotonNetwork.player.ID)
		{
			base.photonView.RequestOwnership();
		}
		yield return new WaitForSeconds(1f);
		if (vipPhotonView.ownerId == PhotonNetwork.player.ID && base.photonView.ownerId == PhotonNetwork.player.ID)
		{
			VipCarManager.I.VipCars[vipCarId].transform.localPosition = new Vector3(carPos.x, carPos.y + 0.5f, carPos.z);
			base.gameObject.transform.localPosition = new Vector3(carPos.x, carPos.y - 500f, carPos.z);
			if (!IsVip)
			{
				vipPhotonView.RPC("RememberOldCarId", PhotonTargets.All, GlobalCarManager.I.GetCarId(this));
			}
			else
			{
				vipPhotonView.RPC("RememberOldCarId", PhotonTargets.All, (byte)_oldCarID);
			}
		}
	}

	public void StartChangeCarVipToCommon()
	{
		StartCoroutine("ChangeCarVipToCommon");
	}

	private IEnumerator ChangeCarVipToCommon()
	{
		Vector3 carPos = base.gameObject.transform.localPosition;
		PhotonView oldCarPhotonView = GlobalCarManager.I.AllCars[_oldCarID].transform.GetComponent<PhotonView>();
		if (oldCarPhotonView != null && base.photonView != null)
		{
			if (oldCarPhotonView.ownerId != PhotonNetwork.player.ID)
			{
				oldCarPhotonView.RequestOwnership();
			}
			if (base.photonView.ownerId != PhotonNetwork.player.ID)
			{
				base.photonView.RequestOwnership();
			}
			yield return new WaitForSeconds(1f);
			if (oldCarPhotonView.ownerId == PhotonNetwork.player.ID && base.photonView.ownerId == PhotonNetwork.player.ID)
			{
				DropThePassengers();
				GlobalCarManager.I.AllCars[_oldCarID].transform.localPosition = new Vector3(carPos.x, carPos.y, carPos.z);
				base.gameObject.transform.localPosition = new Vector3(carPos.x, carPos.y - 500f, carPos.z);
			}
		}
	}

	private void DropThePassengers()
	{
		for (int i = 0; i < seats.Length; i++)
		{
			if (seats[i].playerId != "0")
			{
				PhotonMan photonMan = WorldController.I.WorldPlayers[seats[i].playerId];
				if (photonMan != null)
				{
					base.photonView.RPC("ExitMeFromCar", photonMan.photonView.owner);
				}
			}
		}
	}

	[PunRPC]
	public void RememberOldCarId(byte vipCarId)
	{
		_oldCarID = vipCarId;
	}

	public void PokeWheel()
	{
	}

	public void OnDriverDie()
	{
	}

	public void OnPassengerDie()
	{
	}

	public void OpenTrunk()
	{
	}

	public void Beep()
	{
	}

	public void Ignition()
	{
	}

	public void ToggleLights()
	{
	}

	public void ShootFromCar()
	{
	}

	public void SetDrivingWheelEulerY()
	{
	}
}
