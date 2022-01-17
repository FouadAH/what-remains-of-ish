using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SaveManager))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveManager saveManager = (SaveManager)target;
        if (GUILayout.Button("Create Editor Save File"))
        {
            saveManager.SaveGameEditor();
        }
    }
}

