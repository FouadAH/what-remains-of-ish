using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Attack : MonoBehaviour, IHitboxResponder
{
    public List<Hitbox> hitboxes;

    [Header("Layers")]
    public LayerMask obstacleLayer;
    public LayerMask spikeLayer;
    public LayerMask breakableLayer;

    [Header("Knockback Settings")]
    public float knockbackBasicAttack = 10f;
    public float knockbackUpAttack = 10f;
    public float knockbackDownAttack = 25f;

    [Header("Hit Time Stop")]
    public float changeTime = 0.05f;
    public float restoreSpeed = 10f;
    public float delay = 0.1f;

    [Header("SFX")]
    [FMODUnity.EventRef] public string obstacleHitSFX;
    [FMODUnity.EventRef] public string spikeHitSFX;
    [FMODUnity.EventRef] public string enemyHitSFX;

    [Header("VFX")]
    public Transform vfxSpawnParent;
    public ParticleSystem HitEffect;

    private Vector3 attackDirection;
    bool hasHitWall;

    private AttackProcessor attackProcessor;
    protected Cinemachine.CinemachineImpulseSource impulseListener;

    private Rumbler rumbler;
    private TimeStop timeStop;
    private PlayerMovement player;

    void Start()
    {
        player = GetComponent<PlayerMovement>();
        timeStop = TimeStop.instance;
        attackProcessor = new AttackProcessor();
        impulseListener = GetComponent<Cinemachine.CinemachineImpulseSource>();
        rumbler = GetComponent<Rumbler>();
    }

    public void AttackDefault()
    {
        attackDirection = new Vector3(-transform.localScale.x, 0);
        player.attackDir = attackDirection;

        player.knockbackDistance = knockbackBasicAttack;
        CheckHitboxes();
    }

    public void AttackUp()
    {
        attackDirection = new Vector3(0, -1);
        player.attackDir = attackDirection;

        player.knockbackDistance = knockbackUpAttack;
        CheckHitboxes();
    }
    public void AttackDown()
    {
        attackDirection = new Vector3(0, 1);
        player.attackDir = attackDirection;

        player.knockbackDistance = knockbackDownAttack;
        CheckHitboxes(true);
    }

    private void CheckHitboxes(bool stayAwake = false)
    {
        hasHitWall = false;
        foreach (var hitbox in hitboxes)
        {
            hitbox.useResponder(this);
            hitbox.startCheckingCollision(stayAwake);
        }
    }

    void IHitboxResponder.collisionedWith(Collider2D collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if (hurtbox != null)
        {
            if (attackDirection.y == 1)
            {
                hurtbox.getHitBy(gameObject.GetComponent<IAttacker>(), (attackDirection.x), (attackDirection.y));
                return;
            }

            if(attackDirection.y == -1)
            {
                hurtbox.getHitBy(gameObject.GetComponent<IAttacker>(), 0, -1);
                return;
            }

            Vector2 direction = (hurtbox.transform.position - transform.position).normalized;
            if (direction.x > 0)
            {
                attackDirection.x = -1;
                attackDirection.y = 0;
            }
            else
            {
                attackDirection.x = 1;
                attackDirection.y = 0;
            }

            if (!hurtbox.ignoreHitstop)
                timeStop.StopTime(changeTime, restoreSpeed, delay);

            if (IsInLayerMask(collider.gameObject.layer, breakableLayer))
            {
                hurtbox.GetHitByNoKnockback(gameObject.GetComponent<IAttacker>());
            }
            else
            {
                hurtbox.getHitBy(gameObject.GetComponent<IAttacker>(), (attackDirection.x), (-attackDirection.y));
            }

            if (enemyHitSFX != null)
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(enemyHitSFX, gameObject);
            }

            rumbler.RumblePulse(0.5f, 0.6f, 0.5f, 0.3f);
        }
        else if(IsInLayerMask(collider.gameObject.layer, spikeLayer))
        {
            Debug.Log("Hit spikes");
            attackProcessor.ProcessKnockbackOnHit(gameObject.GetComponent<IAttacker>(), attackDirection.x, attackDirection.y * 1.3f);
            
            if (spikeHitSFX != null)
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(spikeHitSFX, gameObject);
            }

            PlayObstacleHitEffects();
        }
        else if (IsInLayerMask(collider.gameObject.layer, obstacleLayer))
        {
            if (attackDirection.y == 0)
            {
                attackProcessor.ProcessKnockbackOnHit(gameObject.GetComponent<IAttacker>(), attackDirection.x, 0);
            }
            else if(attackDirection.y > 0)
            {
                Debug.Log("Hit obstacle");
                attackProcessor.ProcessKnockbackOnHit(gameObject.GetComponent<IAttacker>(), attackDirection.x, attackDirection.y);
            }

            if (obstacleHitSFX != null)
            {
                FMODUnity.RuntimeManager.PlayOneShotAttached(obstacleHitSFX, gameObject);
            }

            PlayObstacleHitEffects();
        }

    }

    void PlayObstacleHitEffects()
    {
        impulseListener.GenerateImpulseWithForce(0.5f);
        rumbler.RumblePulse(0.5f, 0.6f, 0.5f, 0.3f);
        if (HitEffect != null && !hasHitWall)
        {
            hasHitWall = true;
            Vector3 randomEulerRotation = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
            Quaternion randomQuaternionRotation = Quaternion.Euler(randomEulerRotation.x, randomEulerRotation.y, randomEulerRotation.z);
            ParticleSystem hitEffectInstance = Instantiate(HitEffect, vfxSpawnParent.position, randomQuaternionRotation);
            hitEffectInstance.Play();
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
