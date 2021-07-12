using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBox : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            Hurtbox hurtbox = collision.gameObject.GetComponent<Hurtbox>();

            Vector2 direction = transform.position - hurtbox.transform.position;
            direction = direction.normalized;

            hurtbox?.collisionDamage(1, Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
        }

    }
}
