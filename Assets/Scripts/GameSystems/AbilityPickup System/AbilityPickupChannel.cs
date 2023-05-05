using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newAbilityPickupChannel", menuName = "Channels/Ability Pickup Channel")]
public class AbilityPickupChannel : ScriptableObject
{
    public Action<AbilityType> OnPickupAbility;

    public void RaiseOnPickupAbility(AbilityType type)
    {
        OnPickupAbility?.Invoke(type);
    }
}

public enum AbilityType
{
    Boomerang,
    AirDash,
    Teleport,
    HealingFlask,
    Health,
    Currency
}
