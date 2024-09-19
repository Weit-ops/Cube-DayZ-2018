using UnityEngine;

public class EffectsController : MonoBehaviour
{
	[SerializeField] float _lifetime;

	private void Awake()
	{
		Object.Destroy(base.gameObject, _lifetime);
	}
}
