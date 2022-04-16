using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFlaskShard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
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

            Destroy(gameObject);
        }
    }
}
