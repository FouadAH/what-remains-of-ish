using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newYarnItem", menuName = "Items/New Yarn Item", order = 1)]

public class YarnItemSO : ShopItemSO
{
    public GameEvent receivedYarnItemEvent;

    public override void ReceiveItem()
    {
        base.ReceiveItem();
        receivedYarnItemEvent.Raise();
    }

    public void ReceiveItemTrigger()
    {
        receivedYarnItemEvent.Raise();
    }

    public override void OnBuyItem()
    {
        ReceiveItem();
    }
}
