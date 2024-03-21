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

    bool isInteracting = false;
    bool isInTrigger = false;

    private void Update()
    {
        if (!isInTrigger)
        {
            return;
        }

        if (Input.GetButtonDown("Interact"))
        {
            Interact();
        }
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
        if (collision.gameObject.CompareTag("Player"))
        {
            isInTrigger = true;
            DisplayPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isInTrigger = false;
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
