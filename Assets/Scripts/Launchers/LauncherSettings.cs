using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Launcher/Settings", fileName = "LauncherSettings")]
public class LauncherSettings : ScriptableObject
{
    [SerializeField] private bool usedByAi;
    public bool UsedByAi { get => usedByAi; set => usedByAi = value; }
}
