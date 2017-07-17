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
			// 新規追加時はimportedAssetsなんだけど入れちゃうと頻度高いので抜く
			//if (ContainsScene(importedAssets) ||
			if (ContainsScene(deletedAssets) ||
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

		[MenuItem("Scene/更新", false, 0)]
		static void GenerateSceneMenu()
		{
			s_queued = false;

			var generator = new SceneMenuGenerator();
			generator.Generate();
		}
	}
}