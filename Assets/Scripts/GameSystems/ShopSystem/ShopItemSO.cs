using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[CreateAssetMenu(fileName = "newShopItem", menuName = "Items/New Shop Item", order = 1)]
public class ShopItemSO : ItemSO
{
    [SerializeField] 
    protected string itemName { get; }

    [SerializeField]
    [TextArea(1, 10)]
    protected string itemDescription;

    [SerializeField] 
    protected float itemCost { get; }

    [SerializeField] 
    protected Sprite itemIcon { get; }

    public bool hasBeenSold;

    public override void ReceiveItem()
    {
        base.ReceiveItem();
    }

    public virtual void OnBuyItem()
    {
        UI_HUD.instance.SetDebugText("Bought an item:" + itemName);
        isOwnedByPlayer = true;
    }

    public string GetDescription()
    {
        return itemDescription;
    }
    public string GetName()
    {
        return itemName;
    }

    public float GetCost()
    {
        return itemCost;
    }

    public Sprite GetIcon()
    {
        return itemIcon;
    }
}
