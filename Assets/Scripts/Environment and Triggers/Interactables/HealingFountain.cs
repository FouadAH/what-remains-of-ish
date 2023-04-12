using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFountain : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite fullFountainSprite;
    public Sprite emptyFountainSprite;

    [Header("Data")]
    public PlayerDataSO playerData;

    [Header("Events")]
    public IntegerGameEvent reffilEvent;

    int reffilAmount = 100;
    bool playerInTrigger;

    SpriteRenderer spriteRenderer;
    Animator prompt;

    private void Start()
    {
        prompt = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        reffilAmount = 100;
        spriteRenderer.sprite = fullFountainSprite;
    }

    public void PlayerInput_OnInteract()
    {
        if (playerInTrigger)
        {
            ReffilFlasksFromFountain();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && reffilAmount > 0)
        {
            playerInTrigger = true;
            prompt.SetBool("PopIn", true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && reffilAmount > 0)
        {
            playerInTrigger = false;
            prompt.SetBool("PopIn", false);
        }
    }

    void ReffilFlasksFromFountain()
    {
        int missingHealingAmount = 0;

        foreach (int fillAmount in playerData.playerHealingPodFillAmounts)
        {
            if (fillAmount != 100)
            {
                missingHealingAmount = (100 - fillAmount);
                if (missingHealingAmount < reffilAmount)
                {
                    reffilEvent.Raise(missingHealingAmount);
                }
                else
                {
                    reffilEvent.Raise(reffilAmount);
                }

                break;
            }
        }
        reffilAmount = Mathf.Clamp(reffilAmount - missingHealingAmount, 0, 100);

        if (reffilAmount == 0)
        {
            spriteRenderer.sprite = emptyFountainSprite;
            prompt.gameObject.SetActive(false);
        }
    }
}
