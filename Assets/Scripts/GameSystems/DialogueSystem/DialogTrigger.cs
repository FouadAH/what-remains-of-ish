using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : Savable
{
    protected Animator promptAnimator;
    protected DialogManager dialogueManager;
    protected Canvas promptCanvas;

    int interactTime = 0;
    public string promptText = "Talk";
    public List<DialogueNodeSO> dialogs;
    public DialogueData dialogueData;
    public struct DialogueData
    {
        public int interactTimes;
    }

    public override void Start()
    {
        base.Start();
        promptCanvas = GetComponentInChildren<Canvas>();
        promptAnimator = promptCanvas.GetComponent<Animator>();
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
            DisplayPrompt();
        }
    }

    public virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            if (Input.GetButtonDown("Interact"))
            {
                Interact();
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
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            RemovePrompt();
        }
    }

    public virtual void Interact()
    {
        if (!dialogueManager.dialogueIsActive)
        {
            TriggerDialogue();
        }
    }

    public override string SaveData()
    {
        dialogueData.interactTimes = interactTime;
        return JsonUtility.ToJson(dialogueData);
    }

    public override void LoadDefaultData()
    {
        dialogueData.interactTimes = 0;
        interactTime = 0;
    }

    public override void LoadData(string data, string version)
    {
        dialogueData = JsonUtility.FromJson<DialogueData>(data);
        interactTime = dialogueData.interactTimes;
        Debug.Log("Loading NPC dialogue data: " + data);
    }
}
