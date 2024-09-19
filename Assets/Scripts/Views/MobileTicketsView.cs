using UnityEngine;

public class MobileTicketsView : MonoBehaviour
{
	[SerializeField] tk2dTextMesh _ticketInfo;

	public static MobileTicketsView I;

	public void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	public void OnEnable()
	{
		SetInfo();
	}

	public void Start()
	{
		SetInfo();
	}

	public void SetInfo()
	{
		if (MobileTicketsController.I != null)
		{
			_ticketInfo.text = MobileTicketsController.I.GetTicketsCount().ToString();
		}
	}
}
