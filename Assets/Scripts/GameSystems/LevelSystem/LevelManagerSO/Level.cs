using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "newRoom", menuName = "New Room", order = 1)]
public class Level : ScriptableObject
{
    [HideInInspector] public string scenePath;

    [Header("Level Light Settings")]
    public float globalLightIntensity = 1;
    public Color globalLightColor = Color.white;

    [Header("Post Processing Profile")]
    public VolumeProfile volumeProfile;

    [Header("Map Settings")]
    public bool isRevealed;

    [Header("Level Audio Settings")]
    [FMODUnity.EventRef] public string ambiance;
    [FMODUnity.EventRef] public string theme;

}