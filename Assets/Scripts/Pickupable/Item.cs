using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemSO itemSO;
    public ParticleSystem pickUpPaticles;
    public GameObject itemSprite;
    public GameObject itemCanvas;

    Animator promptAnimator;

    private void Start()
    {
        promptAnimator = GetComponentInChildren<Animator>();
    }

    public virtual void Interact()
    {
        if (itemSO != null)
        {
            itemSO.ReceiveItem();
        }
        else
        {
            Debug.LogWarning("Empty Item. Please assign an item in the inspector.");
        }

        pickUpPaticles.Play();

        itemCanvas.SetActive(false);
        itemSprite.SetActive(false);
        enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            DisplayPrompt();
        }
    }

    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButtonDown("Interact"))
        {
            Interact();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
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
