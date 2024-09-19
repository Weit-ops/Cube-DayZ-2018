using System.Text;
using BattleRoyale;
using CodeStage.AdvancedFPSCounter;
using UnityEngine;

public class MenuViewController : MonoBehaviour
{
	[SerializeField]ShadowText _staticticsInfo;
	[SerializeField]GameObject _statisticsView;
	[SerializeField]GameObject _settingsView;
	[SerializeField]GameObject _buttons;
	[SerializeField]tk2dUIToggleButtonGroup _qualityLvl;
	[SerializeField]tk2dUIToggleButton _profileInfoToggle;

	public GameObject SuicideBtn;
	public GameObject BattleRoayleExitMenu;

	public static MenuViewController I;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
		_qualityLvl.SelectedIndex = QualitySettings.GetQualityLevel();
		_profileInfoToggle.IsOn = DataKeeper.ShowProfileInfo;
	}

	private void OnEnable()
	{
		_statisticsView.SetActive(true);
		_settingsView.SetActive(false);
		UpdateStatistics ();

		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars)
		{
			SuicideBtn.SetActive(false);
		}
	}

	private void OnDisable()
	{
		BattleRoayleExitMenu.SetActive(false);
		_statisticsView.SetActive(true);
		_buttons.SetActive(true);
	}

	private void UpdateStatistics()
	{
		StatisticsInfo playerStatistic = WorldController.I.PlayerStatistic;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(playerStatistic.ZombieKills).Append("\n").Append(playerStatistic.CreatureKills)
			.Append("\n")
			.Append(playerStatistic.PlayerKills)
			.Append("\n")
			.Append("\n")
			.Append(playerStatistic.CraftItems)
			.Append("\n")
			.Append(playerStatistic.PlantsPlanted)
			.Append("\n")
			.Append("\n")
			.Append(playerStatistic.Die)
			.Append("\n")
			.Append(playerStatistic.Suicide);
		_staticticsInfo.SetText(stringBuilder.ToString());
	}

	private void OnClickConsume()
	{
		GameUIController.I.ShowCharacterMenu(false, CharacterMenuType.Menu);
	}

	private void OnClickSettings()
	{
		_statisticsView.SetActive(false);
		_settingsView.SetActive(true);
	}

	private void OnQuailityChanged()
	{
		if (_qualityLvl.SelectedIndex >= 0 && QualitySettings.names.Length > _qualityLvl.SelectedIndex)
		{
			QualitySettings.SetQualityLevel(_qualityLvl.SelectedIndex);
		}

		CameraQualityController[] array = Object.FindObjectsOfType<CameraQualityController>();

		foreach (CameraQualityController cameraQualityController in array)
		{
			cameraQualityController.Set();
		}

		PlayerPrefs.SetInt("QuailityLvl", QualitySettings.GetQualityLevel());
		PlayerPrefs.Save();
	}

	private void OnProfileToggleChanged()
	{
		DataKeeper.ShowProfileInfo = _profileInfoToggle.IsOn;

		if (_profileInfoToggle.IsOn)
		{
			AFPSCounter.Instance.OperationMode = OperationMode.Normal;
			PlayerPrefs.SetInt("AFPS", 1);
		}
		else
		{
			AFPSCounter.Instance.OperationMode = OperationMode.Disabled;
			PlayerPrefs.SetInt("AFPS", 0);
		}

		PlayerPrefs.Save();
	}

	private void OnClickSuicide()
	{
		if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
		{
			GameUIController.I.ShowCharacterMenu(false, CharacterMenuType.Menu);
			WorldController.I.Player.Suicide();
		}
	}

	private void OnClickExitBattleRoyaleMenu()
	{
		if (!BattleRoyaleGameManager.I.IsLobby())
		{
			BattleRoyaleGameManager.I.StartShowKillLog(string.Empty, JsSpeeker.vk_name, string.Empty, null);
		}
		ExitFromGame();
	}

	private void OnClickExit()
	{
		if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
		{
			ExitFromGame();
		}
		else
		{
			ShowExitDialog();
		}
	}

	public void ShowExitDialog()
	{
		_statisticsView.SetActive(false);
		_buttons.SetActive(false);
		BattleRoayleExitMenu.SetActive(true);
	}

	private void ExitFromGame()
	{
		RespawnMenuController.SetDieFlagFalse();
		WorldController.I.SavePlayer(DataKeeper.GameType == GameType.Single);

		if (PhotonNetwork.isMasterClient && WorldController.I.MultiplayerWorldLoaded && PhotonNetwork.room != null)
		{
			WorldController.I.SaveWorldInMultiplayer();
		}

		WorldController.I.LeaveRoomBySelf = true;
		WorldController.I.StopCoroutine("AddMobsForPulling");
		WorldController.I.StopCoroutine("AutoSaveMultiplayerWorld");

		if (PhotonNetwork.room != null)
		{
			PhotonNetwork.LeaveRoom();
		}
	}

	private void OnLeftRoom()
	{
		if (WorldController.I.LeaveRoomBySelf)
		{
			PhotonNetwork.LoadLevel("MainMenu");
		}
	}

	public void ExitFromServer()
	{
		WorldController.I.LeaveRoomBySelf = true;
		if (PhotonNetwork.room != null)
		{
			PhotonNetwork.LeaveRoom();
		}
	}
}
