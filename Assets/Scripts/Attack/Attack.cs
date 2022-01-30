using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IHitboxResponder
{
    public List<Hitbox> hitboxes;
    Vector3 dir;
    PlayerMovement player;
    
    public float knockbackBasicAttack = 10f;
    public float knockbackUpAttack = 10f;
    public float knockbackDownAttack = 25f;

    TimeStop timeStop;

    [Header("Hit Time Stop")]
    public float changeTime = 0.05f;
    public float restoreSpeed = 10f;
    public float delay = 0.1f;

    void Start()
    {
        player = GetComponent<PlayerMovement>();
        timeStop = GetComponent<TimeStop>();
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
            if(!hurtbox.ignoreHitstop)
                timeStop.StopTime(changeTime, restoreSpeed, delay);

            if (dir.y == 1)
            {
                hurtbox?.getHitBy(gameObject.GetComponent<IAttacker>(), (dir.x), (dir.y));
                return;
            }

            if(dir.y == -1)
            {
                hurtbox?.getHitBy(gameObject.GetComponent<IAttacker>(), 0, -1);
                return;
            }

            Vector2 direction = (hurtbox.transform.position - transform.position).normalized;
            if (direction.x > 0)
            {
                dir.x = -1;
                dir.y = 0;
            }
            else
            {
                dir.x = 1;
                dir.y = 0;
            }

        }

        //Debug.Log("Knockback Dir: X:" + (dir.x) + " Y: " + (dir.y));
        hurtbox?.getHitBy(gameObject.GetComponent<IAttacker>(), (dir.x), (-dir.y));
    }
}
