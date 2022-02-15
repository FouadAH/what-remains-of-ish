using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "newShopItem", menuName = "Items/New Shop Item", order = 1)]
public class ShopItemSO : ItemSO
{
    public string itemName;
    [TextArea(4,10)] public string itemDescription;
    public float itemCost;
    public Sprite itemIcon;

    public bool hasBeenSold;
    public bool canBuy;

    public override void ReceiveItem()
    {
        UI_HUD.instance.SetDebugText("Picked up an item!");
    }

    public virtual void OnBuyItem()
    {
        Debug.Log("Buy Item");
    }
}
