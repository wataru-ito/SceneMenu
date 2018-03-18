using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;


namespace SceneMenu
{
	[Serializable]
	class SceneMenuSettings
	{
		const string kSettingFilePath = "ProjectSettings/SceneMenuSettings.txt";
		public const string kDefaultDerectory = "Assets/Editor";

		public string outputDirectoryPath;
		public List<string> targets;
		public List<string> ignores;
		public List<string> grouping;


		//------------------------------------------------------
		// lifetime
		//------------------------------------------------------

		private SceneMenuSettings()
		{
			outputDirectoryPath = string.Empty;
			targets = new List<string>();
			ignores = new List<string>();
			grouping = new List<string>();
		}

		public SceneMenuSettings(SceneMenuSettings src)
		{
			outputDirectoryPath = src.outputDirectoryPath;
			targets = new List<string>(src.targets);
			ignores = new List<string>(src.ignores);
			grouping = new List<string>(src.grouping);
		}


		//------------------------------------------------------
		// accessor
		//------------------------------------------------------

		public bool IsIgnorePath(string assetPath)
		{
			return ignores.FindIndex(i => assetPath.StartsWith(i)) >= 0;
		}

		public string GetScriptPath()
		{
			return string.Format("{0}/SceneMenuCode.cs",
				string.IsNullOrEmpty(outputDirectoryPath) ? kDefaultDerectory : outputDirectoryPath);
		}


		//------------------------------------------------------
		// i/o
		//------------------------------------------------------

		public static SceneMenuSettings Load()
		{
			var settings = new SceneMenuSettings();
			settings.Revert();
			return settings;
		}

		public void Revert()
		{
			try
			{
				if (File.Exists(kSettingFilePath))
				{
					var json = File.ReadAllText(kSettingFilePath);
					EditorJsonUtility.FromJsonOverwrite(json, this);
				}
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
			}
		}

		public bool Save()
		{
			try
			{
				var json = EditorJsonUtility.ToJson(this);
				File.WriteAllText(kSettingFilePath, json);
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				return false;
			}

			return true;
		}
	}
}
