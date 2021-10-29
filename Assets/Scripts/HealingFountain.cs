using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFountain : MonoBehaviour
{
    [SerializeField] private Animator prompt;

    public float reffilAmount = 100;
    SpriteRenderer spriteRenderer;

    public Sprite fullFountainSprite;
    public Sprite emptyFountainSprite;


    private void Start()
    {
        prompt = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        reffilAmount = 100;
        spriteRenderer.sprite = fullFountainSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player") && reffilAmount > 0)
        {
            prompt.SetBool("PopIn", true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButtonDown("Interact") && reffilAmount > 0)
        {
            List<HealingPod> healingFlasks = UI_HUD.instance.healingFlasks;
            float missingHealingAmount = 0;

            foreach(HealingPod pod in healingFlasks)
            {
                if (pod.fillAmount != 100)
                {
                    missingHealingAmount = (100 - pod.fillAmount);
                    if(missingHealingAmount < reffilAmount)
                    {
                        pod.Refill(missingHealingAmount);
                    }
                    else
                    {
                        pod.Refill(reffilAmount);
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player") && reffilAmount > 0)
        {
            prompt.SetBool("PopIn", false);
        }
    }
}
