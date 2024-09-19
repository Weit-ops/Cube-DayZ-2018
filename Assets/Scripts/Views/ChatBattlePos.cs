using UnityEngine;

public class ChatBattlePos : MonoBehaviour
{
	private Vector3 oldPos;
	private Vector3 disAblePos = new Vector3(-20f, -1.625f, -3f);
	public GameObject CharecterMenu;
	private Transform thisTransform;

	[SerializeField] tk2dTextMesh _helpTextHideChat;

	private void Awake()
	{
		thisTransform = base.transform;
		oldPos = thisTransform.position;
		if (PlayerPrefs.HasKey("showchat") && PlayerPrefs.GetInt("showchat") == 0)
		{
			HideChat();
			switch (DataKeeper.Language)
			{
			case Language.Russian:
				_helpTextHideChat.text = "'X' - показать чат";
				break;
			case Language.English:
				_helpTextHideChat.text = "'X' - show chat";
				break;
			default:
				_helpTextHideChat.text = "'X' - показать чат";
				break;
			}
		}
	}

	private void OnEnable()
	{
	}

	private void Update()
	{
		if (!Input.GetKeyUp(KeyCode.X) || !(ChatGui.I != null) || ChatGui.I.chatIsFocused)
		{
			return;
		}
		if (thisTransform.position == oldPos)
		{
			HideChat();
			switch (DataKeeper.Language)
			{
			case Language.Russian:
				_helpTextHideChat.text = "'X' - показать чат";
				break;
			case Language.English:
				_helpTextHideChat.text = "'X' - show chat";
				break;
			default:
				_helpTextHideChat.text = "'X' - показать чат";
				break;
			}
			PlayerPrefs.SetInt("showchat", 0);
		}
		else
		{
			ShowChat();
			switch (DataKeeper.Language)
			{
			case Language.Russian:
				_helpTextHideChat.text = "'X' - cкрыть чат";
				break;
			case Language.English:
				_helpTextHideChat.text = "'X' - hide chat";
				break;
			default:
				_helpTextHideChat.text = "'X' - cкрыть чат";
				break;
			}
			PlayerPrefs.SetInt("showchat", 1);
		}
	}

	public void HideChat()
	{
		if (thisTransform != null)
		{
			thisTransform.position = disAblePos;
		}
	}

	public void ShowChat()
	{
		if (thisTransform != null)
		{
			thisTransform.position = oldPos;
		}
	}
}
