using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    public Animator promptAnimator;
    public Canvas promptCanvas;

    public List<ShopItemSO> shopItems;
    public GameEvent onBuyEvent;

    bool isInteracting = false;

    public void Interact()
    {
        isInteracting = true;
        OpenShopWindow();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        promptCanvas.GetComponentInChildren<TMPro.TMP_Text>().text = "Shop";

        if (collision.gameObject.tag.Equals("Player"))
        {
            DisplayPrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            RemovePrompt();
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
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

    bool isInDialogueMode = true;

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

    public void OnDestroy()
    {

#if UNITY_EDITOR
        for (int i = 0; i < shopItems.Count; i++)
        {
            shopItems[0].hasBeenSold = false;
        }
#endif

    }
}
