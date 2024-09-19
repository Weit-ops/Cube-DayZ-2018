using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class ObjectsStatistic : MonoBehaviour
{
	public enum Mode
	{
		Console = 0
	}

	private static Dictionary<Type, List<UnityEngine.Object>> unityObjects = new Dictionary<Type, List<UnityEngine.Object>>();

	private static Dictionary<Type, List<PropertyInfo>> properties = new Dictionary<Type, List<PropertyInfo>>();

	private static Dictionary<Type, List<FieldInfo>> fields = new Dictionary<Type, List<FieldInfo>>();

	private List<GameObject> gos = new List<GameObject>();

	private bool _showDebugWindow = true;

	private Vector2 _debugScroll;

	private Vector2 _gosScroll;

	private Vector2 _inspectorScroll;

	private readonly List<string> _debugMessages = new List<string>();

	public static ObjectsStatistic instance;

	[HideInInspector]
	public bool show_console;

	private List<GameObject> foldoutGameObjects = new List<GameObject>();

	private Mode mode;

	private string m_consoleComm = string.Empty;

	private GameObject current;

	public int ConsoleLength
	{
		get
		{
			return _debugMessages.Count;
		}
	}

	private void LogErrors(string condition, string stacktrace, LogType type)
	{
		_debugMessages.Insert(0, condition + "\r\n" + stacktrace);
	}

	public static void Log(string message)
	{
		if (instance != null)
		{
			instance._debugMessages.Add(message);
		}
		else
		{
			Debug.Log(message);
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			if (GameControls.I != null)
			{
				GameControls.I.MenuControls(!show_console);
			}
			show_console = !show_console;
		}
	}

	public void RegisterLog()
	{
		Application.logMessageReceived += LogErrors;
	}

	private IEnumerator Start()
	{
		if (instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			yield break;
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	public void OnGUI()
	{
		if (!show_console)
		{
			return;
		}
		GUI.skin = null;
		GUI.depth = -20;
		if (_showDebugWindow)
		{
			Rect screenRect = new Rect(10f, 100f, Screen.width - 20, Screen.height - 200);
			GUILayout.BeginArea(screenRect, GUI.skin.box);
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Console"))
			{
				mode = Mode.Console;
			}
			if (GUILayout.Button("Copy log"))
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string debugMessage in _debugMessages)
				{
					stringBuilder.Append(debugMessage);
					stringBuilder.Append(Environment.NewLine);
					stringBuilder.Append(Environment.NewLine);
					stringBuilder.Append(Environment.NewLine);
				}
				string text = stringBuilder.ToString();
				TextEditor textEditor = new TextEditor();
				textEditor.content = new GUIContent(text);
				textEditor.SelectAll();
				textEditor.Copy();
			}
			GUILayout.EndHorizontal();
			if (mode == Mode.Console)
			{
				_debugScroll = GUILayout.BeginScrollView(_debugScroll);
				foreach (string debugMessage2 in _debugMessages)
				{
					Color color = GUI.color;
					if (debugMessage2.Contains("Debug:LogError") || debugMessage2.Contains("Exception"))
					{
						GUI.color = Color.red;
						GUILayout.TextField(debugMessage2);
					}
					else if (debugMessage2.Contains("Warning"))
					{
						GUI.color = Color.yellow;
						GUILayout.TextField(debugMessage2);
					}
					else
					{
						GUI.color = Color.green;
						GUILayout.TextField(debugMessage2);
					}
					GUI.color = color;
				}
				GUILayout.EndScrollView();
			}
			GUILayout.EndArea();
		}
		m_consoleComm = GUI.TextField(new Rect(0f, 0f, 1f, 1f), m_consoleComm);
		if (m_consoleComm.LastIndexOf("~") > 0 || m_consoleComm.LastIndexOf("`") > 0)
		{
			show_console = false;
			base.enabled = false;
			m_consoleComm = string.Empty;
		}
	}

	private void DrawInspector()
	{
		if (current == null)
		{
			return;
		}
		Component[] components = current.GetComponents<Component>();
		Component[] array = components;
		foreach (Component component in array)
		{
			Type type = component.GetType();
			List<PropertyInfo> list = properties[type];
			List<FieldInfo> list2 = fields[type];
			GUILayout.BeginHorizontal(GUI.skin.box);
			GUILayout.Label(type.Name);
			GUILayout.EndHorizontal();
			foreach (FieldInfo item in list2)
			{
				try
				{
					GUILayout.BeginHorizontal();
					GUILayout.Space(20f);
					GUILayout.Label(item.Name, GUILayout.Width(200f));
					object value = item.GetValue(component);
					GUILayout.Label((value != null) ? value.ToString() : "null");
					GUILayout.EndHorizontal();
					if (value == null || !(value is IEnumerable) || value is string)
					{
						continue;
					}
					foreach (object item2 in value as IEnumerable)
					{
						GUILayout.BeginHorizontal();
						GUILayout.Space(40f);
						GUILayout.Label((item2 != null) ? item2.ToString() : "null");
						GUILayout.EndHorizontal();
					}
				}
				catch
				{
				}
			}
			foreach (PropertyInfo item3 in list)
			{
				try
				{
					if (item3.GetIndexParameters().Length != 0)
					{
						continue;
					}
					GUILayout.BeginHorizontal();
					GUILayout.Space(20f);
					GUILayout.Label(item3.Name, GUILayout.Width(200f));
					object value2 = item3.GetValue(component, null);
					GUILayout.Label((value2 != null) ? value2.ToString() : "null");
					GUILayout.EndHorizontal();
					if (value2 == null || !(value2 is IEnumerable) || value2 is string)
					{
						continue;
					}
					foreach (object item4 in value2 as IEnumerable)
					{
						GUILayout.BeginHorizontal();
						GUILayout.Space(40f);
						GUILayout.Label((item4 != null) ? item4.ToString() : "null");
						GUILayout.EndHorizontal();
					}
				}
				catch
				{
				}
			}
		}
	}

	private void DrawGameObjects()
	{
		foreach (GameObject go in gos)
		{
			DrawGameObject(go);
		}
	}

	private void DrawGameObject(GameObject go, int level = 0)
	{
		if (go == null)
		{
			return;
		}
		bool flag = foldoutGameObjects.Contains(go);
		if (current == go)
		{
			GUILayout.BeginHorizontal(GUI.skin.box);
		}
		else
		{
			GUILayout.BeginHorizontal();
		}
		GUILayout.Label(string.Empty, GUILayout.Width(level * 10));
		if (GUILayout.Button((go.transform.childCount == 0) ? string.Empty : ((!flag) ? ">" : "v"), GUI.skin.label, GUILayout.Width(16f)))
		{
			if (flag)
			{
				foldoutGameObjects.Remove(go);
			}
			else
			{
				foldoutGameObjects.Add(go);
			}
		}
		if (GUILayout.Button(go.name, GUI.skin.label, GUILayout.ExpandWidth(true)))
		{
			current = go;
		}
		GUILayout.EndHorizontal();
		if (!flag)
		{
			return;
		}
		foreach (Transform item in go.transform)
		{
			DrawGameObject(item.gameObject, level + 1);
		}
	}
}
