using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealingController : MonoBehaviour
{
    public PlayerDataSO playerData;

    float refillModNormal = 1f;
    float refillModExtra = 1.5f;

    public void OnRest()
    {
        for (int i = 0; i < playerData.playerHealingPodFillAmounts.Count; i++)
        {
            playerData.playerHealingPodFillAmounts[i] = 100;
        }
    }

    public void OnHeal()
    {
        //Empty the first flask
        playerData.playerHealingPodFillAmounts[0] = 0;

        //Trickle down flasks
        for (int i = 1; i < playerData.playerHealingPodFillAmounts.Count; i++)
        {
            if (playerData.playerHealingPodFillAmounts[i] > 0)
            {
                float newFillAmount = playerData.playerHealingPodFillAmounts[i];
                playerData.playerHealingPodFillAmounts[i - 1] = (int)newFillAmount;
                playerData.playerHealingPodFillAmounts[i] = 0;
            }
        }
    }

    public void OnReffil(int amount)
    {
        for (int i = 0; i < playerData.playerHealingPodFillAmounts.Count; i++)
        {
            if (playerData.playerHealingPodFillAmounts[i] < 100)
            {
                float refillMod = (GameManager.instance.equippedBrooch_03) ? refillModExtra : refillModNormal;

                playerData.playerHealingPodFillAmounts[i] += (int)(amount * refillMod);
                playerData.playerHealingPodFillAmounts[i] = (int)Mathf.Clamp((float)playerData.playerHealingPodFillAmounts[i], 0, 100);

                break;
            }
        }
    }
}
