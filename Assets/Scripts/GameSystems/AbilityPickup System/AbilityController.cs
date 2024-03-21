using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityController : MonoBehaviour
{
    [Header("Player Data")]

    public PlayerDataSO PlayerData;

    public void EnableBoomerangAbility()
    {
        PlayerData.hasBoomerangAbility = true;
    }

    public void EnableAirDashAbility()
    {
        PlayerData.hasAirDashAbility = true;
    }

    public void EnableWallJumpAbility()
    {
        PlayerData.hasWallJumpAbility = true;
    }

    public void EnableTeleportAbility()
    {
        PlayerData.hasTeleportAbility = true;
    }
}
