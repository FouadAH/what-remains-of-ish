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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            prompt.ResetTrigger("PopOut");
            prompt.SetTrigger("PopIn");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (!dialogueManager.dialogueIsActive)
            {
                prompt.ResetTrigger("PopIn");
                prompt.SetTrigger("PopOut");
                TriggerDialogue();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            prompt.ResetTrigger("PopIn");
            prompt.SetTrigger("PopOut");
        }
    }
}
