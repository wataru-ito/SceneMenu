using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SceneMenu
{
	public class SceneMenuGenerator
	{
		class CodeTemplate
		{
			const string kTagBegin = "##MENU_LIST_BEGIN##";
			const string kTagEnd = "##MENU_LIST_END##";
			const string kTagMenuName = "##MENU_NAME##";
			const string kTagMenuPath = "##MENU_ITEM_PATH##";
			const string kTagAssetPath = "##ASSET_PATH##";
			const string kTagPriority = "##PRIORITY##";

			string m_body;
			int m_itemInsertIndex;
			string m_item;

			public CodeTemplate(string raw)
			{
				var begin = raw.IndexOf(kTagBegin);
				if (begin < 0)
				{
					throw new Exception(string.Format("SceneMenuCodeTemplate need tag[{0}]", kTagBegin));
				}

				var end = raw.IndexOf(kTagEnd);
				if (end < 0)
				{
					throw new Exception(string.Format("SceneMenuCodeTemplate need tag[{0}]", kTagEnd));
				}

				m_body = raw.Remove(begin, end+kTagEnd.Length-begin);
				m_itemInsertIndex = begin;

				var beginTail = begin + kTagBegin.Length;
				m_item = raw.Substring(beginTail, end-beginTail);
			}

			public string GenerateCode(string itemCode)
			{
				return m_body.Insert(m_itemInsertIndex, itemCode);
			}

			public string GenerateItemCode(string funcName, string menuPath, string assetPath, int priority = 10)
			{
				return m_item.Replace(kTagMenuName, funcName)
					.Replace(kTagMenuPath, menuPath)
					.Replace(kTagAssetPath, assetPath)
					.Replace(kTagPriority, priority.ToString());
			}
		}

		SceneMenuSettings m_settings;


		//------------------------------------------------------
		// accessor
		//------------------------------------------------------

		public void Generate()
		{
			m_settings = SceneMenuSettings.Load();

			var scenePaths = GetTargetScenes();
			var code = CodeGenerate(scenePaths);
			WriteFile(code);
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

		string CodeGenerate(IEnumerable<string> assetPaths)
		{
			var template = LoadTempate();

			var itemCode = new System.Text.StringBuilder();
			foreach (var assetPath in assetPaths)
			{
				var code = template.GenerateItemCode(
					ToFuncName(assetPath),
					ToMenuItemPath(assetPath),
					assetPath);

				itemCode.Append(code);
			}

			return template.GenerateCode(itemCode.ToString());
		}

		CodeTemplate LoadTempate()
		{
			var GUIDs = AssetDatabase.FindAssets("SceneMenuCodeTemplate t:TextAsset");
			if (GUIDs.Length == 0)
			{
				throw new Exception("SceneMenuCodeTemplate not found");
			}

			var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(GUIDs[0]));
			return new CodeTemplate(textAsset.text);
		}

		static string ToFuncName(string assetPath)
		{
			var funcName = assetPath.Replace("/", "_");
			return new System.Text.RegularExpressions.Regex(@"[^0-9a-zA-Z_]+").Replace(funcName, "");
		}

		string ToMenuItemPath(string assetPath)
		{
			var menuPath = assetPath.Replace('/', '|');
			foreach (var group in m_settings.grouping)
			{
				if (assetPath.StartsWith(group))
				{
					// 1文字入れ替えたいだけなんだけどなあ。もっと軽い方法...
					menuPath = string.Format("{0}/{1}",
						menuPath.Remove(group.Length),
						menuPath.Substring(group.Length + 1));
				}
			}

			return "Scene/" + menuPath;
		}


		//------------------------------------------------------
		// file
		//------------------------------------------------------

		void WriteFile(string code)
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
				return;
			}

			File.WriteAllText(assetPath, code);

			Debug.LogFormat("SceneMenu generate [{0}]", assetPath);
		}
	}
}
