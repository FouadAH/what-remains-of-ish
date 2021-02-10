using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask hittable;
    public GameObject bulletEffect;
    public event Action<Collider2D> OnRangedHit = delegate { };
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(IsInLayerMask(collision.gameObject.layer, hittable))
        {
            OnRangedHit.Invoke(collision);
            Instantiate(bulletEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
