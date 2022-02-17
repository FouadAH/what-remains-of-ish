using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBox : MonoBehaviour
{
    public LayerMask damagables;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() || IsInLayerMask(collision.gameObject.layer, damagables))
        {
            Hurtbox hurtbox = collision.gameObject.GetComponent<Hurtbox>();

            Vector2 dir = Vector2.zero;

            if (hurtbox != null)
            {
                Vector2 direction = (hurtbox.transform.position - transform.position).normalized;

                if (direction.x > 0.1)
                {
                    dir.x = -1;
                }
                else if (direction.x < -0.1)
                {
                    dir.x = 1;
                }

                dir.y = direction.y;
            }

            //Debug.Log("Knockback Dir: X:" + (dir.x) + " Y: " + (dir.y));
            hurtbox?.collisionDamage(1, dir.x, -dir.y);
        }

    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
