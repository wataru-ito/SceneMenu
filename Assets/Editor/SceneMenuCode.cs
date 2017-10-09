// このファイルはSceneMenuGeneratorから自動生成されたファイルです
using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneMenuItem
{
	
	[MenuItem(@"Scene/Assets|Scenes|Group/Child/child.unity", false, 10)]
	static void OpenAssets_Scenes_Group_Child_childunity()
	{
		EditorSceneManager.OpenScene(@"Assets/Scenes/Group/Child/child.unity");
	}
	
	[MenuItem(@"Scene/Assets|Scenes|Group/group1.unity", false, 10)]
	static void OpenAssets_Scenes_Group_group1unity()
	{
		EditorSceneManager.OpenScene(@"Assets/Scenes/Group/group1.unity");
	}
	
	[MenuItem(@"Scene/Assets|Scenes|Group/group2.unity", false, 10)]
	static void OpenAssets_Scenes_Group_group2unity()
	{
		EditorSceneManager.OpenScene(@"Assets/Scenes/Group/group2.unity");
	}
	
	[MenuItem(@"Scene/Assets|Scenes|Scene1.unity", false, 10)]
	static void OpenAssets_Scenes_Scene1unity()
	{
		EditorSceneManager.OpenScene(@"Assets/Scenes/Scene1.unity");
	}
	
	[MenuItem(@"Scene/Assets|Scenes|Scene2.unity", false, 10)]
	static void OpenAssets_Scenes_Scene2unity()
	{
		EditorSceneManager.OpenScene(@"Assets/Scenes/Scene2.unity");
	}
	
}