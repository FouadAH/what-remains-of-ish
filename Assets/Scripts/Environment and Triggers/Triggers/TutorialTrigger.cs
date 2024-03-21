using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public TutorialType tutorialType;
    Animator prompt;
    DialogManager dialogManager;
    TutorialManager tutorialManager;

    private void Start()
    {
        prompt = GetComponentInChildren<Animator>();
        dialogManager = DialogManager.instance;
        tutorialManager = TutorialManager.instance;
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
            if (!dialogManager.dialogueIsActive)
            {
                prompt.ResetTrigger("PopIn");
                prompt.SetTrigger("PopOut");
                tutorialManager.DisplayTutorial(tutorialType);
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactive Objects/Tips", GetComponent<Transform>().position);
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
