using Photon;
using UnityEngine;

public class TestObjectsSections : Photon.MonoBehaviour
{
	[SerializeField] WorldObjectSection _weaponSection;
	[SerializeField] WorldObjectSection _clothingSection;
	[SerializeField] WorldObjectSection _consumableSection;

	private bool _parentInitialized;

	private void SpawnObj(Item item, int index, WorldObjectSection section)
	{
		Vector3? section2 = section.GetSection(index);
		if (section2.HasValue)
		{
			Object @object = Resources.Load(WorldController.I.GetResourceLoadPath(item.Type) + item.Prefab);
			if ((bool)@object)
			{
				int num = ((item.Type == ItemType.Weapon) ? UnityEngine.Random.Range(0, 15) : 0);
				WorldController.I.InstantiateLocalItem(item.Id, section.transform.position + section2.Value, Quaternion.identity, (byte)UnityEngine.Random.Range(1, item.MaxInStack), (byte)num);
			}
			else
			{
				Debug.Log("PREFAB NULL: " + item.Prefab);
			}
		}
	}

	[PunRPC]
	private void InstantiateObjects()
	{
		if (!(WorldController.I == null) && WorldController.I.IsDone)
		{
			ContentInfo info = WorldController.I.Info;
			for (int i = 0; i < info.Weapons.Count; i++)
			{
				SpawnObj(info.Weapons[i], i, _weaponSection);
			}
			for (int j = 0; j < info.Clothings.Count; j++)
			{
				SpawnObj(info.Clothings[j], j, _clothingSection);
			}
			for (int k = 0; k < info.Consumables.Count; k++)
			{
				SpawnObj(info.Consumables[k], k, _consumableSection);
			}
		}
	}
}
