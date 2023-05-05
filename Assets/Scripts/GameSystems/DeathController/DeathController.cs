using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathController : MonoBehaviour
{
    public DeathDrop deathDropPrefab;

    public PlayerDataSO playerData;
    public PlayerRuntimeDataSO PlayerRuntimeDataSO;
    public float dropPrecentage = 30f;
    DeathDrop deathDrop;

    public void OnPlayerDeath()
    {
        int lostCurrency = Mathf.FloorToInt(playerData.playerCurrency.Value * (dropPrecentage/100));

        if(deathDrop != null)
        {
            Destroy(deathDrop.gameObject);
        }

        deathDrop = Instantiate(deathDropPrefab, PlayerRuntimeDataSO.lastPlayerGroundedPosition, Quaternion.identity, transform);
        deathDrop.InitializeDeathDrop(lostCurrency, PlayerRuntimeDataSO.lastPlayerGroundedPosition);
        deathDrop.pickUpEvent += OnPickupDeathDrop;
        playerData.playerCurrency.Value -= lostCurrency;
    }

    void OnPickupDeathDrop(int pickupAmount)
    {
        playerData.playerCurrency.Value += pickupAmount;
        deathDrop.pickUpEvent -= OnPickupDeathDrop;

        Destroy(deathDrop.gameObject);
    }

}
