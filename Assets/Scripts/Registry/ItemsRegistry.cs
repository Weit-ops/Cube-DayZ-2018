using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeyValueTexture
{
	public string Key;
	public Texture Value;
}

[Serializable]
public class KeyValueInt
{
	public string Key;
	public int Value;
}

public class ItemsRegistry : MonoBehaviour
{
	public static ItemsRegistry I;

	[SerializeField] List<KeyValueTexture> _clothingTextures;
	[SerializeField] List<KeyValueInt> _weaponsIdOrder;
	[SerializeField] int _hungerStepTime = 30;
	[SerializeField] int _thirstStepTime = 20;
	[SerializeField] int _hallucinationTime = 60;
	[SerializeField] LayerMask _itemsCollisionsMask;
	[SerializeField] Texture _defaultClothingTex;

	public int HungerStepTime
	{
		get
		{
			return _hungerStepTime;
		}
	}

	public int ThirstStepTime
	{
		get
		{
			return _thirstStepTime;
		}
	}

	public int HallucinationTime
	{
		get
		{
			return _hallucinationTime;
		}
	}

	public LayerMask ItemsCollisionsMask
	{
		get
		{
			return _itemsCollisionsMask;
		}
	}

	private void Awake()
	{
		I = this;
	}

	public void SetClothingTexture(Texture texture, List<string> ids)
	{
		List<KeyValueTexture> list = _clothingTextures.FindAll((KeyValueTexture tex) => ids.Contains(tex.Key));

		if (list.Count <= 0)
		{
			return;
		}

		string id;

		foreach (string id2 in ids)
		{
			id = id2;
			KeyValueTexture keyValueTexture = _clothingTextures.Find((KeyValueTexture tex) => tex.Key == id);

			if (keyValueTexture != null)
			{
				keyValueTexture.Value = texture;
			}
		}

		/*if (!(WorldController.I != null))
		{
			return;
		}
		foreach (PhotonMob worldMob in WorldController.I.WorldMobs)
		{
			if (worldMob != null && worldMob.MobIsActive)
			{
				ZombiesCustomizer customizer = worldMob.Customizer;
				if (customizer != null)
				{
					customizer.OnTextureLoad(ids);
				}
			}
		}
		foreach (KeyValuePair<string, PhotonMan> worldPlayer in WorldController.I.WorldPlayers)
		{
			if (worldPlayer.Value != null)
			{
				worldPlayer.Value.OnTextureLoad(ids);
			}
		}*/
	}

	public int GetWeaponIndex(string id)
	{
		KeyValueInt keyValueInt = _weaponsIdOrder.Find((KeyValueInt weaponInfo) => weaponInfo.Key == id);

		if (keyValueInt != null)
		{
			return keyValueInt.Value;
		}

		return 0;
	}

	public Texture GetClothingTexture(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}

		KeyValueTexture keyValueTexture = _clothingTextures.Find((KeyValueTexture texture) => texture.Key == id);
		return (keyValueTexture == null) ? _defaultClothingTex : keyValueTexture.Value;
	}

	public string GetClothingTextureId(Texture texture)
	{
		KeyValueTexture keyValueTexture = _clothingTextures.Find((KeyValueTexture tex) => tex.Value == texture);

		if (keyValueTexture != null)
		{
			return keyValueTexture.Key;
		}

		return null;
	}
}
