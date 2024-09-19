using System.Collections.Generic;
using UnityEngine;

public class LocalizationController
{
	private static LocalizationController _instance;
	private List<LanguageKeyValue> _languageLabels = new List<LanguageKeyValue>();

	public static LocalizationController Instance
	{
		get
		{
			return _instance ?? (_instance = new LocalizationController());
		}
	}

	public void AddLanguageLabel(LanguageKeyValue label)
	{
		label.UpdateLabel();

		if (!_languageLabels.Contains(label))
		{
			_languageLabels.Add(label);
		}
	}

	public void RemoveLanguageLabel(LanguageKeyValue label)
	{
		if (_languageLabels.Contains(label))
		{
			_languageLabels.Remove(label);
		}
	}

	public void UpdateLabels()
	{
		foreach (LanguageKeyValue languageLabel in _languageLabels)
		{
			languageLabel.UpdateLabel();
		}
	}
}

public class MenuLocalizationController : MonoBehaviour
{
	[SerializeField] tk2dUIToggleButtonGroup _languages;
	private bool _isFirstToggle = true;
	public string CurrentLocale = "ru";

	private void Awake()
	{
		LoadCurrentLang();
		if (CurrentLocale == "en")
		{
			DataKeeper.Language = Language.English;
		}
		LocalizationController.Instance.UpdateLabels();
	}

	private void OnLanguageChange()
	{
		if (!_isFirstToggle)
		{
			switch (_languages.SelectedIndex)
			{
			case 0:
				DataKeeper.Language = Language.Russian;
				SaveCurrentLang("ru");
				break;
			case 1:
				DataKeeper.Language = Language.English;
				SaveCurrentLang("eng");
				break;
			default:
				DataKeeper.Language = Language.English;
				SaveCurrentLang("eng");
				break;
			}
			LocalizationController.Instance.UpdateLabels();
		}
		_isFirstToggle = false;
	}

	private void SaveCurrentLang(string lng)
	{
		PlayerPrefs.SetString("lng", lng);
	}

	private void LoadCurrentLang()
	{
		if (PlayerPrefs.HasKey("lng"))
		{
			string @string = PlayerPrefs.GetString("lng");
			if (@string == "ru")
			{
				DataKeeper.Language = Language.Russian;
			}
			else
			{
				DataKeeper.Language = Language.English;
			}
			LocalizationController.Instance.UpdateLabels();
		}
	}
}
