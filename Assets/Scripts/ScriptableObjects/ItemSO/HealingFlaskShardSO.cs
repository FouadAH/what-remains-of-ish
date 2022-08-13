using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newHealingFlaskShard", menuName = "Items/New HealingFlaskShard", order = 1)]
public class HealingFlaskShardSO : ShopItemSO
{
    public override void ReceiveItem()
    {
        base.ReceiveItem();
        int remainder = GameManager.instance.AddHealingFlaskShard();

        if (remainder == 0)
        {
            UI_HUD.instance.SetDebugText("3 healing pod shards collected. Healing pods increased by 1");
            UI_HUD.instance.InitHealingPods();
        }
        else
        {
            UI_HUD.instance.SetDebugText("Picked Up a Healing Pod Shard. Pick up " + (3 - remainder) + " more to increase the number of healing pods");
        }
    }

    public override void OnBuyItem()
    {
        ReceiveItem();
    }
}
