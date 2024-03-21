using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    public LayerMask playerLayer;
    public LayerMask obstacles;

    public GameObject destroyEffect;
    public event Action<Collider2D, Vector2> OnRangedHit = delegate { };

    bool canDamage = true;

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, 5f));    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject.layer, playerLayer))
        {
            if (canDamage && !collision.GetComponent<Player>().invinsible)
            {
                Vector2 hitPos = transform.position;
                OnRangedHit.Invoke(collision, hitPos);
                Instantiate(destroyEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
        else if (IsInLayerMask(collision.gameObject.layer, obstacles))
        {
            canDamage = false;
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
