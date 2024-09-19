using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class MobileTicketsController : MonoBehaviour
{
	private const string PREFS_TICKETS = "ticket6";
	private ObscuredInt _tickets = 1;
	public ObscuredInt ChargeTicketCount = 1;
	public string EncryptionKey = "change me!";

	[SerializeField] GameObject _dialogWindow;

	public static MobileTicketsController I;

	private void Awake()
	{
		ObscuredPrefs.CryptoKey = EncryptionKey;
		if (I == null)
		{
			I = this;
		}
		LoadTickets();
	}

	public void OnDisable()
	{
		SaveTickets();
	}

	public int GetTicketsCount()
	{
		return _tickets;
	}

	public void AddTickets(ObscuredInt count)
	{
		Debug.LogError("AddTickets" + GetTicketsCount());
		_tickets = (int)_tickets + (int)count;
		SaveTickets();
		MobileTicketsView.I.SetInfo();
	}

	public void ReduceTickets(ObscuredInt count)
	{
		_tickets = (int)_tickets - (int)count;
		if ((int)_tickets < 0)
		{
			_tickets = 0;
		}
		SaveTickets();
	}

	private void LoadTickets()
	{
		_tickets = ((!ObscuredPrefs.HasKey("ticket6")) ? ((int)_tickets) : ObscuredPrefs.GetInt("ticket6"));
	}

	private void SaveTickets()
	{
		ObscuredPrefs.SetInt("ticket6", _tickets);
	}

	public void ToggleDialogWindow(bool flag)
	{
		_dialogWindow.SetActive(flag);
	}

	public void OnDialogClickOk()
	{
		ToggleDialogWindow(false);
		MainMenuController.I.ToggleMainMenu(true);
		AppodealAdsController.I.ShowVideo();
	}

	public void OnDialogClickCancel()
	{
		ToggleDialogWindow(false);
		MainMenuController.I.ToggleMainMenu(true);
	}
}
