using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Level), true)]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var level = target as Level;
        var scenePath = AssetDatabase.LoadAssetAtPath<SceneAsset>(level.scenePath);
        serializedObject.Update();

        var scenePathPropertyToLoad = serializedObject.FindProperty("scenePath");
        if (!scenePathPropertyToLoad.hasMultipleDifferentValues)
        {
            var newSceneToLoad = EditorGUILayout.ObjectField("Scene", scenePath, typeof(SceneAsset), false) as SceneAsset;
            var newPathToLoad = AssetDatabase.GetAssetPath(newSceneToLoad);
            scenePathPropertyToLoad.stringValue = newPathToLoad;
        }

        serializedObject.ApplyModifiedProperties();
    }
}