using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BundleTextureInfo
{
	public string BundleName;
	public string TextureName;
	public Material Material;
	public List<string> ItemsRegistryIds;
}

public class BundleResourceManager : MonoBehaviour
{
	public static BundleResourceManager I;

	[SerializeField]
	private List<BundleTextureInfo> _staticTexturesInfo;

	private void Start()
	{
		I = this;

		foreach(BundleTextureInfo item in _staticTexturesInfo)
		{
			item.Material.mainTexture = ItemsRegistry.I.GetClothingTexture (item.ItemsRegistryIds [0]);
		}
	}

	public void OnAssetBundleDownloaded(string bundleName)
	{
		/*#if UNITY_EDITOR
		List<BundleTextureInfo> list = _staticTexturesInfo.FindAll((BundleTextureInfo info) => info.BundleName == bundleName);

		if (list.Count <= 0)
		{
			return;
		}

		foreach (BundleTextureInfo item in list)
		{
			AssetBundle bundle = AssetBundlesService.I.GetBundle(bundleName);
			Texture texture = null;

			if (bundle != null)
			{
				texture = bundle.LoadAsset<Texture>(item.TextureName);
			}

			if (texture != null)
			{
				if (item.Material != null)
				{
					item.Material.mainTexture = texture;
				}

				if (item.ItemsRegistryIds != null && item.ItemsRegistryIds.Count > 0)
				{
					ItemsRegistry.I.SetClothingTexture(texture, item.ItemsRegistryIds);
				}
			}
		}
		#endif*/
	}
}
