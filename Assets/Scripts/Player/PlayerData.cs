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
    public float[] dronePosition;
    public float[] lastCheckpointPos;
    public int lastCheckpointLevelIndex;
    public int currentScene;

    public PlayerData(GameManager gm)
    {
        health = gm.health;
        maxHealth = gm.maxHealth;
        currency = gm.currency;

        playerPosition = new float[3];
        playerPosition[0] = gm.player.transform.position.x;
        playerPosition[1] = gm.player.transform.position.y;
        playerPosition[2] = gm.player.transform.position.z;

        dronePosition = new float[3];
        dronePosition[0] = gm.drone.transform.position.x;
        dronePosition[1] = gm.drone.transform.position.y;
        dronePosition[2] = gm.drone.transform.position.z;

        lastCheckpointPos = new float[2];
        lastCheckpointPos[0] = gm.lastCheckpointPos.x;
        lastCheckpointPos[1] = gm.lastCheckpointPos.y;

        lastCheckpointLevelIndex = gm.lastCheckpointLevelIndex;

        currentScene = gm.currentScene;
    }
}
