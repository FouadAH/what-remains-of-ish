using UnityEditor;

[CustomEditor(typeof(Level), true)]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var level = target as Level;
        var scenePath = AssetDatabase.LoadAssetAtPath<SceneAsset>(level.scene);
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        var newSceneToLoad = EditorGUILayout.ObjectField("Scene", scenePath, typeof(SceneAsset), false) as SceneAsset;

        if (EditorGUI.EndChangeCheck())
        {
            var newPathToLoad = AssetDatabase.GetAssetPath(newSceneToLoad);

            var scenePathPropertyToLoad = serializedObject.FindProperty("scene");

            scenePathPropertyToLoad.stringValue = newPathToLoad;

        }
        serializedObject.ApplyModifiedProperties();
    }
}