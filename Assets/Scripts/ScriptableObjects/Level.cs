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