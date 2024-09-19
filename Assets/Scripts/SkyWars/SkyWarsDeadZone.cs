using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace SkyWars
{
	public class SkyWarsDeadZone : MonoBehaviour
	{
		private void OnTriggerEnter(Collider collider)
		{
			if (collider.name.Contains("Player") && WorldController.I.Player.photonView.isMine)
			{
				ObscuredShort damage = (short)1000;
				if (collider.GetComponent<PhotonMan> ()) {
					collider.GetComponent<PhotonMan> ().HitPlayer (damage, (byte)1, "fogdamage");
				}
			}
		}
	}
}
