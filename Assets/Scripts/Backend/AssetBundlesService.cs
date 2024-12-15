using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BundleInfo
{
	public string Name;
	public string Url;
	public int Version;
}

public enum DownloadResult
{
	None = 0,
	Downloading = 1,
	Done = 2,
	Failed = 3
}

public class BundleDownloadProcessInfo
{
	public Coroutine DownloadingCoroutine;
	public WWW Www;
	public int BundleIndex;
	public int DownloadTryCount;
}

public class AssetBundlesService : MonoBehaviour
{
	private const int ImportantDownloadingTryCount = 5;
	private const int DownloadingTryCount = 5;
	private const float DownloadingTryCooldown = 2.5f;

	public static AssetBundlesService I;

	[SerializeField] string _serverBundlesUrl;
	[SerializeField] string _iosServerBundlesUrl;
	[SerializeField] string _androidServerBundlesUrl;
	[SerializeField] string _reskinWastelandServerBundlesUrl;

	private string _bundlesUrl;

	[SerializeField] int DowloadingBundleProcessCount = 3;
	[SerializeField] List<BundleInfo> _importantBundlesInfo;
	[SerializeField] List<BundleInfo> _additionalBundlesInfo;

	private Dictionary<string, DownloadResult> _downloadResults = new Dictionary<string, DownloadResult>();
	private Dictionary<string, AssetBundle> _bundles = new Dictionary<string, AssetBundle>();
	private List<BundleDownloadProcessInfo> _bundlesDownloadInfo = new List<BundleDownloadProcessInfo>();

	private int _currentImportantBundleIndex = 0;
	private int _currentAdditionalBundleIndex;
	private WWW _importantBundleWWW;
	private int _currentImportantDownloadTryCount;
	private bool _downloadingImportantBundle;
	private Coroutine _importantBundleCheckCoroutine;
	private Coroutine _importantBundleCoroutine;

	[SerializeField] bool _allBundLoaded;

	public ReskinGameType ReskinType;
	public int Progress;

	private void DefineBundleUrl()
	{
		_bundlesUrl = _androidServerBundlesUrl;
	}

	public bool IsAllBundleLoaded()
	{
		bool result = true;

		foreach (KeyValuePair<string, DownloadResult> downloadResult in _downloadResults)
		{
			if (downloadResult.Value == DownloadResult.Downloading)
			{
				result = false;
			}
		}

		return result;
	}

	private void Awake()
	{
		Application.targetFrameRate = 72;
		I = this;
		DefineBundleUrl();
		GameObject.DontDestroyOnLoad(base.gameObject);
		Initialize();
	}

	public void DownloadImportantBundle(string bundleName)
	{
		if (_downloadResults.ContainsKey(bundleName) && (_downloadResults[bundleName] == DownloadResult.None || _downloadResults[bundleName] == DownloadResult.Failed))
		{
			StartCoroutine("DownloadImportantBundleCoroutine", bundleName);
		}
	}

	private IEnumerator DownloadImportantBundleCoroutine(string bundleName)
	{
		Debug.Log("DownloadImportantBundleCoroutine: " + bundleName + ", " + Time.time);
		_downloadingImportantBundle = false;

		if (_importantBundleCheckCoroutine != null)
		{
			StopCoroutine(_importantBundleCheckCoroutine);
		}

		if (_importantBundleCoroutine != null)
		{
			StopCoroutine(_importantBundleCoroutine);
		}

		if (_importantBundleWWW != null)
		{
			_importantBundleWWW.Dispose();
		}

		yield return null;
		_importantBundleCoroutine = StartCoroutine(LoadImportantBundle(bundleName));
	}

	public AssetBundle GetBundle(string bundleName)
	{
		if (_bundles.ContainsKey(bundleName))
		{
			return _bundles[bundleName];
		}

		return null;
	}

	public DownloadResult GetDownloadingBundleResult(string bundleName)
	{
		if (_downloadResults.ContainsKey(bundleName))
		{
			return _downloadResults[bundleName];
		}

		return DownloadResult.Failed;
	}

