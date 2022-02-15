using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newHealthShard", menuName = "Items/New HealthShard", order = 1)]
public class HealthShardSO : ShopItemSO
{
    public override void ReceiveItem()
    {
        int remainder = GameManager.instance.AddHealthShard();

        if (remainder == 0)
        {
            UI_HUD.instance.SetDebugText("3 health shards collected. Health increased by 1");
        }
        else
        {
            UI_HUD.instance.SetDebugText("Picked Up Health Shard. Pick up " + (3 - remainder) + " more to increase your health");
        }
    }

    public override void OnBuyItem()
    {
        ReceiveItem();
    }
}
