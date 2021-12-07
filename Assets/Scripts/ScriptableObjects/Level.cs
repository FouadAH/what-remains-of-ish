using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEditor;
using System;

[CreateAssetMenu(fileName = "Level", menuName = "Levels/New level", order = 1)]
public class Level : ScriptableObject
{
    [SerializeField] private string m_ID = Guid.NewGuid().ToString();
    public string ID => m_ID;

    [ContextMenu("Generate new ID")]
    private void RegenerateGUID() => m_ID = Guid.NewGuid().ToString();

    public string levelName;

    [HideInInspector] public string scenePath;

#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif

    [Header("Level Light Settings")]
    public float globalLightIntensity;
    public Color globalLightColor;

    public bool isRevealed;

}