using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newGameSettings", menuName = "Settings/New Game Settings", order = 1)]
public class GameSettingsSO : ScriptableObject
{
    [SerializeField] 
    private bool useDirectionalMouseAttacks;

    [SerializeField] 
    private bool aimAssistOn;

    [HideInInspector]
    public bool controllerConnected;

    public bool UseDirectionalMouseAttacks
    {
        get => PlayerPrefs.GetInt("useDirectionalMouseAttacks", 1) == 0;
        set => PlayerPrefs.SetInt("useDirectionalMouseAttacks", (useDirectionalMouseAttacks) ? 0 : 1);
    }

    public bool AimAssistOn
    {
        get => PlayerPrefs.GetInt("aimAssistOn", 0) == 0;
        set => PlayerPrefs.SetInt("aimAssistOn", (aimAssistOn) ? 0 : 1);
    }
}
