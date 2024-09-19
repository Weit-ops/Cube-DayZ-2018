using System.Collections.Generic;
using JsonFx.Json;
using UnityEngine;

public class CustomizationSkinController : MonoBehaviour
{
	[SerializeField] tk2dUILayoutContainerSizer _content;
	[SerializeField] tk2dUIToggleButtonGroup _skinsToggleGroup;
	[SerializeField] tk2dUILayout _skin;
	[SerializeField] Renderer _character;
	[SerializeField] GameObject _vipMessage;
	[SerializeField] GameObject _superVipMessage;
	[SerializeField] GameObject SexChangeObj;
	[SerializeField] tk2dUIToggleButtonGroup _viewTypes;
	[SerializeField] GameObject _mainMenu;

	private List<string> _currentFaces;
	private Sex _currentsSex;
	private int _currentFaceIndex;
	private int _currentPackIndex;
	private bool _isInitialized;

	[SerializeField] GameObject _rainCoats;
	[SerializeField] GameObject _characterRef;
	[SerializeField] List<Texture2D> _rainCoatsTextures;

	private int CurrentRainCoatIndex { get; set; }

	private void Start()
	{
		if (!_isInitialized)
		{
			Initialize();
		}
		CustomizationRegistry instance = CustomizationRegistry.Instance;
		if (instance.SkinColors.Count - 1 < DataKeeper.SkinColorIndex)
		{
			DataKeeper.SkinColorIndex = 0;
			_skinsToggleGroup.SelectedIndex = 0;
		}
		else
		{
			_skinsToggleGroup.SelectedIndex = DataKeeper.SkinColorIndex;
		}
		_currentsSex = DataKeeper.Sex;
		CurrentRainCoatIndex = DataKeeper.VipRaincoatIndex;
		if (CurrentRainCoatIndex >= _rainCoatsTextures.Count - 1 || CurrentRainCoatIndex < 0)
		{
			CurrentRainCoatIndex = 0;
		}
		SetCharacterSex();
		_currentFaces = ((_currentsSex != 0) ? instance.FemaleFaces : instance.MaleFaces);
		if (_currentFaces.Count - 1 < DataKeeper.FaceIndex)
		{
			_currentFaceIndex = 0;
			ChangeFace();
		}
		else
		{
			_currentFaceIndex = DataKeeper.FaceIndex;
			ChangeFace();
		}
		if (_currentPackIndex < CustomizationRegistry.Instance.PremiumClothings.Count)
		{
			_currentPackIndex = DataKeeper.PremiumPackIndex;
			ChangeClothings();
		}
		else
		{
			_currentPackIndex = 0;
			ChangeClothings();
		}
		if (!DataKeeper.BackendInfo.user.has_premium)
		{
			List<int> list = ((_currentsSex != 0) ? instance.PremiumFemaleFaceIndeces : instance.PremiumMaleFaceIndeces);
			if (instance.PremiumSkinIndeces.Contains(_skinsToggleGroup.SelectedIndex))
			{
				DataKeeper.SkinColorIndex = 0;
				_skinsToggleGroup.SelectedIndex = 0;
			}
			if (list.Contains(_currentFaceIndex))
			{
				DataKeeper.FaceIndex = 0;
				_currentFaceIndex = 0;
				ChangeFace();
			}
			Save();
		}
	}

	private void Initialize()
	{
		List<tk2dUIToggleButton> list = new List<tk2dUIToggleButton>();
		CustomizationRegistry instance = CustomizationRegistry.Instance;
		for (int i = 0; i < instance.SkinColors.Count; i++)
		{
			SkinColor component = AddSkinToContent(_skin).GetComponent<SkinColor>();
			component.SetColor(instance.SkinColors[i], !DataKeeper.BackendInfo.user.has_premium && instance.PremiumSkinIndeces.Contains(i));
			list.Add(component.Toggle);
		}
		_skinsToggleGroup.AddNewToggleButtons(list.ToArray());
		_content.Refresh();
		_isInitialized = true;
	}

