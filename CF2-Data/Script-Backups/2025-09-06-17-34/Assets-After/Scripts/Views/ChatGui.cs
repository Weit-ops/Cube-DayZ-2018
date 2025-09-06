using System.Collections.Generic;
using Photon.Chat;
using UnityEngine;

public enum ChatType
{
	MainMenu = 0,
	Server = 1,
	None = 2
}

public class ChatGui : MonoBehaviour
{
	public string ChatAppId;
	public int HistoryLengthToFetch;
	public bool DemoPublishOnSubscribe;
	private ChatChannel selectedChannel;
	private int selectedChannelIndex;
	private bool doingPrivateChat;
	private string inputLine = string.Empty;
	private string userIdInput = string.Empty;
	public Vector2 scrollPos = Vector2.zero;
	private static string WelcomeText = "Welcome to chat.\\help lists commands.";
	private static string HelpText = "\n\\subscribe <list of channelnames> subscribes channels.\n\\unsubscribe <list of channelnames> leaves channels.\n\\msg <username> <message> send private message to user.\n\\clear clears the current chat tab. private chats get closed.\n\\help gets this help message.";

	public ChatType chatType = ChatType.None;

	[SerializeField] tk2dTextMesh _messages;
	[SerializeField] tk2dUIScrollableArea _scrollArea;
	[SerializeField] tk2dUITextInput _inputObject;
	[SerializeField] tk2dTextMesh _inputEmptyText;
	[SerializeField] BoxCollider inputCollider;
	[SerializeField] tk2dUIScrollbar scrollBar;

	private List<string> channels;
	private int countOfPublicChannels;
	private bool can_use_chat;

	public string selectedChannelName;
	public bool chatIsFocused;

	[SerializeField] tk2dSlicedSprite _backgound1;
	[SerializeField] tk2dSlicedSprite _backgound2;

	private Color bgr1Clr;
	private Color bgr2Clr;

	public static ChatGui I;

	[SerializeField] tk2dTextMesh _commonChatCount;

