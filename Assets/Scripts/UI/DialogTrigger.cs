using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public Dialog dialog;
    [SerializeField] private Animator prompt;
    public DialogManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<DialogManager>();
    }
    
    public void TriggerDialogue()
    {
        gm.StartDialogue(dialog);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            prompt.SetTrigger("PopIn");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButtonDown("Interact"))
        {
            gm.StartDialogue(dialog);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            prompt.SetTrigger("PopOut");
        }
    }
}
