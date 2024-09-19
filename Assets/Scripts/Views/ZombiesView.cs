using UnityEngine;

public class ZombiesView : MonoBehaviour
{
	[SerializeField] Color _skinColor;
	[SerializeField] string HeadTexId;
	[SerializeField] string BodyTexId;
	[SerializeField] string LegsTexId;
	[Range(0f, 2f)]
	[SerializeField] int _bloodMaskIndex;

	private void Start()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			renderer.material.SetInt("_MaskIndex", _bloodMaskIndex);
			renderer.material.SetColor("_SkinColor", _skinColor);
			renderer.material.SetTexture("_HeadTex", ItemsRegistry.I.GetClothingTexture(HeadTexId));
			renderer.material.SetTexture("_BodyTex", ItemsRegistry.I.GetClothingTexture(BodyTexId));
			renderer.material.SetTexture("_LegsTex", ItemsRegistry.I.GetClothingTexture(LegsTexId));
		}
	}
}