	private void Initialize()
	{
		foreach (BundleInfo item in _importantBundlesInfo)
		{
			_downloadResults.Add(item.Name, DownloadResult.None);
		}

		foreach (BundleInfo item2 in _additionalBundlesInfo)
		{
			_downloadResults.Add(item2.Name, DownloadResult.None);
		}

		for (int i = 0; i < DowloadingBundleProcessCount; i++)
		{
			_bundlesDownloadInfo.Add(new BundleDownloadProcessInfo());
			StartLoadNewBundle(i);
		}
	}

	private IEnumerator LoadImportantBundle(string bundleName)
	{
		_downloadingImportantBundle = true;
		string bundleName2 = default(string);

		while (_downloadingImportantBundle)
		{
			BundleInfo bundle = _importantBundlesInfo.Find((BundleInfo b) => b.Name == bundleName2);

			if (bundle != null && !string.IsNullOrEmpty(bundle.Url))
			{
				string url = _bundlesUrl + bundle.Url;
				_importantBundleWWW = WWW.LoadFromCacheOrDownload(url, bundle.Version);
				_downloadResults[bundle.Name] = DownloadResult.Downloading;

				if (_importantBundleCheckCoroutine != null)
				{
					StopCoroutine(_importantBundleCheckCoroutine);
				}

				_importantBundleCheckCoroutine = StartCoroutine(CheckImportantBundleProgress(bundleName));
				yield return _importantBundleWWW;

				if (_importantBundleCheckCoroutine != null)
				{
					StopCoroutine(_importantBundleCheckCoroutine);
				}

				if (_importantBundleWWW.error == null)
				{
					AssetBundle assetBundle = _importantBundleWWW.assetBundle;
					_bundles.Add(bundle.Name, assetBundle);
					assetBundle.LoadAllAssets();

					_downloadResults[bundle.Name] = DownloadResult.Done;
					_currentImportantDownloadTryCount = 0;

					Debug.Log("Bundle " + bundle.Name + " successfully downloaded!");

					_downloadingImportantBundle = false;
					if (CheckBundle("map_1") && !CheckBundle("map_2_skywars"))
					{
						DownloadImportantBundle("map_2_skywars");
					}
				}
				else
				{
					_currentImportantDownloadTryCount++;

					if (_currentImportantDownloadTryCount > 5)
					{
						_downloadResults[bundle.Name] = DownloadResult.Failed;
						Debug.Log("Failed to download asset! " + bundle.Name + " Reason: " + _importantBundleWWW.error);
						_downloadingImportantBundle = false;
					}
					else
					{
						Debug.Log("Retry download bundle " + bundle.Name);
						yield return new WaitForSeconds((float)_currentImportantDownloadTryCount * 2.5f);
					}
				}

				_importantBundleWWW.Dispose();
			} else {
				_downloadingImportantBundle = false;
				Debug.Log("Incorrect bundle info!");
			}
		}
	}

