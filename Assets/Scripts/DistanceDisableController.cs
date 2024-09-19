using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceDisableController : MonoBehaviour
{
	[SerializeField]
	private float _distance;

	[SerializeField]
	private float _checkTime;

	[SerializeField]
	private List<GameObject> _objects;

	private List<Transform> _transformsCache = new List<Transform>();

	private void Awake()
	{
		foreach (GameObject @object in _objects)
		{
			if (@object != null)
			{
				_transformsCache.Add(@object.transform);
			}
		}
		StartCoroutine(DisableProcess());
	}

	private IEnumerator DisableProcess()
	{
		if (WorldController.I != null && WorldController.I.Player != null)
		{
			for (int i = 0; i < _objects.Count; i++)
			{
				if (_objects[i] != null)
				{
					bool enable = Vector3.Distance(_transformsCache[i].position, WorldController.I.Player.transform.position) < _distance;
					if (_objects[i].activeSelf != enable)
					{
						_objects[i].SetActive(enable);
					}
				}
			}
		}
		yield return new WaitForSeconds(_checkTime);
		StartCoroutine(DisableProcess());
	}
}
