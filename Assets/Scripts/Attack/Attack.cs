using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IHitboxResponder
{
    public List<Hitbox> hitboxes;
    Vector3 dir;
    PlayerMovement player;
    
    public ParticleSystem attackParticles;

    Vector3 offset;
    Quaternion quaternion;
    ParticleSystem attackParticlesRuntime;

    public float knockbackBasicAttack = 10f;
    public float knockbackUpAttack = 10f;
    public float knockbackDownAttack = 25f;

    void Start()
    {
        player = GetComponent<PlayerMovement>();
    }

    public void AttackDefault()
    {
        dir = new Vector3(-transform.localScale.x, 0);
        player.knockbackDistance = knockbackBasicAttack;
        CheckHitboxes();
    }

    public void AttackUp()
    {
        dir = new Vector3(0, -1);
        player.knockbackDistance = knockbackUpAttack;
        CheckHitboxes();
    }
    public void AttackDown()
    {
        dir = new Vector3(0, 1);
        player.knockbackDistance = knockbackDownAttack;
        CheckHitboxes();
    }

    private void CheckHitboxes()
    {
        foreach (var hitbox in hitboxes)
        {
            hitbox.useResponder(this);
            hitbox.startCheckingCollision();
        }
    }

    void IHitboxResponder.collisionedWith(Collider2D collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if (hurtbox != null)
        {
            if(dir.y == 1)
            {
                //if (attackParticlesRuntime == null)
                //{
                //    offset = new Vector3(0, -4, 0);
                //    quaternion = Quaternion.Euler(0, 0, 180);
                //    attackParticlesRuntime = Instantiate(attackParticles, transform.position + offset, quaternion);
                //    attackParticlesRuntime.Play();
                //}

                hurtbox?.getHitBy(gameObject.GetComponent<IAttacker>(), (dir.x), (dir.y));
                return;
            }

            //if (attackParticlesRuntime == null)
            //{
            //    offset = new Vector3(dir.x * -4, dir.y * -4, 0);
            //    quaternion = Quaternion.Euler(0, 0, 90 * dir.x);
            //    attackParticlesRuntime = Instantiate(attackParticles, transform.position + offset, quaternion);
            //    attackParticlesRuntime.Play();
            //}

            Vector2 direction = (hurtbox.transform.position - transform.position).normalized;

            if (direction.x > 0)
            {
                dir.x = -1;
            }
            else
            {
                dir.x = 1;
            }

            dir.y = direction.y;
        }

        //Debug.Log("Knockback Dir: X:" + (dir.x) + " Y: " + (dir.y));
        hurtbox?.getHitBy(gameObject.GetComponent<IAttacker>(), (dir.x), (-dir.y));
    }
}
