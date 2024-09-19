using System;
using UnityEngine;

public class SoundSettings : MonoBehaviour
{
	private const string MusicKey = "Music";

	[SerializeField] tk2dUIScrollbar _musicVolumeBar;
	[SerializeField] GameObject _disableMusic;
	[SerializeField] AudioSource _mainMenuMusic;

	private float _previousMusicVolume;

	private void OnEnable()
	{
		if (PlayerPrefs.HasKey("Music"))
		{
			_mainMenuMusic.volume = PlayerPrefs.GetFloat("Music");
		}
		if ((double)_mainMenuMusic.volume < 0.01)
		{
			_previousMusicVolume = 0.5f;
		}
		_musicVolumeBar.Value = _mainMenuMusic.volume;
	}

	private void OnMusicVolumeScroll()
	{
		_mainMenuMusic.volume = (float)Math.Round(_musicVolumeBar.Value, 2);
		_disableMusic.SetActive((double)_mainMenuMusic.volume < 0.01);
		if ((double)_musicVolumeBar.Value >= 0.01)
		{
			_previousMusicVolume = _mainMenuMusic.volume;
		}
	}

	private void OnClickDisableMusic()
	{
		if ((double)_musicVolumeBar.Value < 0.01)
		{
			_musicVolumeBar.Value = _previousMusicVolume;
		}
		else
		{
			_musicVolumeBar.Value = 0f;
		}
	}

	private void OnDisable()
	{
		if (_mainMenuMusic != null)
		{
			PlayerPrefs.SetFloat("Music", _mainMenuMusic.volume);
			PlayerPrefs.Save();
		}
	}
}
