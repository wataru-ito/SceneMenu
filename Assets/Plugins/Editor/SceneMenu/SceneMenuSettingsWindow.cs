using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace SceneMenu
{
	public class SceneMenuSettingsWindow : EditorWindow
	{
		const float kItemHeight = 18f;

		readonly string[] kTab =
		{
			"限定",
			"除外",
			"階層",
		};

		SceneMenuSettings m_origin;
		SceneMenuSettings m_settings;

		GUIStyle m_labelStyle;
		DefaultAsset m_output;
		int m_tab;
		Vector2 m_scrollPosition;
		Rect m_scrollRect;
		List<string> m_current;
		int m_selectedIndex = -1; // 複数選択できる必要もない


		//------------------------------------------------------
		// static function
		//------------------------------------------------------

		static SceneMenuSettingsWindow s_intance;

		[MenuItem("Scene/設定", false, 0)]
		public static SceneMenuSettingsWindow Open()
		{
			if (!s_intance)
			{
				s_intance = CreateInstance<SceneMenuSettingsWindow>();
				s_intance.ShowUtility();
			}

			return s_intance;
		}


		//------------------------------------------------------
		// unity system function
		//------------------------------------------------------

		void OnEnable()
		{
			s_intance = this; // 表示状態で生成モード切替挟んだ時の対応

			titleContent = new GUIContent("SceneMenu設定");

			m_origin = SceneMenuSettings.Load();
			Revert();
		}

		void OnGUI()
		{
			if (m_labelStyle == null)
			{
				m_labelStyle = GUI.skin.FindStyle("Hi Label");
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Apply", "ButtonLeft"))
				{
					Apply();
				}
				if (GUILayout.Button("Revert", "ButtonRight"))
				{
					Revert();
				}
			}

			GUILayout.Space(8);

			EditorGUI.BeginChangeCheck();
			m_output = EditorGUILayout.ObjectField("出力", m_output, typeof(DefaultAsset), false) as DefaultAsset;
			if (EditorGUI.EndChangeCheck())
			{
				m_settings.outputDirectoryPath = m_output ? AssetDatabase.GetAssetPath(m_output) : string.Empty;
			}

			DrawList();
			ProcessDragAndDrop();
		}


		//------------------------------------------------------
		// gui
		//------------------------------------------------------

		void DrawList()
		{
			EditorGUI.BeginChangeCheck();
			m_tab = GUILayout.Toolbar(m_tab, kTab);
			if (EditorGUI.EndChangeCheck() || m_current == null)
			{
				m_current = GetList();
				m_selectedIndex = -1;
				Selection.activeObject = null;
			}

			GUILayout.Space(-5);
			GUILayout.Box(GUIContent.none,
				GUILayout.ExpandWidth(true),
				GUILayout.ExpandHeight(true));

			m_scrollRect = GUILayoutUtility.GetLastRect();
			// この後描画されるbackgrounで枠線が消えてしまうので削る
			m_scrollRect.x += 1f;
			m_scrollRect.y += 1f;
			m_scrollRect.width -= 2f;
			m_scrollRect.height -= 2f;

			DrawBackground();

			var viewRect = new Rect(0, 0, m_scrollRect.width, m_current.Count * kItemHeight);
			using (var scroll = new GUI.ScrollViewScope(m_scrollRect, m_scrollPosition, viewRect))
			{
				var itemPosition = new Rect(0, 0, viewRect.width, kItemHeight);
				for (int i = 0; i < m_current.Count; ++i)
				{
					if (DrawFolder(itemPosition, m_current[i], m_selectedIndex == i))
					{
						if (m_selectedIndex >= i)
						{
							--m_selectedIndex;
						}
						m_current.RemoveAt(i--);
					}

					itemPosition.y += itemPosition.height;
				}

				// アイテム描画した後に更新しないとDrawBackground()とズレる
				m_scrollPosition = scroll.scrollPosition;
			}

			GUILayout.Space(-5);
			EditorGUILayout.HelpBox("フォルダをドロップで追加", MessageType.Info);
		}

		void DrawBackground()
		{
			var prev = GUI.color;
			var gray = new Color(prev.r * 0.95f, prev.g * 0.95f, prev.b * 0.95f);
			var style = GUI.skin.FindStyle("CN EntryBackOdd");

			float y = m_scrollRect.yMin - m_scrollPosition.y;
			for (int i = 0; y < m_scrollRect.yMax; ++i, y += kItemHeight)
			{
				if (y + kItemHeight <= m_scrollRect.yMin) continue;
				if (y >= m_scrollRect.yMax) continue;

				var itemPisition = new Rect(m_scrollRect.x,
					Mathf.Max(y, m_scrollRect.y),
					m_scrollRect.width,
					Mathf.Min(kItemHeight, m_scrollRect.yMax - y));

				GUI.color = i % 2 == 1 ? prev : gray;
				GUI.Box(itemPisition, GUIContent.none, style);
			}

			GUI.color = prev;
		}

		bool DrawFolder(Rect itemPosition, string folder, bool selected)
		{
			var styleState = GetStyleState(selected);
			if (styleState.background)
				GUI.DrawTexture(itemPosition, styleState.background);

			const float kBtnWidth = 46f;
			const float kPadding = 2f;

			var r = itemPosition;
			r.width -= kBtnWidth + kPadding;
			EditorGUI.LabelField(r, folder);

			r.x += r.width;
			r.width = kBtnWidth;
			r.y += 1f;
			r.height -= 2f;
			return GUI.Button(r, "削除");
		}

		GUIStyleState GetStyleState(bool selected)
		{
			if (selected)
				return EditorWindow.focusedWindow == this ? m_labelStyle.onActive : m_labelStyle.onNormal;
			return m_labelStyle.normal;
		}


		//------------------------------------------------------
		// events
		//------------------------------------------------------

		void ProcessDragAndDrop()
		{
			var ev = Event.current;
			int controlId = GUIUtility.GetControlID(FocusType.Passive);
			switch (ev.type)
			{
				case EventType.DragUpdated:
				case EventType.DragPerform:
					if (!m_scrollRect.Contains(ev.mousePosition)) break;

					DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					DragAndDrop.activeControlID = controlId;

					if (ev.type == EventType.DragPerform)
					{
						DragAndDrop.AcceptDrag();

						var dropFiles = DragAndDrop.objectReferences
							.Where(i => i is DefaultAsset)
							.Select(i => AssetDatabase.GetAssetPath(i));

						OnDrop(dropFiles);

						DragAndDrop.activeControlID = 0;
					}
					ev.Use();
					break;

				case EventType.MouseDown:
					if (m_scrollRect.Contains(ev.mousePosition))
					{
						OnSelected(ev);
					}
					ev.Use();
					break;
			}
		}

		void OnDrop(IEnumerable<string> folders)
		{
			var list = GetList();

			foreach (var folder in folders)
			{
				if (!list.Contains(folder))
				{
					list.Add(folder);
				}
			}

			list.Sort();
		}

		void OnSelected(Event ev)
		{
			var index = Mathf.FloorToInt((ev.mousePosition.y - m_scrollRect.y + m_scrollPosition.y) / kItemHeight);
			if (index >= m_current.Count)
			{
				m_selectedIndex = -1;
				Selection.activeGameObject = null;
				return;
			}

			m_selectedIndex = index;
			Selection.activeObject = AssetDatabase.LoadAssetAtPath<DefaultAsset>(m_current[index]);
		}



		//------------------------------------------------------
		// settings
		//------------------------------------------------------

		void Apply()
		{
			if (m_settings.Save())
			{
				if (m_settings.outputDirectoryPath != m_origin.outputDirectoryPath)
				{
					var filePath = m_origin.GetScriptPath();
					if (File.Exists(filePath))
					{
						File.Delete(filePath);
					}
				}

				m_origin = new SceneMenuSettings(m_settings);
			}

			SceneMenuUpdater.GenerateSceneMenu();
		}

		void Revert()
		{
			m_settings = new SceneMenuSettings(m_origin);
			m_current = GetList();

			if (!string.IsNullOrEmpty(m_settings.outputDirectoryPath))
			{
				m_output = AssetDatabase.LoadAssetAtPath<DefaultAsset>(m_settings.outputDirectoryPath);
			}

			Repaint();
		}


		//------------------------------------------------------
		// folder list
		//------------------------------------------------------

		List<string> GetList()
		{
			switch (m_tab)
			{
				default:
				case 0: return m_settings.targets;
				case 1: return m_settings.ignores;
				case 2: return m_settings.grouping;
			}
		}
	}
}
