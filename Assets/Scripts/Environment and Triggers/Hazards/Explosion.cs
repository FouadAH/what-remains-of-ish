using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ParticleSystem explosionParticles;
    public bool isDamaging = true;
    public LayerMask damagables;
    public int damageAmount = 1;
    public int playerDamageAmount = 1;


    private void Start()
    {
        explosionParticles.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collision.attachedRigidbody.AddExplosionForce(50f, transform.position, 12f);

        if (IsInLayerMask(collision.gameObject.layer, damagables)) 
        {
            Hurtbox hurtbox = collision.gameObject.GetComponent<Hurtbox>();

            Vector2 dir = Vector2.zero;

            if (hurtbox != null)
            {
                Vector2 direction = (hurtbox.transform.position - transform.position).normalized;

                if (direction.x > 0.1)
                    dir.x = -1;
                else if (direction.x < -0.1)
                    dir.x = 1;

                dir.y = direction.y;
            }

            if (hurtbox.gameObject.tag == "Player")
            {
                hurtbox?.collisionDamage(playerDamageAmount, dir.x, -dir.y);
            }
            else
            {
                hurtbox?.collisionDamage(damageAmount, dir.x, -dir.y);
            }
        }
    }

    public void SeldDestruct()
    {
        Destroy(gameObject);
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
