using System.Collections.Generic;
using UnityEngine;

public class CatCustomizer : MonoBehaviour
{
	[SerializeField]
	private Renderer _cat;

	[SerializeField]
	private List<Texture> _catTextures;

	private void Awake()
	{
		if ((bool)_cat)
		{
			_cat.material.mainTexture = _catTextures[UnityEngine.Random.Range(0, _catTextures.Count)];
		}
	}
}
