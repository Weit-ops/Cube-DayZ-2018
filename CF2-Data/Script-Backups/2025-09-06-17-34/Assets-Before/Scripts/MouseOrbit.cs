using UnityEngine;

public class MouseOrbit : MonoBehaviour
{
	public Transform Target;
	public float Distance = 5f;
	public float xSpeed = 250f;
	public float ySpeed = 120f;
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	private float x;
	private float y;
	public float zoomMin = 1f;
	public float zoomMax = 20f;
	public float scrollSpeed = 10f;
	public float distance;

	private void Awake()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.x;
		y = eulerAngles.y;
	}

	private void LateUpdate()
	{
		if (Target != null)
		{
			x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			y = ClampAngle(y, yMinLimit, yMaxLimit);
			Quaternion quaternion = Quaternion.Euler(y, x, 0f);
			Vector3 position = quaternion * new Vector3(0f, 0f, 0f - Distance) + Target.position;
			base.transform.rotation = quaternion;
			base.transform.position = position;
			if (Input.GetAxis("Mouse ScrollWheel") != 0f)
			{
				distance = Vector3.Distance(base.transform.position, Target.position);
				distance = ZoomLimit(distance - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, zoomMin, zoomMax);
				position = -(base.transform.forward * distance) + Target.position;
				base.transform.position = position;
			}
		}
	}

	private float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public static float ZoomLimit(float dist, float min, float max)
	{
		if (dist < min)
		{
			dist = min;
		}
		if (dist > max)
		{
			dist = max;
		}
		return dist;
	}
}
