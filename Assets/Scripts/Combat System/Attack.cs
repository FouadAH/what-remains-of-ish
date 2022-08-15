﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IHitboxResponder
{
    public List<Hitbox> hitboxes;
    Vector3 dir;
    PlayerMovement player;

    public LayerMask obstacleLayer;
    public LayerMask spikeLayer;
    public LayerMask breakableLayer;

    public float knockbackBasicAttack = 10f;
    public float knockbackUpAttack = 10f;
    public float knockbackDownAttack = 25f;

    TimeStop timeStop;

    [Header("Hit Time Stop")]
    public float changeTime = 0.05f;
    public float restoreSpeed = 10f;
    public float delay = 0.1f;

    [Header("SFX")]
    [FMODUnity.EventRef] public string obstacleHitSFX;
    [FMODUnity.EventRef] public string spikeHitSFX;
    [FMODUnity.EventRef] public string enemyHitSFX;


    private AttackProcessor attackProcessor;
    protected Cinemachine.CinemachineImpulseSource impulseListener;
    public ParticleSystem HitEffect;
    Rumbler rumbler;
    void Start()
    {
        player = GetComponent<PlayerMovement>();
        timeStop = GetComponent<TimeStop>();
        attackProcessor = new AttackProcessor();
        impulseListener = GetComponent<Cinemachine.CinemachineImpulseSource>();
        rumbler = GetComponent<Rumbler>();
    }

    public void AttackDefault()
    {
        dir = new Vector3(-transform.localScale.x, 0);
        player.attackDir = dir;

        player.knockbackDistance = knockbackBasicAttack;
        CheckHitboxes();
    }

    public void AttackUp()
    {
        dir = new Vector3(0, -1);
        player.attackDir = dir;

        player.knockbackDistance = knockbackUpAttack;
        CheckHitboxes();
    }
    public void AttackDown()
    {
        dir = new Vector3(0, 1);
        player.attackDir = dir;

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

            if (enemyHitSFX != null)
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(enemyHitSFX, gameObject);
            }

            rumbler.RumblePulse(1, 2, 0.5f, 0.5f);
        }
        else if(IsInLayerMask(collider.gameObject.layer, spikeLayer))
        {
            Debug.Log("Hit spikes");
            attackProcessor.ProcessKnockbackOnHit(gameObject.GetComponent<IAttacker>(), dir.x, dir.y * 1.3f);

            if (spikeHitSFX != null)
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(spikeHitSFX, gameObject);
            }

            //impulseListener.GenerateImpulse();

            if (HitEffect != null && !hasHitWall)
            {
                hasHitWall = true;
                Vector3 randomEulerRotation = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
                Quaternion randomQuaternionRotation = Quaternion.Euler(randomEulerRotation.x, randomEulerRotation.y, randomEulerRotation.z);
                //ParticleSystem hitEffectInstance = Instantiate(HitEffect, hitbox.transform.position, randomQuaternionRotation);
                //hitEffectInstance.Play();
            }
        }
        else if (IsInLayerMask(collider.gameObject.layer, obstacleLayer))
        {
            Debug.Log("Hit obstacle");
            rumbler.RumblePulse(1, 2, 0.5f, 0.5f);

            if (dir.y == 0)
            {
                attackProcessor.ProcessKnockbackOnHit(gameObject.GetComponent<IAttacker>(), dir.x, 0);
            }

            if (obstacleHitSFX != null)
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(obstacleHitSFX, gameObject);
            }
        }

    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
