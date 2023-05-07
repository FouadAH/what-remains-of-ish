using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SaveManager))]
public class ObjectBuilderEditor : Editor
{
    bool showSceneData = true;
    bool showEntityData = true;
    bool showItemData = true;

    SaveManager saveManager;

    System.Collections.Generic.Dictionary<string, string> data_entries_scene;
    System.Collections.Generic.Dictionary<string, string> data_entries_entities;
    System.Collections.Generic.Dictionary<string, string> data_entries_items;

    private void OnEnable()
    {
        saveManager = (SaveManager)target;
        if (saveManager.currentSaveFile != null)
        {
            data_entries_scene = saveManager.currentSaveFile.gameData.scene_data.data_entries;
            data_entries_entities = saveManager.currentSaveFile.gameData.enemy_data.data_entries;
            data_entries_items = saveManager.currentSaveFile.gameData.item_data.data_entries;
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        saveManager = (SaveManager)target;
        if (GUILayout.Button("Create Editor Save File"))
        {
            saveManager.SaveGameEditor();
        }

        if (GUILayout.Button("Save"))
        {
            saveManager.SaveGame();

            data_entries_scene = saveManager.currentSaveFile.gameData.scene_data.data_entries;
            data_entries_entities = saveManager.currentSaveFile.gameData.enemy_data.data_entries;
            data_entries_items = saveManager.currentSaveFile.gameData.item_data.data_entries;
        }

        if (GUILayout.Button("Load"))
        {
            saveManager.LoadSavedGame(saveManager.testSaveFile);
        }

        if (GUILayout.Button("Reset and load data"))
        {
            saveManager.ResetAndLoad();
        }

        serializedObject.ApplyModifiedProperties();

        if (saveManager.currentSaveFile != null)
        {
            showSceneData = EditorGUILayout.Foldout(showSceneData, "Current Scene Data");
            if (showSceneData)
            {
                if (data_entries_scene != null)
                {
                    foreach (string key in data_entries_scene.Keys)
                    {
                        //bool _foldout = false;
                        //_foldout = EditorGUILayout.Foldout(_foldout, key);
                        //if (_foldout) {
                        //    EditorGUILayout.TextArea(data_entries_scene[key], EditorStyles.textArea);
                        //}

                        GUILayout.Label(key);
                        EditorGUILayout.TextArea(data_entries_scene[key], EditorStyles.textArea);
                    }
                }
            }

            showEntityData = EditorGUILayout.Foldout(showEntityData, "Current Entity Data");

            if (showEntityData)
            {
                if (data_entries_entities != null)
                {
                    foreach (string key in data_entries_entities.Keys)
                    {
                        GUILayout.Label(key);
                        EditorGUILayout.TextArea(data_entries_entities[key], EditorStyles.textArea);
                    }
                }
            }

            showItemData = EditorGUILayout.Foldout(showItemData, "Current Item Data");

            if (showItemData)
            {
                if (data_entries_items != null)
                {
                    foreach (string key in data_entries_items.Keys)
                    {
                        GUILayout.Label(key);
                        EditorGUILayout.TextArea(data_entries_items[key], EditorStyles.textArea);
                    }
                }
            }
        }
    }
}

