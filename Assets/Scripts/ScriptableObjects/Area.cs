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

    [Header("Levels")]
    public List<Level> levels;

    [Header("Area Theme")]
    [FMODUnity.EventRef] public string Event;

    [Header("Visuals")]
    public PostProcessProfile postprocess;

    [Header("Map UI")]
    public Image areaUI;
}
