using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapArea))]
public class MapAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapArea mapArea = (MapArea)target;
        serializedObject.Update();

        if (GUILayout.Button("Add area levels"))
        {
            MapLevel[] mapLevels = mapArea.GetAreaLevels();
            serializedObject.SetIsDifferentCacheDirty();

            var mapLevelsSetializedProperty = serializedObject.FindProperty("mapLevels");

            mapLevelsSetializedProperty.ClearArray();
            for (int i = 0; i < mapLevels.Length; i++)
            {
                mapLevelsSetializedProperty.InsertArrayElementAtIndex(i);
                mapLevelsSetializedProperty.GetArrayElementAtIndex(i).objectReferenceValue = mapLevels[i];
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}