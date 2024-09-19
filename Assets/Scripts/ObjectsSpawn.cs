using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ParseUtils
{
	public static Vector2 Vector2FromString(string str)
	{
		if (str == null)
		{
			Debug.LogWarning("Vector2 parse failed because string is null, setting default value");
			return new Vector2(66f, 66f);
		}
		string[] array = str.Substring(1, str.Length - 2).Split(',');
		float x = float.Parse(array[0], CultureInfo.InvariantCulture);
		float y = float.Parse(array[1], CultureInfo.InvariantCulture);
		return new Vector2(x, y);
	}

	public static Vector3 Vector3FromString(string str)
	{
		if (str == null)
		{
			Debug.LogWarning("Vector3 parse failed because string is null, setting default value");
			return new Vector3(66f, 66f, 66f);
		}
		string[] array = str.Substring(1, str.Length - 2).Split(',');
		float x = float.Parse(array[0], CultureInfo.InvariantCulture);
		float y = float.Parse(array[1], CultureInfo.InvariantCulture);
		float z = float.Parse(array[2], CultureInfo.InvariantCulture);
		return new Vector3(x, y, z);
	}

	public static Vector4 Vector4FromString(string str)
	{
		if (str == null)
		{
			Debug.LogWarning("Vector4 parse failed because string is null, setting default value");
			return new Vector4(66f, 66f, 66f, 66f);
		}
		string[] array = str.Substring(1, str.Length - 2).Split(',');
		float x = float.Parse(array[0], CultureInfo.InvariantCulture);
		float y = float.Parse(array[1], CultureInfo.InvariantCulture);
		float z = float.Parse(array[2], CultureInfo.InvariantCulture);
		float w = float.Parse(array[3], CultureInfo.InvariantCulture);
		return new Vector4(x, y, z, w);
	}
}


public class PhotonSpawnMobInfo
{
	public string Id;
	public string Prefab;
	public Vector3 Position;
}

public enum SpawnType
{
	Object = 0,
	Mob = 1
}

public class ObjectsSpawn
{
	public string Position;
	public string SpawnArea;
	public List<string> SpawnIds;
	public List<int> SpawnIdsChance;
	public List<VariableValue> CountForItems;
	public int MinCount;
	public int MaxCount;
	public bool SpawnDifferentObjects;
	public bool CanSpawnSameObjects;

	public SpawnType SpawnType;

	public List<PhotonSpawnMobInfo> GetMobSpawnInfo(LayerMask mobSpawnLayerMask)
	{
		List<PhotonSpawnMobInfo> list = new List<PhotonSpawnMobInfo>();
		ContentInfo info = WorldController.I.Info;
		int num = UnityEngine.Random.Range(MinCount, MaxCount + 1);
		Vector3 vector = ParseUtils.Vector3FromString(Position);
		int objIdIndex = GetObjIdIndex();
		Vector3 vector2 = ParseUtils.Vector3FromString(SpawnArea);
		for (int i = 0; i < num; i++)
		{
			if (objIdIndex < 0)
			{
				break;
			}
			string objId = SpawnIds[objIdIndex];
			Vector3 origin = new Vector3(vector.x + vector2.x / 2f * UnityEngine.Random.Range(-1f, 1f), vector.y + vector2.y / 2f, vector.z + vector2.z / 2f * UnityEngine.Random.Range(-1f, 1f));
			RaycastHit hitInfo;
			if (Physics.Raycast(origin, -Vector3.up, out hitInfo, 150f, mobSpawnLayerMask))
			{
				SpawnType spawnType = SpawnType;
				if (spawnType == SpawnType.Mob)
				{
					Mob mob = info.Mobs.Find((Mob m) => m.Id == objId);
					if (mob != null)
					{
						list.Add(new PhotonSpawnMobInfo
						{
							Id = mob.Id,
							Prefab = mob.Prefab,
							Position = hitInfo.point
						});
					}
				}
			}
			if (SpawnDifferentObjects)
			{
				if (!CanSpawnSameObjects)
				{
					SpawnIds.RemoveAt(objIdIndex);
					SpawnIdsChance.RemoveAt(objIdIndex);
				}
				objIdIndex = GetObjIdIndex();
			}
		}
		return list;
	}

