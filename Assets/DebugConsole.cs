using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugConsole : MonoBehaviour
{
    [Header("Debug UI")]
    public TMP_Text velocityXDebug;
    public TMP_Text velocityYDebug;

    public TMP_Text positionDebugText;
    public TMP_Text checkpointPosDebugText;
    public TMP_Text savepointPosText;

    public TMP_Text lastGroundedPosText;

    [Header("Data")]
    public PlayerRuntimeDataSO playerRuntimeData;
    public PlayerDataSO playerData;

    Canvas canvas;
    Player_Input player_Input;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }
    private void OnEnable()
    {
        player_Input = FindObjectOfType<Player_Input>();
        player_Input.OnDebug += Player_Input_OnDebug;
    }
    private void OnDisable()
    {
        player_Input.OnDebug -= Player_Input_OnDebug;   
    }
    private void Player_Input_OnDebug()
    {
        ToggleCanvas();
    }

    void ToggleCanvas()
    {
        canvas.enabled = !canvas.enabled;
    }

    private void Update()
    {
        velocityXDebug.text = string.Format("VelocityX: {0}", playerRuntimeData.velocity.x.ToString());
        velocityYDebug.text = string.Format("VelocityY: {0}", playerRuntimeData.velocity.y.ToString());

        positionDebugText.text = string.Format("World Pos: {0}", playerRuntimeData.playerPosition.ToString());
        checkpointPosDebugText.text = string.Format("Checkpoint Pos: ({0}, {1})", playerData.lastCheckpointPos.X, playerData.lastCheckpointPos.Y);
        savepointPosText.text = string.Format("Savepoint Pos: ({0}, {1})", playerData.lastSavepointPos.X, playerData.lastCheckpointPos.Y);

        lastGroundedPosText.text = string.Format("Last Grounded Pos: {0}", playerRuntimeData.lastPlayerGroundedPosition.ToString());
    }
}
