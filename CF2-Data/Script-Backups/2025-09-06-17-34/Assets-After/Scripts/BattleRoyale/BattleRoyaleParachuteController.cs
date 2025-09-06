using UnityEngine;

namespace BattleRoyale
{
	public class BattleRoyaleParachuteController : MonoBehaviour
	{
		public Transform Player;

		public Rigidbody RigidPlayer;

		private void FixedUpdate()
		{
			if (ControlFreak2.CF2Input.GetKey(KeyCode.W))
			{
				RigidPlayer.AddForce(Player.transform.forward * 13f, ForceMode.Force);
			}
		}
	}
}
