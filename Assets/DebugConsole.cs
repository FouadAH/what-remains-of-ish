using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    [Header("Debug UI")]
    public TMP_Text velocityXDebug;
    public TMP_Text velocityYDebug;

    [Header("Data")]
    public PlayerRuntimeDataSO playerRuntimeData;
    public PlayerDataSO playerData;

    private void Update()
    {
        velocityXDebug.text = playerRuntimeData.velocity.x.ToString();
        velocityXDebug.text = playerRuntimeData.velocity.y.ToString();
    }
}
