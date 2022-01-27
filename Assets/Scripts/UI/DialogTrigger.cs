using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialog dialog;

    [SerializeField] private Animator prompt;
    [HideInInspector] public DialogManager dialogueManager;

    public int interactTime = 0;
    public List<DialogueNodeSO> dialogs;


    private void Start()
    {
        dialogueManager = DialogManager.instance;
    }
    
    public void TriggerDialogue()
    {
        dialogueManager.StartDialogue(dialogs[interactTime].dialog, GetType());

        if (interactTime < dialogs.Count - 1)
        {
            interactTime++;
        }
    }

    void DisplayPrompt()
    {
        prompt.ResetTrigger("PopOut");
        prompt.SetTrigger("PopIn");
    }

    void RemovePrompt()
    {
        prompt.ResetTrigger("PopIn");
        prompt.SetTrigger("PopOut");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            DisplayPrompt();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (!dialogueManager.dialogueIsActive)
            {
                TriggerDialogue();
            }
        }

        if (!dialogueManager.dialogueIsActive)
        {
            DisplayPrompt();
        }
        else
        {
            RemovePrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            RemovePrompt();
        }
    }
}
