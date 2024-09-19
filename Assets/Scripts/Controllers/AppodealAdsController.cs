//using AppodealAds.Unity.Api;
//using AppodealAds.Unity.Common;
using UnityEngine;

public class AppodealAdsController : MonoBehaviour/*, IRewardedVideoAdListener, IVideoAdListener */
{
	private string appKey = "e88f9ddb14dd17d0d54430057e17ffe63519f3f180db400d";

	public static AppodealAdsController I;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	public void Clear()
	{
	}

	private void Start()
	{
		/*Appodeal.initialize(appKey, 130);
		Appodeal.setVideoCallbacks(this);
		Appodeal.setRewardedVideoCallbacks(this);
		Appodeal.setLogging(true);*/
	}

	public void ShowVideo()
	{
		/*Debug.LogError("ShowVideo   " + MobileTicketsController.I.GetTicketsCount());
		if (Appodeal.isLoaded(2))
		{
			AppodealShowVideo();
		}
		else if (Appodeal.isLoaded(128))
		{
			AppodealShowRewardedVideo();
		}
		else
		{
			Debug.Log("Video not found ! ");
		}*/
	}

	public void AppodealShowRewardedVideo()
	{
/*		if (Appodeal.isLoaded(128))
		{
			Debug.Log("Show rewarded video ad");
			Appodeal.show(128);
		}
		else*/
		{
			Debug.Log("Video rewarded ad is not found");
		}
	}

	public void AppodealShowVideo()
	{
		/*if (Appodeal.isLoaded(2))
		{
			Debug.Log("Show video ad");
			Appodeal.show(2);
		}
		else*/
		{
			Debug.Log("Video ad is not found");
		}
	}

	private void CustomOnVideoFinished(int amount, string name)
	{
		/*if (MainMenuController.I != null)
		{
			MobileTicketsController.I.AddTickets(MobileTicketsController.I.ChargeTicketCount);
		}
		if (MobileTicketsView.I != null)
		{
			MobileTicketsView.I.SetInfo();
		}*/

		Debug.Log("OnVIDEO FINISHED  count = " + amount + "  name = " + name);
		MonoBehaviour.print("OnVIDEO FINISHED  count = " + amount + "  name = " + name);
	}

	public void onVideoLoaded()
	{
		MonoBehaviour.print("Video loaded");
	}

	public void onVideoFailedToLoad()
	{
		MonoBehaviour.print("Video failed");
	}

	public void onVideoShown()
	{
		MonoBehaviour.print("Video opened");
	}

	public void onVideoClosed()
	{
		MonoBehaviour.print("Video closed");
		if (MobileTicketsView.I != null)
		{
			MobileTicketsView.I.SetInfo();
		}
	}

	public void onVideoFinished()
	{
		CustomOnVideoFinished(1, "tickets");
		MonoBehaviour.print("Video finished");
	}

	public void onRewardedVideoLoaded()
	{
		MonoBehaviour.print("Rewarded Video loaded");
	}

	public void onRewardedVideoFailedToLoad()
	{
		MonoBehaviour.print("Rewarded Video failed");
	}

	public void onRewardedVideoShown()
	{
		MonoBehaviour.print("Rewarded Video opened");
	}

	public void onRewardedVideoClosed()
	{
		MonoBehaviour.print("Rewarded Video closed");
		if (MobileTicketsView.I != null)
		{
			MobileTicketsView.I.SetInfo();
		}
	}

	public void onRewardedVideoFinished(int amount, string name)
	{
		CustomOnVideoFinished(amount, name);
		MonoBehaviour.print("Rewarded Video finished: Reward: " + amount + name);
	}
}
