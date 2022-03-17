using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IHitboxResponder
{
    public List<Hitbox> hitboxes;
    Vector3 dir;
    PlayerMovement player;

    public LayerMask obstacleLayer;
    public LayerMask breakableLayer;

    public float knockbackBasicAttack = 10f;
    public float knockbackUpAttack = 10f;
    public float knockbackDownAttack = 25f;

    TimeStop timeStop;

    [Header("Hit Time Stop")]
    public float changeTime = 0.05f;
    public float restoreSpeed = 10f;
    public float delay = 0.1f;

    private AttackProcessor attackProcessor;
    protected Cinemachine.CinemachineImpulseSource impulseListener;
    public ParticleSystem HitEffect;

    void Start()
    {
        player = GetComponent<PlayerMovement>();
        timeStop = GetComponent<TimeStop>();
        attackProcessor = new AttackProcessor();
        impulseListener = GetComponent<Cinemachine.CinemachineImpulseSource>();
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
        hasHitWall = false;
        foreach (var hitbox in hitboxes)
        {
            hitbox.useResponder(this);
            hitbox.startCheckingCollision();
        }
    }

    bool hasHitWall;

    void IHitboxResponder.collisionedWith(Collider2D collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if (hurtbox != null)
        {
            if (dir.y == 1)
            {
                hurtbox.getHitBy(gameObject.GetComponent<IAttacker>(), (dir.x), (dir.y));
                return;
            }

            if(dir.y == -1)
            {
                hurtbox.getHitBy(gameObject.GetComponent<IAttacker>(), 0, -1);
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
            if (!hurtbox.ignoreHitstop)
                timeStop.StopTime(changeTime, restoreSpeed, delay);

            if (IsInLayerMask(collider.gameObject.layer, breakableLayer))
            {
                hurtbox.GetHitByNoKnockback(gameObject.GetComponent<IAttacker>());
            }
            else
            {
                hurtbox.getHitBy(gameObject.GetComponent<IAttacker>(), (dir.x), (-dir.y));
            }
        }
        else if(IsInLayerMask(collider.gameObject.layer, obstacleLayer))
        {
            attackProcessor.ProcessKnockbackOnHit(gameObject.GetComponent<IAttacker>(), (dir.x), 0);
            //impulseListener.GenerateImpulse();

            //if (HitEffect != null && !hasHitWall )
            //{
            //    hasHitWall = true;
            //    Vector3 randomEulerRotation = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
            //    Quaternion randomQuaternionRotation = Quaternion.Euler(randomEulerRotation.x, randomEulerRotation.y, randomEulerRotation.z);
            //    //ParticleSystem hitEffectInstance = Instantiate(HitEffect, hitbox.transform.position, randomQuaternionRotation);
            //    //hitEffectInstance.Play();
            //}
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
