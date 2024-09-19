using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GeoPainterPoint
{
	public GameObject go;
	public Vector3 pos;
	public Vector3 scale;
	public Vector3 normal;
	public bool useNormal;
}

[Serializable]
public class GeoPainterGroup : MonoBehaviour
{
	public GameObject[] myLibraryBuiltIn;
	public int rndSeed;
	[SerializeField]
	public List<GeoPainterPoint> myPointsList;
	public float offPosX;
	public float offPosY;
	public float offPosZ;
	public float rndPosMinX;
	public float rndPosMinY;
	public float rndPosMinZ;
	public float rndPosMaxX;
	public float rndPosMaxY;
	public float rndPosMaxZ;
	public float offRotX;
	public float offRotY;
	public float offRotZ;
	public float rndRotMinX;
	public float rndRotMinY;
	public float rndRotMinZ;
	public float rndRotMaxX;
	public float rndRotMaxY;
	public float rndRotMaxZ;
	public bool scaleUniform;
	public float offSclX;
	public float offSclY;
	public float offSclZ;
	public float rndSclMinX;
	public float rndSclMinY;
	public float rndSclMinZ;
	public float rndSclMaxX;
	public float rndSclMaxY;
	public float rndSclMaxZ;

	public GeoPainterGroup()
	{
		rndSeed = 1;
		myPointsList = new List<GeoPainterPoint>();
	}

	public virtual void addObject(GameObject _go, Vector3 _pos, Vector3 _scale, Vector3 _normal, bool _useNormal)
	{
		GeoPainterPoint geoPainterPoint = new GeoPainterPoint();
		geoPainterPoint.go = _go;
		geoPainterPoint.pos = _pos;
		geoPainterPoint.scale = _scale;
		geoPainterPoint.normal = _normal;
		geoPainterPoint.useNormal = _useNormal;
		myPointsList.Add(geoPainterPoint);
	}
}