	private tk2dUILayout AddSkinToContent(tk2dUILayout layout)
	{
		tk2dUILayout tk2dUILayout2 = Object.Instantiate(layout);
		if (tk2dUILayout2 != null)
		{
			_content.AddLayoutAtIndex(tk2dUILayout2, tk2dUILayoutItem.FixedSizeLayoutItem(), _content.layoutItems.Count);
			tk2dUILayout2.transform.localPosition = Vector3.zero;
		}
		return tk2dUILayout2;
	}

	private void SetCharacterSex()
	{
		Texture texture = ((_currentsSex != 0) ? CustomizationRegistry.Instance.FemaleDefaultSkin : CustomizationRegistry.Instance.MaleDefaultSkin);
		_character.material.SetTexture("_MainTex", texture);
	}

	private void SetCharacterSkinColor(Color color)
	{
		_character.material.SetColor("_SkinColor", color);
	}

	private void ChangeFace()
	{
		ShowPremiumInfoForFaces();
		_character.material.SetTexture("_HeadTex", ItemsRegistry.I.GetClothingTexture(_currentFaces[_currentFaceIndex]));
	}

	private void OnClickLeft()
	{
		switch (_viewTypes.SelectedIndex)
		{
		case 1:
		{
			List<PremiumClothingPack> premiumClothings = CustomizationRegistry.Instance.PremiumClothings;
			_currentPackIndex = (premiumClothings.Count + (_currentPackIndex - 1)) % premiumClothings.Count;
			ChangeClothings();
			break;
		}
		case 2:
			CurrentRainCoatIndex--;
			ChangeRainCoat(CurrentRainCoatIndex);
			break;
		default:
			_currentFaceIndex = (_currentFaces.Count + (_currentFaceIndex - 1)) % _currentFaces.Count;
			ChangeFace();
			break;
		}
	}

	private void OnClickRight()
	{
		switch (_viewTypes.SelectedIndex)
		{
		case 1:
			_currentPackIndex = (_currentPackIndex + 1) % CustomizationRegistry.Instance.PremiumClothings.Count;
			ChangeClothings();
			break;
		case 2:
			CurrentRainCoatIndex++;
			ChangeRainCoat(CurrentRainCoatIndex);
			break;
		default:
			_currentFaceIndex = (_currentFaceIndex + 1) % _currentFaces.Count;
			ChangeFace();
			break;
		}
	}

	private void ChangeClothings()
	{
		int selectedIndex = _viewTypes.SelectedIndex;
		if (selectedIndex == 1)
		{
			PremiumClothingPack premiumClothingPack = CustomizationRegistry.Instance.PremiumClothings[_currentPackIndex];
			Texture clothingTexture = ItemsRegistry.I.GetClothingTexture(premiumClothingPack.BodyClothingId);
			Texture clothingTexture2 = ItemsRegistry.I.GetClothingTexture(premiumClothingPack.LegsClothingId);
			_character.material.SetTexture("_BodyTex", clothingTexture);
			_character.material.SetTexture("_LegsTex", clothingTexture2);
		}
		else
		{
			_character.material.SetTexture("_BodyTex", null);
			_character.material.SetTexture("_LegsTex", null);
		}
	}

	private void OnClickMale()
	{
		_currentFaceIndex = 0;
		_currentsSex = Sex.Male;
		_currentFaces = CustomizationRegistry.Instance.MaleFaces;
		ChangeFace();
		SetCharacterSex();
	}

	private void OnClickFemale()
	{
		_currentFaceIndex = 0;
		_currentsSex = Sex.Female;
		_currentFaces = CustomizationRegistry.Instance.FemaleFaces;
		ChangeFace();
		SetCharacterSex();
	}

