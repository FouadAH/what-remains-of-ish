using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newHealthShard", menuName = "Items/New HealthShard", order = 1)]
public class HealthShardSO : ShopItemSO
{
    public GameEvent ReceivedHealthShard;

    public override void ReceiveItem()
    {
        base.ReceiveItem();
        ReceivedHealthShard.Raise();
    }

    public void ReceiveItemTrigger()
    {
        ReceivedHealthShard.Raise();
    }

    public override void OnBuyItem()
    {
        ReceiveItem();
    }
}
