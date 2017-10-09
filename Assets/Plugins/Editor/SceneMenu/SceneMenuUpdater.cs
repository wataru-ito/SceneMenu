using UnityEngine;
using UnityEditor;
using System;

namespace SceneMenu
{
	public class SceneMenuUpdater : AssetPostprocessor
	{
		static bool s_queued;

		//------------------------------------------------------
		// unity system function
		//------------------------------------------------------

		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			if (ContainsScene(importedAssets) ||
				ContainsScene(deletedAssets) ||
				ContainsScene(movedAssets))
			{
				if (!s_queued)
				{
					s_queued = true;
					EditorApplication.delayCall += GenerateSceneMenu;
				}
			}
		}

		static bool ContainsScene(string[] assetPaths)
		{
			return Array.FindIndex(assetPaths, i => i.EndsWith(".unity")) >= 0;
		}


		//------------------------------------------------------
		// menu
		//------------------------------------------------------

		[MenuItem("Scene/更新", false, 1)]
		public static void GenerateSceneMenu()
		{
			s_queued = false;

			var generator = new SceneMenuGenerator();
			generator.Generate();
		}
	}
}