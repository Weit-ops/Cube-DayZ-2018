using System.Collections.Generic;
using JsonFx.Json;
using UnityEngine;

public class PlayerViewSaveInfo
{
	public byte Sex;
	public byte FaceIndex;
	public byte SkinColorIndex;
	public byte PremiumPackIndex;
	public byte VipRainCoat;
}

public class CustomizationRegistry : MonoBehaviour
{
	[SerializeField] List<Color> _skinsColors;
	[SerializeField] Texture _maleDefaultSkin;
	[SerializeField] Texture _femaleDefaultSkin;
	[SerializeField] List<string> _maleTexIds;
	[SerializeField] List<string> _femaleTexIds;
	[SerializeField] List<PremiumClothingPack> _premiumClothingsPackId;
	[SerializeField] List<int> _premiumSkinIndeces;
	[SerializeField] List<int> _premiumMaleFaceIndeces;
	[SerializeField] List<int> _premiumFemaleFaceIndeces;

	public static CustomizationRegistry Instance { get; private set; }

	public List<Color> SkinColors
	{
		get
		{
			return _skinsColors;
		}
	}

	public Texture MaleDefaultSkin
	{
		get
		{
			return _maleDefaultSkin;
		}
	}

	public Texture FemaleDefaultSkin
	{
		get
		{
			return _femaleDefaultSkin;
		}
	}

	public List<string> MaleFaces
	{
		get
		{
			return _maleTexIds;
		}
	}

	public List<string> FemaleFaces
	{
		get
		{
			return _femaleTexIds;
		}
	}

	public List<PremiumClothingPack> PremiumClothings
	{
		get
		{
			return _premiumClothingsPackId;
		}
	}

	public List<int> PremiumSkinIndeces
	{
		get
		{
			return _premiumSkinIndeces;
		}
	}

	public List<int> PremiumMaleFaceIndeces
	{
		get
		{
			return _premiumMaleFaceIndeces;
		}
	}

	public List<int> PremiumFemaleFaceIndeces
	{
		get
		{
			return _premiumFemaleFaceIndeces;
		}
	}

	private int RainCoatIndex { get; set; }

	public int GetRainCoatIndex()
	{
		if (RainCoatIndex < 0)
		{
			RainCoatIndex = 0;
		}
		return RainCoatIndex;
	}

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		if (!PlayerPrefs.HasKey("PlayerViewInfo"))
		{
			return;
		}
		string @string = PlayerPrefs.GetString("PlayerViewInfo");
		if (!string.IsNullOrEmpty(@string))
		{
			PlayerViewSaveInfo playerViewSaveInfo = JsonReader.Deserialize<PlayerViewSaveInfo>(PlayerPrefs.GetString("PlayerViewInfo"));
			if (playerViewSaveInfo != null)
			{
				DataKeeper.Sex = (Sex)playerViewSaveInfo.Sex;
				DataKeeper.FaceIndex = playerViewSaveInfo.FaceIndex;
				DataKeeper.SkinColorIndex = playerViewSaveInfo.SkinColorIndex;
				DataKeeper.PremiumPackIndex = playerViewSaveInfo.PremiumPackIndex;
				DataKeeper.VipRaincoatIndex = playerViewSaveInfo.VipRainCoat;
			}
		}
		SetRaincoatIndexForGame(DataKeeper.VipRaincoatIndex);
	}

	public Texture GetDefaultTexture(Sex sex)
	{
		if (sex == Sex.Female)
		{
			return _femaleDefaultSkin;
		}
		return _maleDefaultSkin;
	}

	public string GetFaceTextureId(Sex sex, byte index)
	{
		List<string> list = ((sex != Sex.Female) ? _maleTexIds : _femaleTexIds);
		int index2 = Mathf.Max(0, Mathf.Min(index, list.Count - 1));
		return list[index2];
	}

	public Color GetColor(byte index)
	{
		int index2 = Mathf.Max(0, Mathf.Min(index, _skinsColors.Count - 1));
		return _skinsColors[index2];
	}

	public void SetRaincoatIndexForGame(int index)
	{
		RainCoatIndex = index;
	}
}
