using UnityEngine;

public class SubscriptionStatusViewInfo : MonoBehaviour
{
	public static SubscriptionStatusViewInfo I;

	[SerializeField] ShadowText _statusLabel;

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
				if (DataKeeper.Language == Language.Russian)
				{
					_statusLabel.SetText("SUPERVIP");
				}
				else
				{
					_statusLabel.SetText("SUPERVIP");
				}
			}
			else if (DataKeeper.BackendInfo.user.has_premium)
			{
				if (DataKeeper.Language == Language.Russian)
				{
					_statusLabel.SetText("VIP");
				}
				else
				{
					_statusLabel.SetText("VIP");
				}
			}
			else
			{
				_statusLabel.SetText(string.Empty);
			}
		}
		else
		{
			_statusLabel.SetText(string.Empty);
		}
	}
}