	public float test = 0.295f;
	public string UserName { get; set; }

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}

		if (chatType == ChatType.Server)
		{
			bgr1Clr = _backgound1.color;
			bgr2Clr = _backgound2.color;
		}
	}

	public void OnEnable()
	{
		can_use_chat = true;

		if (DataKeeper.GameType == GameType.Single && chatType == ChatType.Server)
		{
			base.gameObject.SetActive(false);
			return;
		}

		base.gameObject.SetActive(true);
		UpdateChatCount();

		if (chatType == ChatType.Server) {
			UpdateManager.I.SubscribeToServerChat ();
		}
	}

	private void OnDisable()
	{
	}

	private void Start()
	{
		if (!can_use_chat)
		{
			_inputEmptyText.color = new Color(255f, 255f, 255f, 255f);
		}
	}

	private void Update()
	{
		if (UpdateManager.I != null && UpdateManager.I.ChatClient == null)
		{
			return;
		}
			
		if (!can_use_chat)
		{
			if (_inputEmptyText.enabled)
			{
				_inputEmptyText.text = "^CFFC300FFЧат - только для VIP игроков";
				if (DataKeeper.Language == Language.English)
				{
					_inputEmptyText.text = "^CFFC300FFChat - only for VIP players";
				}
				_inputEmptyText.Commit();
				inputCollider.enabled = false;
			}
		}
		else if (_inputEmptyText.enabled)
		{
			_inputEmptyText.text = "^CFFFFFFFFНажмите Enter для отправки сообщения";
			if (DataKeeper.Language == Language.English)
			{
				_inputEmptyText.text = "^CFFC300FFPress Enter to sent message";
			}
			_inputEmptyText.Commit();
			inputCollider.enabled = true;
		}
		if ((ControlFreak2.CF2Input.GetKeyUp(KeyCode.KeypadEnter) || ControlFreak2.CF2Input.GetKeyUp(KeyCode.Return)) && can_use_chat)
		{
			if (!_inputObject.IsFocus)
			{
				chatIsFocused = true;
				if (chatType == ChatType.Server)
				{
					GameControls.I.MenuControls(true);
					Color color = bgr1Clr;
					color.a = 255f;
					_backgound1.color = color;
					Color color2 = bgr2Clr;
					color2.a = 255f;
					_backgound2.color = color2;
				}
				_inputObject.SetFocus(true);
			}
			else
			{
				if (chatType == ChatType.Server)
				{
					GameControls.I.MenuControls(false);
					_inputObject.SetFocus(false);
					chatIsFocused = false;
					_backgound1.color = bgr1Clr;
					_backgound2.color = bgr2Clr;
				}
				if (chatType == ChatType.Server && PhotonNetwork.room != null)
				{
					UpdateManager.I.SendMessageToChat(UpdateManager.ServerChatChanel, string.Concat("^CE5B05AFF[", JsSpeeker.vk_name, "^CE5B05AFF]: ^CFFFFFFFF", _inputObject.Text));
				}
				if (chatType == ChatType.MainMenu)
				{
					UpdateManager.I.SendMessageToChat(UpdateManager.CommonChatChanel, string.Concat("^CE5B05AFF[", JsSpeeker.vk_name, "^CE5B05AFF]: ^CFFFFFFFF", _inputObject.Text));
				}
				_inputObject.Text = string.Empty;
			}
		}
		if (countOfPublicChannels == 0 && UpdateManager.I.ChatClient.State == ChatState.ConnectedToFrontEnd && UpdateManager.I.ChatClient.PublicChannels != null)
		{
			channels = new List<string>(UpdateManager.I.ChatClient.PublicChannels.Keys);
			countOfPublicChannels = channels.Count;
			channels.AddRange(UpdateManager.I.ChatClient.PrivateChannels.Keys);
			if (channels.Count > 0)
			{
				selectedChannelName = channels[selectedChannelIndex];
			}
		}
	}

	public void MessageReceived()
	{
		string channelName = UpdateManager.CommonChatChanel;
		if (chatType == ChatType.Server)
		{
			channelName = UpdateManager.ServerChatChanel;
		}
		if (!UpdateManager.I.ChatClient.TryGetChannel(channelName, false, out selectedChannel))
		{
			return;
		}
		string text = string.Empty;
		_scrollArea.ContentLength = 0f;
		_messages.Commit();
		for (int i = 0; i < selectedChannel.Messages.Count; i++)
		{
			object obj = selectedChannel.Messages[i];
			text += string.Concat((string)obj , "\n");
		}
		_messages.text = text;
		_messages.Commit();
		_scrollArea.ContentLength = GetContentLength();
		tk2dUtil.SetDirty(_scrollArea);
		scrollBar.Value = float.MaxValue;
		scrollPos.y = float.MaxValue;
	}

	public void UpdateChatCount()
	{
		if (chatType == ChatType.MainMenu)
		{
			_commonChatCount.text = "(" + PhotonNetwork.countOfPlayersOnMaster + ")";
		}
		else if (chatType == ChatType.Server)
		{
			_commonChatCount.text = "(" + PhotonNetwork.room.PlayerCount + ")";
		}
	}

	private float GetContentLength(){
		Bounds b = GetRendererBoundsInChildren( _scrollArea.contentContainer.transform, _scrollArea.contentContainer.transform );
		b.Encapsulate(Vector3.zero);
		return ((_scrollArea.scrollAxes == tk2dUIScrollableArea.Axes.XAxis) ? b.size.x : b.size.y) * 1.02f;
	}

	static readonly Vector3[] boxExtents = new Vector3[] {
		new Vector3(-1, -1, -1),
		new Vector3( 1, -1, -1),
		new Vector3(-1,  1, -1),
		new Vector3( 1,  1, -1),
		new Vector3(-1, -1,  1),
		new Vector3( 1, -1,  1),
		new Vector3(-1,  1,  1),
		new Vector3( 1,  1,  1),
	};

	static void GetRendererBoundsInChildren(Matrix4x4 rootWorldToLocal, Vector3[] minMax, HashSet<Transform> ignoreItems, Transform t, bool includeAllChildren) {
		if (!ignoreItems.Contains(t)) {
			MeshFilter mf = t.GetComponent<MeshFilter>();
			if (mf != null && mf.sharedMesh != null) {
				Bounds b = mf.sharedMesh.bounds;
				Matrix4x4 relativeMatrix = rootWorldToLocal * t.localToWorldMatrix;
				for (int j = 0; j < 8; ++j) {
					Vector3 localPoint = b.center + Vector3.Scale(b.extents, boxExtents[j]);
					Vector3 pointRelativeToRoot = relativeMatrix.MultiplyPoint(localPoint);
					minMax[0] = Vector3.Min(minMax[0], pointRelativeToRoot);
					minMax[1] = Vector3.Max(minMax[1], pointRelativeToRoot);
				}
			}
			for (int i = 0; i < t.childCount; ++i) {
				Transform child = t.GetChild(i);

				if (!includeAllChildren && child.GetComponent<Collider>() != null) {
					continue;
				}

				GetRendererBoundsInChildren(rootWorldToLocal, minMax, ignoreItems, child, includeAllChildren);
			}
		}
	}

	static Bounds GetRendererBoundsInChildren(Transform root, HashSet<Transform> ignoreItems, Transform transform, bool includeAllChildren) {
		Vector3 vector3Min = new Vector3(float.MinValue, float.MinValue, float.MinValue);
		Vector3 vector3Max = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
		Vector3[] minMax = new Vector3[] {
			vector3Max,
			vector3Min
		};

		GetRendererBoundsInChildren( root.worldToLocalMatrix, minMax, ignoreItems, transform, includeAllChildren);

		Bounds b = new Bounds();
		if (minMax[0] != vector3Max && minMax[1] != vector3Min) {
			b.SetMinMax(minMax[0], minMax[1]);
		}
		return b;
	}

	public static Bounds GetRendererBoundsInChildren(Transform root, Transform transform) {
		return GetRendererBoundsInChildren(root, new HashSet<Transform>(), transform, true);
	}
}