using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public float health;
    public float maxHealth;
    public int currency;

    public float[] playerPosition;
    public int currentScene;

    public float[] lastCheckpointPos;
    public int lastCheckpointLevelIndex;
    public string lastCheckpointLevelPath;


    public float[] lastSavepointPos;
    public int lastSavepointLevelIndex;
    public string lastSavepointLevelPath;

    public PlayerData(GameManager gm)
    {
        health = gm.health;
        maxHealth = gm.maxHealth;
        currency = gm.currency;

        playerPosition = new float[3];
        playerPosition[0] = gm.player.transform.position.x;
        playerPosition[1] = gm.player.transform.position.y;
        playerPosition[2] = gm.player.transform.position.z;

        lastCheckpointPos = new float[2];
        lastCheckpointPos[0] = gm.lastCheckpointPos.x;
        lastCheckpointPos[1] = gm.lastCheckpointPos.y;

        lastCheckpointLevelIndex = gm.lastCheckpointLevelIndex;
        lastCheckpointLevelPath = gm.lastCheckpointLevelPath;

        lastSavepointPos = new float[2];
        lastSavepointPos[0] = gm.lastSavepointPos.x;
        lastSavepointPos[1] = gm.lastSavepointPos.y;

        lastSavepointLevelIndex = gm.lastSavepointLevelIndex;
        lastSavepointLevelPath = gm.lastSavepointLevelPath;

        currentScene = gm.currentScene;
    }
}
