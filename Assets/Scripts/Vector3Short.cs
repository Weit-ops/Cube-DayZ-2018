using UnityEngine;

public class Vector3Short
{
	public short X;

	public short Y;

	public short Z;

	public Vector3Short()
	{
	}

	public Vector3Short(short x, short y, short z)
	{
		X = x;
		Y = y;
		Z = z;
	}

	public static Vector3 GetVector3(Vector3Short shortVector3, float decreaseValueFactor)
	{
		return new Vector3((float)shortVector3.X / decreaseValueFactor, (float)shortVector3.Y / decreaseValueFactor, (float)shortVector3.Z / decreaseValueFactor);
	}
}
