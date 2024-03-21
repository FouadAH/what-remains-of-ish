using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class KeyDoor : Door
{
    public DialogTrigger inspectDialogue;
    public EventPromptTrigger unlockDialogue;
    public ItemSO keyItem;

    public override void Awake()
    {
        base.Awake();
        InitDoor();
    }

    void InitDoor()
    {
        if (doorData.isOpen)
        {
            inspectDialogue.gameObject.SetActive(false);
            unlockDialogue.gameObject.SetActive(false);
            return;
        }

        if (keyItem.isOwnedByPlayer)
        {
            inspectDialogue.gameObject.SetActive(false);
            unlockDialogue.gameObject.SetActive(true);
        }
        else
        {
            inspectDialogue.gameObject.SetActive(true);
            unlockDialogue.gameObject.SetActive(false);
        }
    }

    public void Unlock()
    {
        SetState(true);
    }

    public override void LoadDefaultData()
    {
        base.LoadDefaultData();
        InitDoor();
    }

}
