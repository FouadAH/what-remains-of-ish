using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopItemView : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    public TMPro.TMP_Text itemCost;
    public Image itemSprite;
    public Button selectButton;
    public ShopItemSO itemSO;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShopManager.instance.itemName.text = itemSO.GetName();
        ShopManager.instance.itemDescription.text = itemSO.GetDescription();
        ShopManager.instance.currentSelectedItem = itemSO;
        selectButton.interactable = (ShopManager.instance.playerCurrency.Value >= itemSO.GetCost());
    }

    public void OnSelect(BaseEventData eventData)
    {
        ShopManager.instance.itemName.text = itemSO.GetName();
        ShopManager.instance.itemDescription.text = itemSO.GetDescription();
        ShopManager.instance.currentSelectedItem = itemSO;
        selectButton.interactable = (ShopManager.instance.playerCurrency.Value >= itemSO.GetCost());
    }
}
