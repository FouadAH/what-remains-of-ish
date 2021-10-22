using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFountain : MonoBehaviour
{
    [SerializeField] private Animator prompt;

    public float reffilAmount = 100;
    SpriteRenderer sprite;
    [SerializeField] SpriteMask mask;
    float fountainSize;


    private void Start()
    {
        fountainSize = 5;
        mask = GetComponentInChildren<SpriteMask>();
        prompt = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.color = Color.white;
        reffilAmount = 100;
    }

    private void Update()
    {
        float yPos = Mathf.Lerp(mask.transform.localPosition.y,  (((100 - reffilAmount) / 100) * -fountainSize), 0.1f);
        mask.transform.localPosition = new Vector2(mask.transform.localPosition.x, yPos);
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
                    missingHealingAmount += (100 - pod.fillAmount);
                }
            }

            if(reffilAmount < missingHealingAmount)
            {
                UI_HUD.instance.RefillFlask(reffilAmount);
            }
            else
            {
                UI_HUD.instance.RefillFlask(missingHealingAmount);
            }

            reffilAmount = Mathf.Clamp(reffilAmount - missingHealingAmount, 0, 100);
            if (reffilAmount == 0)
            {
                sprite.color = Color.gray;
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
