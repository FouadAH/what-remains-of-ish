using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEditor;

[CreateAssetMenu(fileName = "Level", menuName = "Levels/New level", order = 1)]
public class Level : ScriptableObject
{
    [Header("Information")]
    public string scene;

    [Header("Map UI")]
    public Image levelMap;
}

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