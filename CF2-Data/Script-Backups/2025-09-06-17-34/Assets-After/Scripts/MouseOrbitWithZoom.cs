using UnityEngine;

public class MouseOrbitWithZoom : MonoBehaviour
{
	public Transform target;
	public float distance = 5f;
	public float xSpeed = 120f;
	public float ySpeed = 120f;
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	public float distanceMin = 0.5f;
	public float distanceMax = 15f;
	private Rigidbody rb;
	private float x;
	private float y;
	public bool isEnable = true;

	public static MouseOrbitWithZoom I;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	private void Start()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		x = eulerAngles.y;
		y = eulerAngles.x;
		rb = GetComponent<Rigidbody>();
		if (rb != null)
		{
			rb.freezeRotation = true;
		}
	}

	private void LateUpdate()
	{
		if ((bool)target)
		{
			if (isEnable) {
				x += ControlFreak2.CF2Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
				y -= ControlFreak2.CF2Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

				distance = Mathf.Clamp(distance - ControlFreak2.CF2Input.GetAxis("Mouse ScrollWheel") * 5f, distanceMin, distanceMax);
			}

			y = ClampAngle(y, yMinLimit, yMaxLimit);
			Quaternion quaternion = Quaternion.Euler(y, x, 0f);
			Vector3 vector = new Vector3(0f, 0f, 0f - distance);
			Vector3 position = quaternion * vector + target.position;
			base.transform.rotation = quaternion;
			base.transform.position = position;
		}
	}

	public static float ClampAngle(float angle, float min, float max)
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

	public void SetTargetCar(Transform _target)
	{
		target = _target;
	}
}
