using UnityEngine;

public class RagdollViewController : MonoBehaviour
{
	[SerializeField] Renderer _character;

	public void SetView(Material material)
	{
		_character.material.CopyPropertiesFromMaterial(material);
	}
}
