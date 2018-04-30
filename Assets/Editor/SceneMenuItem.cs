// このファイルはSceneMenuGeneratorから自動生成されたファイルです
using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneMenuItem
{
	
	[MenuItem(@"Scene/Assets|Scenes|Group/Child/child.unity", false, 100)]
	static void OpenAssetsScenesGroupChildchildunity()
	{
		EditorSceneManager.OpenScene(@"Assets/Scenes/Group/Child/child.unity");
	}
	
	[MenuItem(@"Scene/Assets|Scenes|Group/group1.unity", false, 100)]
	static void OpenAssetsScenesGroupgroup1unity()
	{
		EditorSceneManager.OpenScene(@"Assets/Scenes/Group/group1.unity");
	}
	
	[MenuItem(@"Scene/Assets|Scenes|Group/group2.unity", false, 100)]
	static void OpenAssetsScenesGroupgroup2unity()
	{
		EditorSceneManager.OpenScene(@"Assets/Scenes/Group/group2.unity");
	}
	
	[MenuItem(@"Scene/Assets|Scenes|Scene1.unity", false, 100)]
	static void OpenAssetsScenesScene1unity()
	{
		EditorSceneManager.OpenScene(@"Assets/Scenes/Scene1.unity");
	}
	
	[MenuItem(@"Scene/Assets|Scenes|Scene2.unity", false, 100)]
	static void OpenAssetsScenesScene2unity()
	{
		EditorSceneManager.OpenScene(@"Assets/Scenes/Scene2.unity");
	}
	
	[MenuItem(@"Scene/Assets|Scenes|特殊文字=+-%&!^~#$@'`()[]{};,._.unity", false, 100)]
	static void OpenAssetsScenes特殊文字unity()
	{
		EditorSceneManager.OpenScene(@"Assets/Scenes/特殊文字=+-%&!^~#$@'`()[]{};,._.unity");
	}
	
}