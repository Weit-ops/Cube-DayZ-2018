using System;
using UnityEngine;

public class PremiumTimeView : MonoBehaviour
{
	public static PremiumTimeView I;

	[SerializeField] ShadowText _timeLabel;

	private void Awake()
	{
		I = this;
		UpdateStatus();
	}

	private void LateUpdate(){
		UpdateStatus ();
	}
	public void UpdateStatus()
	{
		if (!DataKeeper.IsUserDummy)
		{
			if (DataKeeper.BackendInfo.user.has_vip)
			{
				string dateTime = DataKeeper.BackendInfo.user.premium_vip_end_time;
				switch (DataKeeper.Language)
				{
				case Language.Russian:
					_timeLabel.SetText("Окончание: " + dateTime);
					break;
				case Language.English:
					_timeLabel.SetText("End time: " + dateTime);
					break;
				default:
					_timeLabel.SetText("End time: " + dateTime);
					break;
				}
			}
			else if (DataKeeper.BackendInfo.user.has_premium)
			{
				string dateTime2 = DataKeeper.BackendInfo.user.premium_end_time;
				switch (DataKeeper.Language)
				{
				case Language.Russian:
					_timeLabel.SetText("Окончание: " + dateTime2);
					break;
				case Language.English:
					_timeLabel.SetText("End time: " + dateTime2);
					break;
				default:
					_timeLabel.SetText("End time: " + dateTime2);
					break;
				}
			}
			else
			{
				_timeLabel.SetText(string.Empty);
			}
		}
		else
		{
			_timeLabel.SetText(string.Empty);
		}
	}
}
