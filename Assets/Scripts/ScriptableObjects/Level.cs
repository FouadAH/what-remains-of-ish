using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEditor;
using System;

[CanEditMultipleObjects]
[CreateAssetMenu(fileName = "newRoom", menuName = "New Room", order = 1)]
public class Level : ScriptableObject
{

    [HideInInspector] public string scenePath;

    [Header("Level Light Settings")]
    public float globalLightIntensity = 1;
    public Color globalLightColor = Color.white;

    public bool isRevealed;

}