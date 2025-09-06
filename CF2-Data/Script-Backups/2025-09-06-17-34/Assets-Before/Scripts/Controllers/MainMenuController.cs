using System.Collections;
using CodeStage.AdvancedFPSCounter;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
	private const string GamelvlName = "map_1";
	private const string PremiumPackId = "333";

	[SerializeField] CustomizationSkinController _customize;
	[SerializeField] GameObject _shop;
	[SerializeField] GameObject _block;
	[SerializeField] GameObject _disabledMicrophone;
	[SerializeField] GameObject _chooseGameTypeMenu;
	[SerializeField] GameObject _inviteFirendsPanel;
	[SerializeField] GameObject _specialOffer1;
	[SerializeField] GameObject _specialOffer2;
	[SerializeField] GameObject _specialOffer3;
	public GameObject _errorAuthOldVersionPanel;
	public GameObject _errorAuthBanPanel;
	public GameObject _loadingAccountPanel;
	public GameObject _newNamePanel;
	public GameObject MobileLoginWindow;

	public ReskinGameType _reskinType;
	public static MainMenuController I;
	public int PackIndex;

	private void Awake()
	{
		I = this;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		InventoryController.Clear();
		_customize.gameObject.SetActive(false);
		_disabledMicrophone.SetActive(!Application.HasUserAuthorization(UserAuthorization.Microphone));
		if (_reskinType == ReskinGameType.Wasteland)
		{
			QualitySettings.SetQualityLevel(2);
		}
		else if (PlayerPrefs.HasKey("QuailityLvl"))
		{
			QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QuailityLvl"));
		}
	}

	private void OnEnable()
	{
		StartCoroutine("UpdateFriendListInfos");
		if (UpdateManager.I != null)
		{
			UpdateManager.I.UnSubscribeServerChat();
		}
		CheckSpecialOffer();
	}

	private void OnDisable()
	{
		StopCoroutine("UpdateFriendListInfos");
	}

	private void Start()
	{
		if (PlayerPrefs.HasKey("AFPS"))
		{
			DataKeeper.ShowProfileInfo = PlayerPrefs.GetInt("AFPS") > 0;
			if (DataKeeper.ShowProfileInfo)
			{
				AFPSCounter.Instance.OperationMode = OperationMode.Normal;
			}
			else
			{
				AFPSCounter.Instance.OperationMode = OperationMode.Disabled;
			}
		}
		StartCoroutine("RequestUserAuthorization");
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.F12))
		{
			if (ChatGui.I != null && !ChatGui.I.chatIsFocused)
			{
				ScreenUtils.FullScreen(!Screen.fullScreen);
			}
			if (JsSpeeker.I.ReskinType != 0 || DataKeeper.GameType == GameType.SkyWars)
			{
				ScreenUtils.FullScreen(!Screen.fullScreen);
			}
		}
	}

	private IEnumerator RequestUserAuthorization()
	{
		_block.SetActive(true);
		yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
		_disabledMicrophone.SetActive(!Application.HasUserAuthorization(UserAuthorization.Microphone));
		_block.SetActive(false);
	}

	private void OnClickCustomize()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		base.gameObject.SetActive(false);
		_customize.gameObject.SetActive(true);
	}

	private void OnCkickStart()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		base.gameObject.SetActive(false);
		_chooseGameTypeMenu.SetActive(true);
	}

	private void OnClickMicrophone()
	{
		StartCoroutine("RequestUserAuthorization");
	}

	private void OnClickShop()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		PackIndex = 9;
		_shop.SetActive(true);
		base.gameObject.SetActive(false);
	}

	private void OnClickBuyPremium()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		PackIndex = 9;
		base.gameObject.SetActive(false);
		_shop.SetActive(true);
	}

	private void OnUpdatedFriendList()
	{
		if (FriendsAndClanPanelView.I.GetAllFriendsItems() != null)
		CubezStatic.UpdateFriendOnlineStatus(FriendsAndClanPanelView.I.GetAllFriendsItems());
	}

	private IEnumerator UpdateFriendListInfos()
	{
		while (true)
		{
			yield return new WaitForSeconds(5f);
			CubezStatic.UpdatePhotonFriends();
		}
	}

	public void HideMainMenu()
	{
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void OnClickGarage()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		OpenCarage(11);
	}

	public void OnClickOffer()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		byte offerStatus = PHPNetwork.I.GetOfferStatus();
		int id = 11;
		if (offerStatus == 2)
		{
			id = 13;
		}
		if (offerStatus == 3)
		{
			id = 18;
		}
		OpenCarage(id);
	}

	public void CheckSpecialOffer()
	{
		if (!string.IsNullOrEmpty(JsSpeeker.viewer_id) && PHPNetwork.I != null)
		{
			byte offerStatus = PHPNetwork.I.GetOfferStatus();
			if (offerStatus == 1)
			{
				_specialOffer1.SetActive(true);
				_specialOffer2.SetActive(false);
				_specialOffer3.SetActive(false);
			}
			if (offerStatus == 2)
			{
				_specialOffer1.SetActive(false);
				_specialOffer2.SetActive(true);
				_specialOffer3.SetActive(false);
			}
			if (offerStatus == 3)
			{
				_specialOffer1.SetActive(false);
				_specialOffer2.SetActive(false);
				_specialOffer3.SetActive(true);
			}
		}
	}

	private void OpenCarage(int id)
	{
		PackIndex = id;
		_shop.SetActive(true);
		base.gameObject.SetActive(false);
	}

	private void RateUs()
	{
		Debug.Log("rate us");
		ReviewHelper.RateApplication();
	}

	private void MailUs()
	{
	}

	public void ShowInviteFriendsPanel()
	{
		_inviteFirendsPanel.SetActive(true);
		base.gameObject.SetActive(false);
	}

	public void ToggleMainMenu(bool flag)
	{
		MenuConnectionViewController.I.MainMenuContent.SetActive(flag);
		MenuConnectionViewController.I.MainMenuZombiez.SetActive(flag);
		if (flag)
		{
			PlayerPrefs.SetString("logout3", "false");
		}
	}
}