	public void SpawnObjects(ContentInfo info, bool local = false)
	{
		float num = (float)UnityEngine.Random.Range(MinCount, MaxCount + 1) * DataKeeper.LocalLootFactor;
		Vector3 vector = ParseUtils.Vector3FromString(Position);
		int objIdIndex = GetObjIdIndex();
		Vector3 vector2 = ParseUtils.Vector3FromString(SpawnArea);
		int num2 = 0;
		for (int i = 0; (float)i < num; i++)
		{
			if (objIdIndex < 0)
			{
				break;
			}
			string objId = SpawnIds[objIdIndex];
			Vector3 origin = new Vector3(vector.x + vector2.x / 2f * UnityEngine.Random.Range(-1f, 1f), vector.y + vector2.y / 2f, vector.z + vector2.z / 2f * UnityEngine.Random.Range(-1f, 1f));
			Ray ray = new Ray(origin, -Vector3.up);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 150f))
			{
				switch (SpawnType)
				{
				case SpawnType.Object:
				{
					Quaternion rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, hitInfo.normal), Vector3.Cross(Vector3.up, hitInfo.normal));
					int min = 1;
					int max = 10;
					if (DataKeeper.GameType == GameType.BattleRoyale)
					{
						min = 7;
						max = 30;
					}
					int num3 = CountForItems[objIdIndex].GetValueInt();
					if (DataKeeper.GameType == GameType.BattleRoyale && WorldController.I.Info.GetWeaponInfo(objId) == null && num2 < 1)
					{
						num2++;
						float num4 = UnityEngine.Random.Range(0f, 1f);
						if (num4 < DataKeeper.BR_MoreWeaponsChancePerSpawnZone)
						{
							int count = WorldController.I.Info.Weapons.Count;
							int index = UnityEngine.Random.Range(0, count);
							objId = WorldController.I.Info.Weapons[index].Id;
							num3 = 1;
						}
					}
					int num5 = ((WorldController.I.Info.GetWeaponInfo(objId) != null) ? UnityEngine.Random.Range(min, max) : 0);
					if (local)
					{
						if (DataKeeper.GameType == GameType.BattleRoyale && JsSpeeker.I.ReskinType != 0)
						{
							if (WorldController.I.Info.GetItemInfo(objId).Type == ItemType.Clothing)
							{
								int count2 = WorldController.I.Info.Weapons.Count;
								int index2 = UnityEngine.Random.Range(0, count2);
								objId = WorldController.I.Info.Weapons[index2].Id;
								num3 = 1;
								WorldController.I.InstantiateLocalItem(objId, hitInfo.point, rotation, (byte)num3, 15);
							}
							else
							{
								WorldController.I.InstantiateLocalItem(objId, hitInfo.point, rotation, (byte)num3, (byte)num5);
							}
						}
						else
						{
							WorldController.I.InstantiateLocalItem(objId, hitInfo.point, rotation, (byte)num3, (byte)num5);
						}
					}
					else
					{
						PhotonNetwork.InstantiateSceneObject("PhotonItem", hitInfo.point, rotation, 0, new object[4]
						{
							objId,
							(byte)CountForItems[objIdIndex].GetValueInt(),
							false,
							(byte)num5
						});
					}
					break;
				}
				case SpawnType.Mob:
					if (!local)
					{
						Mob mob = info.Mobs.Find((Mob m) => m.Id == objId);
						if (mob != null)
						{
							PhotonNetwork.InstantiateSceneObject("Mobs/" + mob.Prefab, Vector3.zero, Quaternion.identity, 0, new object[4]
							{
								mob.Id,
								(short)(hitInfo.point.x * 10f),
								(short)(hitInfo.point.y * 10f),
								(short)(hitInfo.point.z * 10f)
							});
						}
					}
					break;
				}
			}
			if (SpawnDifferentObjects)
			{
				if (!CanSpawnSameObjects)
				{
					SpawnIds.RemoveAt(objIdIndex);
					SpawnIdsChance.RemoveAt(objIdIndex);
				}
				objIdIndex = GetObjIdIndex();
			}
		}
	}

	private int GetObjIdIndex()
	{
		if (SpawnIds.Count == 0)
		{
			return -1;
		}
		int result = SpawnIds.Count - 1;
		int max = SpawnIdsChance.Sum();
		int num = UnityEngine.Random.Range(0, max);
		int num2 = 0;
		for (int i = 0; i < SpawnIdsChance.Count; i++)
		{
			num2 += SpawnIdsChance[i];
			if (num < num2)
			{
				return i;
			}
		}
		return result;
	}
}
