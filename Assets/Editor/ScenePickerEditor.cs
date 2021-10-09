using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScenePicker), true)]
public class ScenePickerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var picker = target as ScenePicker;
        var oldSceneToUnload = AssetDatabase.LoadAssetAtPath<SceneAsset>(picker.scenePathToUnload);
        var oldSceneToLoad = AssetDatabase.LoadAssetAtPath<SceneAsset>(picker.scenePathToLoad);

        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        var newSceneToUnload = EditorGUILayout.ObjectField("Scene To Unload", oldSceneToUnload, typeof(SceneAsset), false) as SceneAsset;
        var newSceneToLoad = EditorGUILayout.ObjectField("Scene To Load", oldSceneToLoad, typeof(SceneAsset), false) as SceneAsset;

        if (EditorGUI.EndChangeCheck())
        {
            var newPathToLoad = AssetDatabase.GetAssetPath(newSceneToLoad);
            var newPathToUnload = AssetDatabase.GetAssetPath(newSceneToUnload);

            var scenePathPropertyToLoad = serializedObject.FindProperty("scenePathToLoad");
            var scenePathPropertyToUnload = serializedObject.FindProperty("scenePathToUnload");

            scenePathPropertyToLoad.stringValue = newPathToLoad;
            scenePathPropertyToUnload.stringValue = newPathToUnload;

        }
        serializedObject.ApplyModifiedProperties();
    }
}