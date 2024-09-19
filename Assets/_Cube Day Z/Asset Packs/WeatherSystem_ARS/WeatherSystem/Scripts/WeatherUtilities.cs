using UnityEngine;
using System.Collections;

public class WeatherUtilities
{
    static public Vector3 PolarToCartCoords(float speed, float degrees)
    {
        Vector3 cart = Vector3.zero;
        cart.x = speed / 100.0f * Mathf.Cos(degrees * Mathf.PI / 180);
        cart.y = speed / 100.0f * Mathf.Sin(degrees * Mathf.PI / 180);
        cart.z = speed / 100.0f * Mathf.Sin(degrees * Mathf.PI / 180);
        return cart;
    }

    public static Vector2 RandomOnUnitCircle2(float radius)
    {
        Vector2 randomPointOnCircle = Random.insideUnitCircle;
        randomPointOnCircle.Normalize();
        randomPointOnCircle *= radius;
        return randomPointOnCircle;
    }
    public static Vector3 RandomOnUnitSphere(float radius)
    {
        Vector3 randomPointOnCircle = Random.insideUnitSphere;
        randomPointOnCircle.Normalize();
        randomPointOnCircle *= radius;
        return randomPointOnCircle;
    }

    public static Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = Random.Range(0.0f, Mathf.PI / 2);// Random.value * 360;
        Vector3 pos = Vector3.zero;
        pos.x = center.x + radius * Mathf.Sin(ang);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(ang);
        return pos;
    }
}

public class UnitSphere
{
    /// <summary>
    /// Returns a point on the unit sphere that is within a cone along the y-axis
    /// </summary>
    /// <param name="spotAngle">[0..180] specifies the angle of the cone. </param>
    public static Vector3 GetPointOnCapY(float spotAngle)
    {
        float angle1 = Random.Range(0.0f, Mathf.PI * 2);
        float angle2 = Random.Range(0.0f, spotAngle * Mathf.Deg2Rad);
        Vector3 V = new Vector3(Mathf.Sin(angle1), 0, Mathf.Cos(angle1));
        V *= Mathf.Sin(angle2);
        V.y = Mathf.Cos(angle2);
        return V;
    }

    public static Vector3 GetPointOnCapY(float spotAngle, Quaternion orientation)
    {
        return orientation * GetPointOnCapY(spotAngle);
    }

    public static Vector3 GetPointOnCapY(float spotAngle, Transform relativeTo, float radius)
    {
        return relativeTo.TransformPoint(GetPointOnCapY(spotAngle) * radius);
    }

    /// <summary>
    /// Returns a point on the unit sphere that is within a cone along the z-axis
    /// </summary>
    /// <param name="spotAngle">[0..180] specifies the angle of the cone. </param>
    public static Vector3 GetPointOnCapZ(float spotAngle)
    {
        float angle1 = Random.Range(0.0f, Mathf.PI * 2);
        float angle2 = Random.Range(0.0f, spotAngle * Mathf.Deg2Rad);
        Vector3 V = new Vector3(Mathf.Sin(angle1), Mathf.Cos(angle1), 0);
        V *= Mathf.Sin(angle2);
        V.z = Mathf.Cos(angle2);
        return V;
    }

    public static Vector3 GetPointOnCapZ(float spotAngle, Quaternion orientation)
    {
        return orientation * GetPointOnCapZ(spotAngle);
    }

    public static Vector3 GetPointOnCapZ(float spotAngle, Transform relativeTo, float radius)
    {
        return relativeTo.TransformPoint(GetPointOnCapZ(spotAngle) * radius);
    }


    /// <summary>
    /// Returns a point on the unit sphere that is within the outer cone along the y-axis
    /// but not inside the inner cone. The resulting area describes a ring on the sphere surface.
    /// </summary>
    /// <param name="innerSpotAngle">[0..180] specifies the inner cone that should be excluded.</param>
    /// <param name="outerSpotAngle">[0..180] specifies the outer cone that should be included.</param>
    public static Vector3 GetPointOnRingY(float innerSpotAngle, float outerSpotAngle)
    {
        float angle1 = Random.Range(0.0f, Mathf.PI * 2);
        float angle2 = Random.Range(innerSpotAngle, outerSpotAngle) * Mathf.Deg2Rad;
        Vector3 V = new Vector3(Mathf.Sin(angle1), 0, Mathf.Cos(angle1));
        V *= Mathf.Sin(angle2);
        V.y = Mathf.Cos(angle2);
        return V;
    }

    public static Vector3 GetPointOnRingY(float innerSpotAngle, float outerSpotAngle, Quaternion orientation)
    {
        return orientation * GetPointOnRingY(innerSpotAngle, outerSpotAngle);
    }

    public static Vector3 GetPointOnRingY(float innerSpotAngle, float outerSpotAngle, Transform relativeTo, float radius)
    {
        return relativeTo.TransformPoint(GetPointOnRingY(innerSpotAngle, outerSpotAngle) * radius);
    }

    /// <summary>
    /// Returns a point on the unit sphere that is within the outer cone along the z-axis
    /// but not inside the inner cone. The resulting area describes a ring on the sphere surface.
    /// </summary>
    /// <param name="innerSpotAngle">[0..180] specifies the inner cone that should be excluded.</param>
    /// <param name="outerSpotAngle">[0..180] specifies the outer cone that should be included.</param>
    public static Vector3 GetPointOnRingZ(float innerSpotAngle, float outerSpotAngle)
    {
        float angle1 = Random.Range(0.0f, Mathf.PI * 2);
        float angle2 = Random.Range(innerSpotAngle, outerSpotAngle) * Mathf.Deg2Rad;
        Vector3 V = new Vector3(Mathf.Sin(angle1), Mathf.Cos(angle1), 0);
        V *= Mathf.Sin(angle2);
        V.z = Mathf.Cos(angle2);
        return V;
    }

    public static Vector3 GetPointOnRingZ(float innerSpotAngle, float outerSpotAngle, Quaternion orientation)
    {
        return orientation * GetPointOnRingZ(innerSpotAngle, outerSpotAngle);
    }

    public static Vector3 GetPointOnRingZ(float innerSpotAngle, float outerSpotAngle, Transform relativeTo, float radius)
    {
        return relativeTo.TransformPoint(GetPointOnRingZ(innerSpotAngle, outerSpotAngle) * radius);
    }
}