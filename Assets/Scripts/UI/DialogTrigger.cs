using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : Savable
{
    public Dialog dialog;

    [SerializeField] private Animator prompt;
    [HideInInspector] public DialogManager dialogueManager;

    public int interactTime = 0;
    public List<DialogueNodeSO> dialogs;
    public DialogueData dialogueData;
    public struct DialogueData
    {
        public int interactTimes;
    }

    public override void Start()
    {
        base.Start();
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
