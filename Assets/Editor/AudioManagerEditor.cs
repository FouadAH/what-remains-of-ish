using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        AudioManager audioManager = (AudioManager)target;
        if (GUILayout.Button("Stop Music and Ambiance"))
        {
            audioManager.StopAllAudio();
        }

        if (GUILayout.Button("Stop Ambiance"))
        {
            audioManager.StopAreaAmbianceWithFade();
        }

        if (GUILayout.Button("Stop Music"))
        {
            audioManager.StopAreaThemeWithFade();
        }
    }
}

