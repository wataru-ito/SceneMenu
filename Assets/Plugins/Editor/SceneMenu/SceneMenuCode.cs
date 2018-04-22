using System;
using UnityEngine;
using UnityEditor;

namespace SceneMenu
{
	class SceneMenuCode
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


		//------------------------------------------------------
		// static function
		//------------------------------------------------------

		public static SceneMenuCode LoadTempate()
		{
			var GUIDs = AssetDatabase.FindAssets("SceneMenuCodeTemplate t:TextAsset");
			if (GUIDs.Length == 0)
			{
				EditorUtility.DisplayDialog("SceneMenuコード生成", 
					"SceneMenuCodeTemplate.txt が見つかりませんでした。\nファイル確認の上、Reimportしてみてください。\nその後もう一度お試しください", 
					"確認");
				return null;
			}

			var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(GUIDs[0]));
			return new SceneMenuCode(textAsset.text);
		}


		//------------------------------------------------------
		// accessor
		//------------------------------------------------------
		
		public SceneMenuCode(string raw)
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

			m_body = raw.Remove(begin, end + kTagEnd.Length - begin);
			m_itemInsertIndex = begin;

			var beginTail = begin + kTagBegin.Length;
			m_item = raw.Substring(beginTail, end - beginTail);
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
}