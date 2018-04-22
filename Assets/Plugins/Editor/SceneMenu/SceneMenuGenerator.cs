using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace SceneMenu
{
	class SceneMenuGenerator
	{
		SceneMenuSettings m_settings;


		//------------------------------------------------------
		// accessor
		//------------------------------------------------------

		public void Generate()
		{
			m_settings = SceneMenuSettings.Load();

			var code = GenerateCode();
			if (string.IsNullOrEmpty(code))
				return;

			if (!WriteCode(code))
				return;

			AssetDatabase.Refresh();
		}


		//------------------------------------------------------
		// scene
		//------------------------------------------------------

		IEnumerable<string> GetTargetScenes()
		{
			return AssetDatabase.FindAssets("t:scene", m_settings.targets.ToArray())
				.Select(i => AssetDatabase.GUIDToAssetPath(i))
				.Where(i => !m_settings.IsIgnorePath(i));
		}


		//------------------------------------------------------
		// code generate
		//------------------------------------------------------

		string GenerateCode()
		{
			var template = SceneMenuCode.LoadTempate();
			if (template == null)
				return null;

			var code = new System.Text.StringBuilder();
			foreach (var assetPath in GetTargetScenes())
			{
				var itemCode = template.GenerateItemCode(
					ToFuncName(assetPath),
					ToMenuItemPath(assetPath),
					assetPath, 
					100);

				code.Append(itemCode);
			}

			return template.GenerateCode(code.ToString());
		}

		static string ToFuncName(string assetPath)
		{
			// アセット名には使えて関数名には使えない文字
			// > アセットに使えない文字 /?<>:*|"
			// > アセット名に使えた文字 +-%&~^!#$'_.,@`;()[]{}
			const string kFuncNameRemovePattern = @"[=\+\-%&~\^!#$'_\.,@\`;\(\)\[\]\{\}]+";

			var funcName = assetPath.Replace("/", "_");
			return Regex.Replace(funcName, kFuncNameRemovePattern, "");
		}

		string ToMenuItemPath(string assetPath)
		{	
			var menuPath = assetPath.Replace('/', '|'); // '|'以前の文字は表示されないっぽい
			foreach (var group in m_settings.grouping.Where(i => assetPath.StartsWith(i) && assetPath[i.Length] == '/'))
			{
				// 1文字入れ替えたいだけなんだけどなあ。もっと軽い方法...
				menuPath = string.Format("{0}/{1}",
					menuPath.Remove(group.Length),
					menuPath.Substring(group.Length + 1));
			}
			return "Scene/" + menuPath;
		}


		//------------------------------------------------------
		// file
		//------------------------------------------------------

		bool WriteCode(string code)
		{
			try
			{
				var assetPath = m_settings.GetScriptPath();

				var dir = Path.GetDirectoryName(assetPath);
				if (!Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}

				if (File.Exists(assetPath) && File.ReadAllText(assetPath) == code)
				{
					Debug.Log("SceneMenu generate skipped");
					return false;
				}

				File.WriteAllText(assetPath, code);
				Debug.LogFormat("SceneMenu generate [{0}]", assetPath);

				return true;
			}
			catch (Exception e)
			{
				Debug.LogError(e.Message);
				return false;
			}
		}
	}
}
