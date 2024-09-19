using UnityEngine;

public class TutorialLocalItemSpawn : MonoBehaviour
{
	[SerializeField]
	private string _itemId;

	[SerializeField]
	private byte _count;

	[SerializeField]
	private byte _ammo;

	private void Start()
	{
		if (!string.IsNullOrEmpty(_itemId) && _count > 0)
		{
			WorldController.I.InstantiateLocalItem(_itemId, base.transform.position, Quaternion.identity, _count, _ammo);
		}
	}
}
