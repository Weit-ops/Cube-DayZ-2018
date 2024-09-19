using System;
using System.Collections;
using UnityEngine;

public class FirstSceneLoader : MonoBehaviour
{
	private const string MainMenuBundleName = "main_menu";
	public static FirstSceneLoader I;
	public static bool menu_loaded_correctly;

	private void Awake()
	{
		I = this;
		menu_loaded_correctly = true;
	}

	private void Start()
	{
		Debug.Log("init");
		StartCoroutine("WaitAndLoad");
	}

	public IEnumerator WaitAndLoad()
	{
		yield return new WaitForSeconds(5f);
		PhotonNetwork.LoadLevel(1);
	}

	public void LoadScene(string sceneName, Action onLoadCallback)
	{
		StartCoroutine(LoadGameLevel("main_menu", sceneName, onLoadCallback));
	}

	private IEnumerator LoadGameLevel(string bundleName, string levelName, Action onLoadCallback)
	{
		AssetBundlesService assetBundleService = AssetBundlesService.I;
		if (assetBundleService.GetDownloadingBundleResult(bundleName) == DownloadResult.Failed)
		{
			yield break;
		}
		while (assetBundleService.GetDownloadingBundleResult(bundleName) == DownloadResult.None || assetBundleService.GetDownloadingBundleResult(bundleName) == DownloadResult.Downloading)
		{
			yield return new WaitForSeconds(1f);
		}
		if (assetBundleService.GetDownloadingBundleResult(bundleName) == DownloadResult.Done)
		{
			string[] bundleScenes = assetBundleService.GetBundle(bundleName).GetAllScenePaths();
			if (bundleScenes != null)
			{
				bool containsScene = false;
				string[] array = bundleScenes;
				foreach (string bundleScene in array)
				{
					if (bundleScene.Contains(levelName))
					{
						containsScene = true;
						break;
					}
				}
				if (containsScene)
				{
					if (onLoadCallback != null)
					{
						onLoadCallback();
					}
					yield break;
				}
				Debug.Log("There is no level " + levelName + " in bundle " + bundleName + "!");
			}
			else
			{
				Debug.Log("Bundle " + bundleName + " not contains any scenes!");
			}
		}
		else
		{
			Debug.Log("Bundle " + bundleName + " download failed!");
		}
	}
}
