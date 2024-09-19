using UnityEngine;

public class MapIconPlayerDirection : MonoBehaviour
{
	private void Update()
	{
		if ((bool)WorldController.I && (bool)WorldController.I.Player)
		{
			float y = WorldController.I.Player.transform.eulerAngles.y;
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles.z = y;
			base.transform.eulerAngles = -eulerAngles;
		}
	}
}
