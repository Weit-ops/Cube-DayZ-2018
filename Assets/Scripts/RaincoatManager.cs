using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaincoatManager : MonoBehaviour
{
	[SerializeField]
	private GameObject _rainCoat;

	[SerializeField]
	private GameObject _rainCoatWithBackpack;

	[SerializeField]
	private GameObject _backpackHolder;

	private bool _hasBackPack;

	[SerializeField]
	private List<Texture2D> _rainCoatsTextures;

	public GameObject player;

	private void OnEnable()
	{
		if (_rainCoat != null && _backpackHolder != null && _rainCoatWithBackpack != null)
		{
			if (_backpackHolder.transform.childCount > 1)
			{
				EnableRaincoatBackpack ();
			} else {
				EnableRaincoatOnly ();
			}
		}
	}



	public void EnableRaincoatOnly()
	{
		_rainCoatWithBackpack.SetActive(false);
		_rainCoat.SetActive(true);
		SetTexture(_rainCoat);
	}

	public void EnableRaincoatBackpack()
	{
		_rainCoat.SetActive(false);
		_rainCoatWithBackpack.SetActive(true);
		SetTexture(_rainCoatWithBackpack);
	}

	public void SetTexture(GameObject go)
	{
		PhotonView component = player.GetComponent<PhotonView>();
		if (component != null && component.instantiationData.Length > 3)
		{
			byte index = (byte)component.instantiationData[4];
			Renderer component2 = go.GetComponent<Renderer>();
			if (component2 != null)
			{
				component2.material.mainTexture = _rainCoatsTextures[index];
			}

		}
	}
}
