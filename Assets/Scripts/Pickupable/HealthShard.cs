using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthShard : MonoBehaviour
{
    public HealthShardSO healthShardSO;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerMovement>(out _))
        {
            healthShardSO.ReceiveItemTrigger();
            Destroy(gameObject);
        }
    }
}
