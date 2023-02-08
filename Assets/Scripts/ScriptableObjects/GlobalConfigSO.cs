using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newGlobalCongig", menuName = "Settings/New Global Config", order = 1)]
public class GlobalConfigSO : ScriptableObject
{
    public GameSettingsSO gameSettings;
}
