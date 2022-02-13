using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : DialogTrigger
{
    public List<ShopItemSO> shopItems;
    public GameEvent onBuyEvent;

    bool isInteracting = false;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Interact()
    {
        isInteracting = true;
        if (isInDialogueMode)
        {
            if (!dialogueManager.dialogueIsActive)
            {
                TriggerDialogue();
            }
        }
        else 
        {
            OpenShopWindow();
        }
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchMode();
        }

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

    bool isInDialogueMode = true;
    void SwitchMode()
    {
        isInDialogueMode = !isInDialogueMode;

        if (isInDialogueMode)
        {
            promptCanvas.GetComponentInChildren<TMPro.TMP_Text>().text = "Talk";
        }
        else
        {
            promptCanvas.GetComponentInChildren<TMPro.TMP_Text>().text = "Shop";
        }
    }

    public void OpenShopWindow()
    {
        ShopManager.instance.EnableShopUI();
        ShopManager.instance.ClearItems();
        ShopManager.instance.AddItems(shopItems);
    }

    public void ExitShop()
    {
        ShopManager.instance.DisableShopUI();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

#if UNITY_EDITOR
        for (int i = 0; i < shopItems.Count; i++)
        {
            shopItems[0].hasBeenSold = false;
        }
#endif

    }
}
