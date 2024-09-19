using System;
using System.Collections.Generic;
using Opencoding.CommandHandlerSystem;
using Opencoding.Console;
using UnityEngine;

public class DebugConsoleController : MonoBehaviour
{
	public static DebugConsoleController I;

	[SerializeField] DebugConsole _console;

	public bool IsConsoleEnable;

	[CommandHandler]
	public static void home()
	{
		Debug.Log("_home console command ");
	}

	[CommandHandler]
	public static void say(string text)
	{
		Debug.Log("_say console command " + text);
	}

	[CommandHandler]
	public static void trail(int num)
	{
		Debug.Log("_trail console command " + num);
	}

	[CommandHandler]
	private static void kick([Autocomplete(typeof(DebugConsoleController), "KickPlayerParam")] string playerName)
	{
		Debug.Log("_kick console command " + playerName);
	}

	public static IEnumerable<string> KickPlayerParam()
	{
		return new string[2] { "Petya", "Vasya" };
	}

	[CommandHandler]
	public static void tester_power()
	{
		Debug.Log("_tester_power console command");
	}

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}

		CommandHandlers.RegisterCommandHandlers(typeof(DebugConsoleController));
	}

	private bool OnOpenConsole()
	{
		IsConsoleEnable = true;
		if (GameControls.I != null)
		{
			GameControls.I.MenuControls(true);
		}
		return true;
	}

	private bool OnConsoleClose()
	{
		IsConsoleEnable = false;
		if (GameControls.I != null)
		{
			GameControls.I.MenuControls(false);
		}
		return true;
	}

	public void ShowConsole(bool show)
	{
		DebugConsole.IsVisible = show;
	}
}
