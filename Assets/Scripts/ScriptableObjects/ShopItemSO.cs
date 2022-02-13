using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class ShopItemSO : ScriptableObject
{
    public string itemName;
    [TextArea(4,10)] public string itemDescription;
    public float itemCost;
    public Sprite itemIcon;

    public bool hasBeenSold;
    public bool canBuy;

    public virtual void OnBuyItem()
    {
        Debug.Log("Buy Item");
    }
}
