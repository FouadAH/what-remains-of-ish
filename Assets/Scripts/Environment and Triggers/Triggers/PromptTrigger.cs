using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromptTrigger : MonoBehaviour
{
    protected Animator promptAnimator;
    protected Canvas promptCanvas;

    public string promptText = "Talk";

    bool isInTrigger = false;

    public void Start()
    {
        promptCanvas = GetComponentInChildren<Canvas>();
        promptAnimator = promptCanvas.GetComponent<Animator>();
    }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        promptCanvas.GetComponentInChildren<TMPro.TMP_Text>().text = promptText;

        if (collision.GetComponent<Player>())
        {
            isInTrigger = true;
            DisplayPrompt();
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            isInTrigger = false;
            RemovePrompt();
        }
    }

    public virtual void Interact()
    {
    }
}
