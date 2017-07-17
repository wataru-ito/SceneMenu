// このファイルはSceneMenuGeneratorから自動生成されたファイルです
using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneMenuItem
{
	
	[MenuItem("Scene/Assets|Scenes|Group/Child/child.unity", false, 10)]
	static void OpenAssetsScenesGroupChildchild()
	{
		EditorSceneManager.OpenScene("Assets/Scenes/Group/Child/child.unity");
	}
	
	[MenuItem("Scene/Assets|Scenes|Group/group1.unity", false, 10)]
	static void OpenAssetsScenesGroupgroup1()
	{
		EditorSceneManager.OpenScene("Assets/Scenes/Group/group1.unity");
	}
	
	[MenuItem("Scene/Assets|Scenes|Group/group2.unity", false, 10)]
	static void OpenAssetsScenesGroupgroup2()
	{
		EditorSceneManager.OpenScene("Assets/Scenes/Group/group2.unity");
	}
	
	[MenuItem("Scene/Assets|Scenes|Scene1.unity", false, 10)]
	static void OpenAssetsScenesScene1()
	{
		EditorSceneManager.OpenScene("Assets/Scenes/Scene1.unity");
	}
	
	[MenuItem("Scene/Assets|Scenes|Scene2.unity", false, 10)]
	static void OpenAssetsScenesScene2()
	{
		EditorSceneManager.OpenScene("Assets/Scenes/Scene2.unity");
	}
	
}