	private IEnumerator CheckImportantBundleProgress(string bundleName)
	{
		bool needStopCoroutine = false;

		while (!needStopCoroutine && _importantBundleWWW != null && !_importantBundleWWW.isDone)
		{
			int progress = (Progress = (int)(_importantBundleWWW.progress * 100f));
			Debug.Log("Downloading progress: " + progress + "%  " + bundleName);

			if (_importantBundleWWW.error != null)
			{
				needStopCoroutine = true;
				_currentImportantDownloadTryCount++;

				Debug.Log("Error when downloading!");

				if (_importantBundleCoroutine != null)
				{
					StopCoroutine(_importantBundleCoroutine);
				}
				if (_currentImportantDownloadTryCount > 5)
				{
					BundleInfo bundle = _importantBundlesInfo[_currentImportantBundleIndex];
					_downloadResults[bundle.Name] = DownloadResult.Failed;
					_downloadingImportantBundle = false;
					_currentImportantDownloadTryCount = 0;

					Debug.Log("Failed to download asset! " + bundle.Name + " Reason: " + _importantBundleWWW.error);
				}
				else
				{
					yield return new WaitForSeconds((float)_currentImportantDownloadTryCount * 2.5f);
					_importantBundleCoroutine = StartCoroutine(LoadImportantBundle(bundleName));
				}
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void StartLoadNewBundle(int downloadingProcessIndex)
	{
		if (_currentAdditionalBundleIndex <= _additionalBundlesInfo.Count - 1)
		{
			_bundlesDownloadInfo[downloadingProcessIndex].BundleIndex = _currentAdditionalBundleIndex;
			_currentAdditionalBundleIndex++;
			_bundlesDownloadInfo[downloadingProcessIndex].DownloadTryCount = 0;
			_bundlesDownloadInfo[downloadingProcessIndex].DownloadingCoroutine = StartCoroutine(LoadAdditionalBundle(downloadingProcessIndex));
		}
	}

	private IEnumerator LoadAdditionalBundle(int downloadingProcessIndex)
	{
		BundleDownloadProcessInfo currentProcessInfo = _bundlesDownloadInfo[downloadingProcessIndex];
		bool downloading = true;

		while (downloading)
		{
			BundleInfo bundle = _additionalBundlesInfo[currentProcessInfo.BundleIndex];
			if (!string.IsNullOrEmpty(bundle.Url))
			{
				string url = _bundlesUrl + bundle.Url;
				currentProcessInfo.Www = WWW.LoadFromCacheOrDownload(url, bundle.Version);
				_downloadResults[bundle.Name] = DownloadResult.Downloading;

			
				yield return currentProcessInfo.Www;

				if (currentProcessInfo.Www.error == null)
				{
					downloading = false;
					AssetBundle assetBundle = currentProcessInfo.Www.assetBundle;
					_bundles.Add(bundle.Name, assetBundle);
					assetBundle.LoadAllAssets();

					_downloadResults[bundle.Name] = DownloadResult.Done;
					BundleResourceManager.I.OnAssetBundleDownloaded(bundle.Name);
					currentProcessInfo.Www.Dispose();

					StartLoadNewBundle(downloadingProcessIndex);
					continue;
				}
				currentProcessInfo.DownloadTryCount++;
				if (currentProcessInfo.DownloadTryCount > 5)
				{
					downloading = false;
					_downloadResults[bundle.Name] = DownloadResult.Failed;
					Debug.Log("Failed to download asset! " + bundle.Name + " Reason: " + currentProcessInfo.Www.error);
					currentProcessInfo.Www.Dispose();
					StartLoadNewBundle(downloadingProcessIndex);
				}
				else
				{
					Debug.Log("Retry download bundle " + bundle.Name);
					currentProcessInfo.Www.Dispose();
					yield return new WaitForSeconds((float)currentProcessInfo.DownloadTryCount * 2.5f);
				}
			}
			else
			{
				downloading = false;
				currentProcessInfo.Www.Dispose();
				StartLoadNewBundle(downloadingProcessIndex);
			}
		}
	}

	private IEnumerator CheckAdditionalBundlesProgress(int downloadingProcessIndex)
	{
		BundleDownloadProcessInfo currentProcessInfo = _bundlesDownloadInfo[downloadingProcessIndex];
		BundleInfo bundle = _additionalBundlesInfo[currentProcessInfo.BundleIndex];
		bool needStopThisCoroutine = false;

		while (!needStopThisCoroutine && currentProcessInfo.Www != null && !currentProcessInfo.Www.isDone)
		{
			if (currentProcessInfo.Www.error != null)
			{
				needStopThisCoroutine = true;
				currentProcessInfo.DownloadTryCount++;
				Debug.Log("Error when downloading!");
				StopCoroutine(currentProcessInfo.DownloadingCoroutine);

				if (currentProcessInfo.DownloadTryCount > 5)
				{
					_downloadResults[bundle.Name] = DownloadResult.Failed;
					Debug.Log("Failed to download asset! " + bundle.Name + " Reason: " + currentProcessInfo.Www.error);
					currentProcessInfo.Www.Dispose();
					StartLoadNewBundle(downloadingProcessIndex);
				}
				else
				{
					yield return new WaitForSeconds((float)currentProcessInfo.DownloadTryCount * 2.5f);
					currentProcessInfo.DownloadingCoroutine = StartCoroutine(LoadAdditionalBundle(downloadingProcessIndex));
				}
			}
			yield return new WaitForSeconds(1f);
		}
	}

	public bool CheckBundle(string bundleName)
	{
		return _bundles.ContainsKey(bundleName);
	}
}
