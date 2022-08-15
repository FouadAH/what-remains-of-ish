using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBox : MonoBehaviour
{
    public LayerMask damagables;
    public int damageAmountPlayer = 1;
    public int damageAmountEnemies = 10;


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
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

            hurtbox?.collisionDamage(damageAmountPlayer, dir.x, -dir.y);
        }
        else if(IsInLayerMask(collision.gameObject.layer, damagables))
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

            hurtbox?.collisionDamage(damageAmountEnemies, dir.x, -dir.y);
        }

    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
