using UnityEngine;

public class WorldObjectActivateAction : BaseWorldObjectAction
{
	private PhotonWorldObject _photonObject;

	private bool _isEnabled;

	public override bool CanUse
	{
		get
		{
			return _photonObject.ObjInfo.OwnerUId == 0 || _photonObject.ObjInfo.OwnerUId == DataKeeper.BackendInfo.user.playerId;
		}
	}

	private void Awake()
	{
		_photonObject = GetComponent<PhotonWorldObject>();
	}

	public override void Use()
	{
		base.photonView.RPC("Activate", PhotonTargets.All, !_isEnabled);
	}

	private void OnPhotonPlayerConnected()
	{
		if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("Activate", PhotonTargets.All, _isEnabled);
		}
	}

	[PunRPC]
	private void Activate(bool activate)
	{
		if (_isEnabled != activate)
		{
			_isEnabled = activate;
			if (_photonObject.NormalModel.GetComponent<Animation>() != null)
			{
				string animation = ((!_isEnabled) ? "Disable" : "Enable");
				if (name == "PlaceWoodenDoor(Clone)" || name == "PlaceJailDoor(Clone)") 
				{
					animation = ((!_isEnabled) ? "Disable_0" : "Enable_0");
				}
				_photonObject.NormalModel.GetComponent<Animation>().Play(animation);
			}
		}
	}
}
