using Photon;
using UnityEngine;

public class NetworkPlayer : Photon.MonoBehaviour
{
	public Light[] headLights;

	public Transform FrontLeftWheelTransform;
	public Transform FrontRightWheelTransform;
	public Transform RearLeftWheelTransform;
	public Transform RearRightWheelTransform;
	public RCCCarControllerV2 carContr;
	public RMCRealisticMotorcycleController motContr;

	public Vector3 SyncPos;
	public Quaternion SyncRot;

	[Header("Synchronize wheels ")]
	private Quaternion WheelFL;
	[Header("Synchronize wheels ")]
	private Quaternion WheelFR;
	[Header("Synchronize wheels ")]
	private Quaternion WheelBL;
	[Header("Synchronize wheels ")]
	private Quaternion WheelBR;

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(base.transform.position);
			stream.SendNext(base.transform.rotation);

			if (carContr != null) {
				stream.SendNext (FrontLeftWheelTransform.transform.rotation);
				stream.SendNext (FrontRightWheelTransform.transform.rotation);
				stream.SendNext (RearLeftWheelTransform.transform.rotation);
				stream.SendNext (RearRightWheelTransform.transform.rotation);
			}

			if (motContr != null) {
				stream.SendNext (FrontLeftWheelTransform.transform.rotation);
				stream.SendNext (RearLeftWheelTransform.transform.rotation);
			}
			return;
		}

		SyncPos = (Vector3)stream.ReceiveNext();
		SyncRot = (Quaternion)stream.ReceiveNext();

		WheelFL = (Quaternion)stream.ReceiveNext();
		WheelFR = (Quaternion)stream.ReceiveNext();

		if (carContr != null) {
			WheelBL = (Quaternion)stream.ReceiveNext ();
			WheelBR = (Quaternion)stream.ReceiveNext ();
		}
	}

	private void Update()
	{
		if (!base.photonView.isMine)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, SyncPos, 0.14f);
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, SyncRot, 0.08f);

			if (carContr != null) {
				FrontLeftWheelTransform.rotation  = Quaternion.Lerp(FrontLeftWheelTransform.transform.rotation, WheelFL, 0.08f);
				FrontRightWheelTransform.rotation = Quaternion.Lerp(FrontRightWheelTransform.transform.rotation, WheelFR, 0.08f);
				RearLeftWheelTransform.rotation   = Quaternion.Lerp(RearLeftWheelTransform.transform.rotation, WheelBL, 0.08f);
				RearRightWheelTransform.rotation  = Quaternion.Lerp(RearRightWheelTransform.transform.rotation, WheelBR, 0.08f);
			}

			if (motContr != null) {
				FrontLeftWheelTransform.rotation  = Quaternion.Lerp(FrontLeftWheelTransform.transform.rotation, WheelFL, 0.08f);
				RearLeftWheelTransform.rotation   = Quaternion.Lerp(RearLeftWheelTransform.transform.rotation, WheelFR, 0.08f);
			}
		}
	}

	private void SwitchLight(bool flag)
	{
		for (int i = 0; i < headLights.Length; i++)
		{
			if (flag)
			{
				headLights[i].enabled = true;
			}
			else
			{
				headLights[i].enabled = false;
			}
		}
	}
}
