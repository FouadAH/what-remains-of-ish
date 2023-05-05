using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityItem : MonoBehaviour
{
    public GameEvent abilityEvent;
    public ParticleSystem pickUpPaticles;
    public GameObject itemSprite;
    public GameObject itemCanvas;
    public AbilityType abilityType;
    public AbilityPickupChannel pickupChannel;

    Animator promptAnimator;
    bool hasPickedUp;

    private void Start()
    {
        promptAnimator = GetComponentInChildren<Animator>();
    }

    public virtual void Interact()
    {
        if (abilityEvent != null && !hasPickedUp)
        {
            abilityEvent.Raise();
            pickupChannel.RaiseOnPickupAbility(abilityType);
            hasPickedUp = true;
        }
        else
        {
            Debug.LogWarning("Empty Item. Please assign an event in the inspector.");
        }

        pickUpPaticles.Play();

        itemCanvas.SetActive(false);
        itemSprite.SetActive(false);
        enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasPickedUp)
        {
            return;
        }

        if (collision.gameObject.tag.Equals("Player"))
        {
            DisplayPrompt();
        }
    }

    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (hasPickedUp)
        {
            return;
        }

        if (Input.GetButtonDown("Interact"))
        {
            Interact();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (hasPickedUp)
        {
            return;
        }

        if (collision.gameObject.tag.Equals("Player"))
        {
            RemovePrompt();
        }
    }
    protected void DisplayPrompt()
    {
        promptAnimator.ResetTrigger("PopOut");
        promptAnimator.SetTrigger("PopIn");
    }

    protected void RemovePrompt()
    {
        promptAnimator.ResetTrigger("PopIn");
        promptAnimator.SetTrigger("PopOut");
    }

}
