using UnityEngine;

public class TutorialBtnController : MonoBehaviour
{
	private enum checkMap
	{
		Map1 = 0,
		MapSkyWars = 1
	}

	[SerializeField] GameObject ActiveBtn;
	[SerializeField] GameObject DisActiveBtn;
	[SerializeField] checkMap map;

	private void Update()
	{
		if (AssetBundlesService.I != null)
		{
			if (map == checkMap.Map1)
			{
				if (AssetBundlesService.I.CheckBundle("map_1"))
				{
					DisActiveBtn.SetActive(false);
					ActiveBtn.SetActive(true);
				}
				else
				{
					DisActiveBtn.SetActive(true);
					ActiveBtn.SetActive(false);
				}
			}
			else if (AssetBundlesService.I.CheckBundle("map_2_skywars"))
			{
				DisActiveBtn.SetActive(false);
				ActiveBtn.SetActive(true);
			}
			else
			{
				DisActiveBtn.SetActive(true);
				ActiveBtn.SetActive(false);
			}
		}
		DisActiveBtn.SetActive(false);
		ActiveBtn.SetActive(true);
	}
}
