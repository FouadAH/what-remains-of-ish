using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerConfig", menuName = "New Player Config", order = 1)]

public class PlayerConfig : ScriptableObject
{
    public float health;
    public float maxHealth;
    public int healinPodAmount;

    public List<int> healingPods;
    public int healthShardAmount;
    public int initialCurrency;

    public Vector2 initialPlayerPosition;
    public int initialLevelIndex;
    public string initialLevelPath;

}
