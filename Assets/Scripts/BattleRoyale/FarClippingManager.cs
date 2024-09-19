using UnityEngine;

namespace BattleRoyale
{
	public class FarClippingManager : MonoBehaviour
	{
		public Camera mainCam;

		public static FarClippingManager I;

		private void Awake()
		{
			if (I == null)
			{
				I = this;
			}
			if (DataKeeper.GameType == GameType.SkyWars)
			{
				SetupFarClippingMainCam(300f);
			}
			if (JsSpeeker.I.ReskinType != 0)
			{
				SetupFarClippingMainCam(240f);
			}
		}

		public void SetupFarClippingMainCam(float range)
		{
			mainCam.farClipPlane = range;
		}
	}
}
