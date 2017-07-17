using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;


namespace SceneMenu
{
	public class SceneMenuSettings : ScriptableObject
	{
		public const string kDefaultDerectory = "Assets/Editor";

		public string outputDirectoryPath;
		public List<string> targets = new List<string>();
		public List<string> ignores = new List<string>();
		public List<string> grouping = new List<string>();


		//------------------------------------------------------
		// accessor
		//------------------------------------------------------

		public bool IsIgnorePath(string assetPath)
		{
			return ignores.FindIndex(i => assetPath.StartsWith(i)) >= 0;
		}


		//------------------------------------------------------
		// i/o
		//------------------------------------------------------

		public static SceneMenuSettings Load()
		{
			var GUIDs = AssetDatabase.FindAssets("SceneMenuSettings t:SceneMenuSettings");
			if (GUIDs.Length == 0)
			{
				if (!Directory.Exists("Assets/Editor Default Resources"))
					Directory.CreateDirectory("Assets/Editor Default Resources");

				var instance = ScriptableObject.CreateInstance<SceneMenuSettings>();
				AssetDatabase.CreateAsset(instance, "Assets/Editor Default Resources/SceneMenuSettings.asset");
				return instance;
			}

			return AssetDatabase.LoadAssetAtPath<SceneMenuSettings>(
				AssetDatabase.GUIDToAssetPath(GUIDs[0]));
		}

		public void Save()
		{
			EditorUtility.SetDirty(this);
		}
	}
}
