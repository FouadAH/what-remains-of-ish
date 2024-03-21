using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newRuntimePlayerData", menuName ="PlayerData/New Runtime Data")]
public class PlayerRuntimeDataSO : ScriptableObject
{
    public Vector2 playerPosition;
    public Vector2 lastPlayerGroundedPosition;
    public Vector2 velocity;

    public float entityCoinDropModidier = 1;
    public float entityStunTimeModifier = 1;
}
