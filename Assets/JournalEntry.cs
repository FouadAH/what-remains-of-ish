using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalEntry : MonoBehaviour
{
    [SerializeField] private Animator prompt;
    public JournalEntrySO journalEntrySO;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            prompt.SetBool("PopIn", true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButtonDown("Interact"))
        {
            UI_HUD.instance.SetDebugText("Picked up a journal entry!");
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            prompt.SetBool("PopIn", false);
        }
    }
}
