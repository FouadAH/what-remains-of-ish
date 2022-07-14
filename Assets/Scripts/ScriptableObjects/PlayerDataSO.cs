using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerDataSO : ScriptableObject
{
    public FloatVariable playerHealth;
    public FloatVariable playerMaxHealth;

    public IntegerReference playerHealingPodAmount;
    public List<int> playerHealingPodFillAmounts;

    public IntegerReference playerHealingAmountPerPod;
    public IntegerReference playerHealthShardAmount;
    public IntegerReference playerHealingFlaskShards;
    public IntegerReference playerCurrency;

    public Vector2Variable playerPosition;

    public Vector2Variable lastCheckpointPos;
    public IntegerReference lastCheckpointLevelIndex;
    public string lastCheckpointLevelPath;

    public Vector2Variable lastSavepointPos;
    public IntegerReference lastSavepointLevelIndex;
    public string lastSavepointLevelPath;

    public IntegerReference currentSceneBuildIndex;

    public bool hasBoomerangAbility;
    public bool hasDashAbility;
    public bool hasAirDashAbility;
    public bool hasTeleportAbility;
    public bool hasWallJumpAbility;
    public bool hasSprintAbility;
    public bool hasDoubleJumpAbility;

}