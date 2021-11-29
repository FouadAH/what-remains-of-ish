using UnityEditor;

[CustomEditor(typeof(Level), true)]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var level = target as Level;
        var scenePath = AssetDatabase.LoadAssetAtPath<SceneAsset>(level.scenePath);
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        var newSceneToLoad = EditorGUILayout.ObjectField("Scene", scenePath, typeof(SceneAsset), false) as SceneAsset;
        if (EditorGUI.EndChangeCheck())
        {
            var newPathToLoad = AssetDatabase.GetAssetPath(newSceneToLoad);

            var scenePathPropertyToLoad = serializedObject.FindProperty("scenePath");

            scenePathPropertyToLoad.stringValue = newPathToLoad;

        }
        serializedObject.ApplyModifiedProperties();
    }
}