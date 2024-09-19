using UnityEngine;

public class CarDoor : MonoBehaviour
{
	public CarDoorsType DoorId;

	public CarWrapper MyCarWrapper;

	public void OpenDoor()
	{
		MyCarWrapper.GetInTheCar(DoorId);
	}
}
