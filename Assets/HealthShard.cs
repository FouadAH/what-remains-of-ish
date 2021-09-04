using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthShard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
        if (playerMovement != null)
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

            Destroy(gameObject);
        }
    }
}
