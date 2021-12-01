using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerDetectedStateData", menuName = "Data/State Data/Player Detected State")]
public class D_PlayerDetected : ScriptableObject
{
    public float longRangeActionTime = 1.5f;
    public float chaseSpeed = 10f;
    public Vector2 playerOffset = new Vector2(5, 5);
    public float attackCooldown = 2.5f;
    public float detectTime = 4f;
}
