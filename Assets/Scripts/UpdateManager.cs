using System.Collections;
using Photon.Chat;
using UnityEngine;

public class UpdateManager : MonoBehaviour, IChatClientListener
{
	private static string _systemChatChanel = "systemChatChanel";
	public static string CommonChatChanel = "cubedayz#main1";
	public static string ServerChatChanel = string.Empty;

	[SerializeField] string _chatAppId;
	[SerializeField] ChatChannel _selectedChannel;

	public ChatClient ChatClient;

	[SerializeField] string _kickprase = string.Empty;

	public bool _securityFlag;
	public bool isConnected;

	private string[] _chatChanelsToDescribe2 = new string[2] { _systemChatChanel, CommonChatChanel };
	private string lastmessage = string.Empty;

	public string name_for_chat = "#";
	public static UpdateManager I { get; private set; }

	private void Awake()
	{
		if (I == null)
		{
			DontDestroyOnLoad (this.gameObject);
			I = this;
		}

		ChatClient = new ChatClient(this);
	}

	private void Update()
	{
		if (ChatClient != null) 
		{
			ChatClient.Service();
		}
	}

	public void StartConnection()
	{
		StartCoroutine("ChatReconnection");
	}

	public IEnumerator ChatReconnection()
	{
		while (ChatClient.State != ChatState.ConnectedToFrontEnd)
		{
			Connect();
			yield return new WaitForSeconds(5f);
		}
		if (ChatGui.I != null && ChatGui.I.chatType == ChatType.Server)
		{
			SubscribeToServerChat();
		}
		Debug.Log("Exit chat reconnection ");
	}

	public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
	{

	}

	public void Connect()
	{
		if (ChatClient != null)
		{
			if (ChatClient.State != ChatState.ConnectedToFrontEnd)
			{
				Debug.Log("Trying reconnect to chat" + ChatClient.State);
				name_for_chat = JsSpeeker.vk_name;
				UpdateTaggedName();
				ChatClient.Connect(_chatAppId, DataKeeper._chatAppVersion, new Photon.Chat.AuthenticationValues(name_for_chat));
			}
		}
		else
		{
			Debug.LogError("Chat clien is null");
		}
	}

	public void OnConnected()
	{
		Debug.Log("!!!!!!!!!! OnConnected to chat");
		ChatClient.Subscribe(_chatChanelsToDescribe2, 20);
	}

	public void OnSubscribed(string[] channels, bool[] results)
	{
		isConnected = true;
		Debug.Log("Subscribed to chanel " + channels[0]);
		StartCoroutine("UpdateMessageOnSuscribe");
		if (ChatGui.I != null)
		{
			ChatGui.I.UpdateChatCount();
		}
	}

	public void SendMessageToChat(string _chanelName, object _message)
	{
		if (!string.IsNullOrEmpty(_message.ToString()) && _message.ToString() != lastmessage)
		{
			Debug.Log("Send Message" + ChatClient.State);
			if (ChatClient.State == ChatState.ConnectedToFrontEnd)
			{
				lastmessage = _message.ToString();
				ChatClient.PublishMessage(_chanelName, _message);
			}
			else
			{
				Debug.LogError("Coonection server is false  " + ChatClient.State);
			}
		}
	}

	public void OnGetMessages(string channelName, string[] senders, object[] messages)
	{
		Debug.Log("On Get Message");
		if (channelName == _systemChatChanel)
		{
			for (int i = 0; i < messages.Length; i++)
			{
				Debug.Log("System message " + messages[i]);
				if (messages[i].ToString().Contains(_kickprase))
				{
					string text = messages[i].ToString();
					string[] array = text.Split('/');
					Debug.Log(array[0] + "---" + array[1]);
					if (array[1] != DataKeeper.BuildVersion)
					{
						RefreshFrame();
					}
				}
			}
		}
		else if (ChatGui.I != null)
		{
			ChatGui.I.MessageReceived();
		}
	}

	public void OnDisconnected()
	{
		Debug.LogError("Server Chanel Disconnected");
	}

	public void OnChatStateChange(ChatState state)
	{
	}

	public void OnUserSubscribed(string channel, string user)
	{
		
	}

	public void OnUserUnsubscribed(string channel, string user)
	{

	}

	public void OnUnsubscribed(string[] channels)
	{
		isConnected = false;
	}

	public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
	{
	}

	public void SendPrivateMEssage(object _message, string _userName)
	{
	}

	public void OnPrivateMessage(string sender, object message, string channelName)
	{
	}

	public void SubscribeToServerChat()
	{
		if (PhotonNetwork.room != null)
		{
			if (ChatClient.State != ChatState.ConnectedToFrontEnd)
			{
				StartCoroutine("ChatReconnection");
				return;
			}
			Debug.LogError("Subscraby to server chat  " + PhotonNetwork.room.Name);
			ServerChatChanel = PhotonNetwork.room.Name;
			ChatClient.Subscribe(new string[1] { ServerChatChanel }, 20);
		}
	}

	public void UnSubscribeServerChat()
	{
		if (!string.IsNullOrEmpty(ServerChatChanel))
		{
			ChatClient.Unsubscribe(new string[1] { ServerChatChanel });
		}
	}

	public void UserActionSendMessage(string myMessage)
	{
		SendMessageToChat(name_for_chat + myMessage, name_for_chat);
	}

	private void ParseCommand(string _chanelName, string _command)
	{
		switch (_command.Remove(0, 1))
		{
		case "clear":
			ChatClient.PublicChannels[_chanelName].ClearMessages();
			break;
		}
	}

	public void UpdateTaggedName()
	{
		name_for_chat = JsSpeeker.vk_name;
		if (!DataKeeper.IsUserDummy && DataKeeper.BackendInfo.user.has_vip)
		{
			name_for_chat += "^CFFC300FF VIP";
		}
	}

	private void RefreshFrame()
	{
		Debug.Log("Call reload Frame!");
		JsSpeeker.I.ReloadFrame();
	}

	private IEnumerator UpdateMessageOnSuscribe()
	{
		yield return new WaitForSeconds(1f);
		ChatGui.I.MessageReceived();
	}

	private void OnApplicationQuit()
	{
		ChatClient.Disconnect();
	}
}