using System.Collections;
using Photon;
using UnityEngine;

public class CustomPhotonSectorController : Photon.MonoBehaviour
{
	public float CheckInterval = 2f;
	public float ChangeSectorWaitTime = 3f;
	public PhotonSectorInfo MySector;
	public PhotonSectorInfo OldSector;

	private void Start()
	{
		if (PhotonNetwork.isMasterClient)
		{
			StartCoroutine(CheckSector());
		}
	}

	public virtual void OnSectorChanges()
	{
	}

	private void OnMasterClientSwitched()
	{
		StopAllCoroutines();
		if (PhotonNetwork.isMasterClient)
		{
			StartCoroutine(CheckSector());
		}
	}

	private IEnumerator CheckSector()
	{
		PhotonSectorInfo newSector = PhotonSectorService.I.GetSector(base.transform.position);
		if (newSector != MySector)
		{
			StartCoroutine(ChangeSector(newSector));
		}
		yield return new WaitForSeconds(CheckInterval);
		StartCoroutine(CheckSector());
	}

	private IEnumerator ChangeSector(PhotonSectorInfo newSector)
	{
		yield return new WaitForSeconds(ChangeSectorWaitTime);
		if (newSector != MySector)
		{
			base.photonView.RPC("ObjectChangeSector", PhotonTargets.All, newSector.RowIndex, newSector.ColumnIndex);
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient && MySector != null)
		{
			base.photonView.RPC("ObjectChangeSector", player, MySector.RowIndex, MySector.ColumnIndex);
		}
	}

	[PunRPC]
	protected void ObjectChangeSector(byte row, byte column)
	{
		PhotonSectorInfo sector = PhotonSectorService.I.GetSector(row, column);
		if (sector != null)
		{
			OldSector = MySector;
			MySector = sector;
		}
		OnSectorChanges();
	}
}