	private void Save()
	{
		CustomizationRegistry instance = CustomizationRegistry.Instance;
		DataKeeper.Sex = _currentsSex;
		if (CurrentRainCoatIndex == -1)
		{
			CurrentRainCoatIndex = 0;
		}
		DataKeeper.VipRaincoatIndex = (byte)CurrentRainCoatIndex;
		if (!instance.PremiumSkinIndeces.Contains(_skinsToggleGroup.SelectedIndex) || DataKeeper.BackendInfo.user.has_premium)
		{
			DataKeeper.SkinColorIndex = (byte)_skinsToggleGroup.SelectedIndex;
		}
		List<int> list = ((_currentsSex != 0) ? instance.PremiumFemaleFaceIndeces : instance.PremiumMaleFaceIndeces);
		instance.SetRaincoatIndexForGame(CurrentRainCoatIndex);
		if (!list.Contains(_currentFaceIndex) || DataKeeper.BackendInfo.user.has_premium)
		{
			DataKeeper.FaceIndex = (byte)_currentFaceIndex;
		}
		DataKeeper.PremiumPackIndex = (byte)_currentPackIndex;
		PlayerViewSaveInfo playerViewSaveInfo = new PlayerViewSaveInfo();
		playerViewSaveInfo.Sex = (byte)DataKeeper.Sex;
		playerViewSaveInfo.FaceIndex = DataKeeper.FaceIndex;
		playerViewSaveInfo.SkinColorIndex = DataKeeper.SkinColorIndex;
		playerViewSaveInfo.PremiumPackIndex = DataKeeper.PremiumPackIndex;
		playerViewSaveInfo.VipRainCoat = DataKeeper.VipRaincoatIndex;
		string value = JsonWriter.Serialize(playerViewSaveInfo);
		PHPNetwork.I.SaveView (playerViewSaveInfo);
	}

	private void OnClickSave()
	{
		Save();
		base.gameObject.SetActive(false);
		_mainMenu.SetActive(true);
	}

	private void OnSkinColorChange()
	{
		if (_skinsToggleGroup.SelectedIndex >= 0 && _skinsToggleGroup.SelectedIndex <= CustomizationRegistry.Instance.SkinColors.Count - 1)
		{
			SetCharacterSkinColor(CustomizationRegistry.Instance.SkinColors[_skinsToggleGroup.SelectedIndex]);
		}
	}

	private void ShowPremiumInfoForFaces()
	{
		CustomizationRegistry instance = CustomizationRegistry.Instance;
		List<int> list = ((_currentsSex != 0) ? instance.PremiumFemaleFaceIndeces : instance.PremiumMaleFaceIndeces);
		bool active = DataKeeper.IsUserDummy || (!DataKeeper.BackendInfo.user.has_premium && list.Contains(_currentFaceIndex));
		_vipMessage.SetActive(active);
	}

	private void OnViewTypeChange()
	{
		EnableRainCoats(false);
		switch (_viewTypes.SelectedIndex)
		{
		case 1:
			ShowOnlyForVipMessage(false);
			SexChangeObj.SetActive(false);
			break;
		case 2:
			ShowOnlyForVipMessage(true);
			EnableRainCoats(true);
			Debug.Log("!!!! OnVIEW CHANGE TYPE");
			break;
		default:
			ShowPremiumInfoForFaces();
			SexChangeObj.SetActive(true);
			break;
		}
		ChangeClothings();
	}

	private void OnClickBack()
	{
		base.gameObject.SetActive(false);
		_mainMenu.SetActive(true);
	}

	private void EnableRainCoats(bool flag)
	{
		_rainCoats.SetActive(flag);
		if (flag)
		{
			_characterRef.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			ChangeRainCoat(CurrentRainCoatIndex);
		}
		else
		{
			_characterRef.transform.localEulerAngles = new Vector3(0f, 150f, 0f);
		}
	}

	private void ChangeRainCoat(int textureIndex)
	{
		if (textureIndex >= _rainCoatsTextures.Count)
		{
			CurrentRainCoatIndex = 0;
		}
		if (textureIndex < 0)
		{
			CurrentRainCoatIndex = _rainCoatsTextures.Count - 1;
		}
		_rainCoats.GetComponent<Renderer>().material.mainTexture = _rainCoatsTextures[CurrentRainCoatIndex];
	}

	private void ShowOnlyForVipMessage(bool isSuperVip)
	{
		if (!isSuperVip)
		{
			_superVipMessage.SetActive(false);
			_vipMessage.SetActive(!DataKeeper.BackendInfo.user.has_premium);
		}
		else
		{
			_vipMessage.SetActive(false);
			_superVipMessage.SetActive(!DataKeeper.BackendInfo.user.has_vip);
		}
	}
}
