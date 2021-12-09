using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Area", menuName = "Areas/New area", order = 1)]
public class Area : ScriptableObject
{
    [Header("Information")]
    public string areaName;
    public List<Level> levels;

    [Header("Audio Settings")]
    [FMODUnity.EventRef] public string areaTheme;

    [Header("Visuals")]
    public PostProcessProfile postProcessProfile;

    [Header("Map Settings")]
    public bool isAreaRevealed;
}
