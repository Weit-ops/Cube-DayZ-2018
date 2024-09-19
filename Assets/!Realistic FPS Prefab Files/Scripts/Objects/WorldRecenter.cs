//WorldRecenter.cs by Azuline Studios© All Rights Reserved
//Orients all game objects to scene origin if player travels 
//beyond threshold to correct floating point precision loss in large scenes.
using UnityEngine;
using System.Collections;

public class WorldRecenter : MonoBehaviour {
	private Object[] objects;
	public float threshold = 700.0f;//re-center objects if player moves farther than this distance from scene origin
	public bool refreshTerrain = true;//Refresh terrain to update tree colliders (can cause momentary hiccup on large terrains)
	[HideInInspector]
	public float worldRecenterTime = 0.0f;//most recent time of world recenter
//	True if the !!!FPS Main root object should be removed to allow the !!!FPS Player object 
//	to have synchronized local and world coordinates in order to prevent spatial jitter and larger game worlds.
//	It is suggested to keep removePrefabRoot as true, but if the prefab objects need to be children of a single
//	main object, removePrefabRoot may be false to keep the prefab objects as children of the !!!FPS Main object.
//	removePrefabRoot can be false for use in networking or other situations, but spatial jitter might
//	start to occur around 1000 or -1000 units from scene origin.
	public bool removePrefabRoot = true;

	void Start () {
		if(removePrefabRoot){
			GameObject prefabRoot = transform.parent.transform.gameObject;
			transform.parent.transform.DetachChildren();
			Destroy(prefabRoot);
		}
	}
}