using Photon;
using UnityEngine;

public class GlobalCarManager : Photon.MonoBehaviour
{
	public CarWrapper[] AllCars;
	public GameObject[] DefaultCarPack;
	public GameObject[] CarPack1;
	public GameObject[] CarPack2;
	public GameObject[] CarPack3;
	public GameObject[] CarPack4;

	public static GlobalCarManager I;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	public void OnEnable()
	{
		/*if (PhotonNetwork.room != null)
		{
			if (PhotonNetwork.room.name == "Survival PvP #1")
			{
				EnableCars(CarPack1);
			}
			else if (PhotonNetwork.room.name == "Survival PvP #2")
			{
				EnableCars(CarPack2);
			}
			else if (PhotonNetwork.room.name == "Survival PvP #3")
			{
				EnableCars(CarPack3);
			}
			else if (PhotonNetwork.room.name == "Survival PvP #4")
			{
				EnableCars(CarPack4);
			}
			else
			{
				EnableCars(DefaultCarPack);
			}
		}*/
	}

	public void OnDisable()
	{
		JsSpeeker.I.StopPlayingSong();
	}

	public void EnableCars(GameObject[] cars)
	{
		for (int i = 0; i < cars.Length; i++)
		{
			if (cars[i] != null)
			{
				cars[i].SetActive(true);
			}
		}
	}

	public void ExitFromCarIfEixst(string playerId)
	{
		for (byte b = 0; b < AllCars.Length; b++)
		{
			for (int i = 0; i < AllCars[b].seats.Length; i++)
			{
				if (AllCars[b].seats[i].occupied && AllCars[b].seats[i].playerId == playerId)
				{
					AllCars[b].CallClearSeats(AllCars[b].seats[i].door.DoorId);
				}
			}
		}
	}

	public byte GetCarId(CarWrapper car)
	{
		byte result = byte.MaxValue;
		for (byte b = 0; b < AllCars.Length; b++)
		{
			if (AllCars[b] == car)
			{
				result = b;
			}
		}
		return result;
	}

	public CarWrapper GetCarWrapperById(byte carId)
	{
		CarWrapper result = null;
		for (byte b = 0; b < AllCars.Length; b++)
		{
			if (b == carId)
			{
				result = AllCars[b];
			}
		}
		return result;
	}
}
