using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Opencoding.Console.Editor
{
	internal static class DebugConsoleEditorSettings
	{
		// Change this if you move the Opencoding directory to a different location
		private static string _opencodingDirectoryLocation = "Assets/Opencoding";

		public static bool AutomaticallyLoadConsoleInEditor { get; private set; }

		public static string OpencodingDirectoryLocation { get { return _opencodingDirectoryLocation; } }

		static DebugConsoleEditorSettings()
		{
			AutomaticallyLoadConsoleInEditor = EditorPrefs.GetBool("TouchConsolePro/AutomaticallyLoadConsoleInEditor", true);	
		}
	}
}