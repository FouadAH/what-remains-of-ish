using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public float health;
    public float maxHealth;
    public int currency;
    public int healingPodAmount;
    public int healingAmountPerPod;

    public int healthShardAmount;
    public int healingShardAmount;

    public int[] healingPods;
    public float[] playerPosition;
    public int currentScene;

    public float[] lastCheckpointPos;
    public int lastCheckpointLevelIndex;
    public string lastCheckpointLevelPath;

    public float[] lastSavepointPos;
    public int lastSavepointLevelIndex;
    public string lastSavepointLevelPath;

    public bool hasBoomerangAbility;
    public bool hasDashAbility;
    public bool hasAirDashAbility;
    public bool hasTeleportAbility;
    public bool hasWallJumpAbility;
    public bool hasSprintAbility;
    public bool hasDoubleJumpAbility;

    public PlayerData(PlayerDataSO playerDataSO)
    {
        health = playerDataSO.playerHealth.Value;
        maxHealth = playerDataSO.playerMaxHealth.Value;
        currency = playerDataSO.playerCurrency.Value;

        healingPodAmount = playerDataSO.playerHealingPodAmount.Value;
        healingPods = new int[healingPodAmount];
        for(int i = 0; i < healingPodAmount; i++)
        {
            healingPods[i] = 100;
        }

        playerPosition = new float[3];
        playerPosition[0] = playerDataSO.playerPosition.X;
        playerPosition[1] = playerDataSO.playerPosition.Y;
        playerPosition[2] = 0;

        lastCheckpointPos = new float[2];
        lastCheckpointPos[0] = playerDataSO.lastCheckpointPos.X;
        lastCheckpointPos[1] = playerDataSO.lastCheckpointPos.Y;

        lastCheckpointLevelIndex = playerDataSO.lastCheckpointLevelIndex.Value;
        lastCheckpointLevelPath = playerDataSO.lastCheckpointLevelPath;

        lastSavepointPos = new float[2];
        lastSavepointPos[0] = playerDataSO.lastSavepointPos.X;
        lastSavepointPos[1] = playerDataSO.lastSavepointPos.Y;

        lastSavepointLevelIndex = playerDataSO.lastSavepointLevelIndex.Value;
        lastSavepointLevelPath = playerDataSO.lastSavepointLevelPath;

        currentScene = playerDataSO.currentSceneBuildIndex.Value;
    }

    public PlayerData()
    {
        healingPods = new int[2];
        playerPosition = new float[3];
        lastCheckpointPos = new float[2];
        lastSavepointPos = new float[2];
    }

}
