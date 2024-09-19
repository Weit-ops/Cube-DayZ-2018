using Photon;
using UnityEngine;

public class HerdSimLootController : Photon.MonoBehaviour
{
	public static HerdSimLootController I;

	private void Awake()
	{
		I = this;
	}

	[PunRPC]
	private void DropLoot(string meatId, byte count, Vector3 position)
	{
		Item itemInfo = WorldController.I.Info.GetItemInfo(meatId);
		RaycastHit hitInfo;
		if (itemInfo != null && Physics.Raycast(position + Vector3.up, -Vector3.up, out hitInfo, 15f, ItemsRegistry.I.ItemsCollisionsMask))
		{
			Quaternion rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, hitInfo.normal), Vector3.Cross(Vector3.up, hitInfo.normal));
			PhotonNetwork.InstantiateSceneObject("PhotonItem", hitInfo.point, rotation, 0, new object[4]
			{
				itemInfo.Id,
				count,
				true,
				(byte)0
			});
		}
	}
}
