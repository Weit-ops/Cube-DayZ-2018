using UnityEngine;

public class LoginMenuBlock : MonoBehaviour
{
	public static LoginMenuBlock I;

	[SerializeField] tk2dTextMesh _nameLabel;
	[SerializeField] tk2dTextMesh _idLabel;
	[SerializeField] GameObject _shopButtonBlock;
	[SerializeField] GameObject _vipButtonBlock;
	[SerializeField] GameObject _multiplayerButtonBlock;
	[SerializeField] GameObject _garageButtonBlock;
	[SerializeField] GameObject _offerButtonBlock;

	private void Awake()
	{
		I = this;
	}

	private void Start()
	{
		SetPlayerId(JsSpeeker.viewer_id);
		SetPlayerName(JsSpeeker.vk_name);
	}

	public void SetPlayerId(string playerId)
	{
		if (!string.IsNullOrEmpty(playerId))
		{
			_idLabel.text = playerId;
			_shopButtonBlock.SetActive(false);
			_vipButtonBlock.SetActive(false);
			_multiplayerButtonBlock.SetActive(false);
			if (_garageButtonBlock != null)
			{
				_garageButtonBlock.SetActive(false);
			}
			if (_offerButtonBlock != null)
			{
				_offerButtonBlock.SetActive(false);
			}
			MainMenuController.I.CheckSpecialOffer();
		}
		else
		{
			_idLabel.text = "^CFF0000FF%userid%";
			_shopButtonBlock.SetActive(true);
			_vipButtonBlock.SetActive(true);
			_multiplayerButtonBlock.SetActive(true);
			if (_garageButtonBlock != null)
			{
				_garageButtonBlock.SetActive(true);
			}
			if (_offerButtonBlock != null)
			{
				_offerButtonBlock.SetActive(true);
			}
		}
	}

	public void SetPlayerName(string playerName)
	{
		if (!string.IsNullOrEmpty(playerName))
		{
			_nameLabel.text = playerName;
		}
		else
		{
			_nameLabel.text = "^CFF0000FF%username%";
		}
	}
}
