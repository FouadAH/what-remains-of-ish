using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newHealingFlaskShard", menuName = "Items/New HealingFlaskShard", order = 1)]
public class HealingFlaskShardSO : ShopItemSO
{
    public GameEvent ReceivedHealingFLaskShard;

    public override void ReceiveItem()
    {
        base.ReceiveItem();
        ReceivedHealingFLaskShard.Raise();
    }

    public void ReceiveItemTrigger()
    {
        ReceivedHealingFLaskShard.Raise();
    }

    public override void OnBuyItem()
    {
        ReceiveItem();
    }
}
