using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingFlaskShard : MonoBehaviour
{
    public HealingFlaskShardSO healingFlaskShardSO;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerMovement>(out _))
        {
            healingFlaskShardSO.ReceiveItemTrigger();
            Destroy(gameObject);
        }
    }
}
