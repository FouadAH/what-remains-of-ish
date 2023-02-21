using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagableBase : MonoBehaviour, IHittable
{
    [Header("Entity Particle Effects")]
    public ParticleSystem BloodEffect;
    public ParticleSystem HitEffect;
    public ParticleSystem NearDeathEffect;

    public int MaxHealth { get; set; }
    public float Health { get; set; }

    public event Action<float, float> OnHitEnemy = delegate { };
    public event Action OnDeath = delegate { };

    protected ColouredFlash colouredFlash;
    protected CinemachineImpulseSource impulseListener;

    private void Start()
    {
        colouredFlash = GetComponent<ColouredFlash>();
        impulseListener = GetComponent<CinemachineImpulseSource>();
    }

    protected void RaiseOnHitEnemyEvent(float health, float maxHealth)
    {
        var eh = OnHitEnemy;
        if (eh != null)
            OnHitEnemy(health, maxHealth);
    }

    public virtual void ProcessHit(int amount, DamageType type)
    {
        if (!isHittable)
            return;

        StartCoroutine(StunTimer());

        float damageAmount = amount;
        Health -= damageAmount;

        RaiseOnHitEnemyEvent(Health, MaxHealth);
        impulseListener.GenerateImpulse();

        if (BloodEffect != null)
            BloodEffect.Play();

        if (HitEffect != null)
        {
            Vector3 randomEulerRotation = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
            Quaternion randomQuaternionRotation = Quaternion.Euler(randomEulerRotation.x, randomEulerRotation.y, randomEulerRotation.z);
            ParticleSystem hitEffectInstance = Instantiate(HitEffect, transform.position, randomQuaternionRotation);
            hitEffectInstance.Play();
        }

        if (colouredFlash != null)
            colouredFlash.Flash(Color.white);

        if (NearDeathEffect != null)
        {
            if (Health <= 10 && !NearDeathEffect.isPlaying)
            {
                NearDeathEffect.Play();
            }
        }

    }

    private int currentFrames;
    private readonly int ignoreFrames = 2;
    protected bool isHittable = true;

    private IEnumerator StunTimer()
    {
        isHittable = false;
        currentFrames = 0;
        while (currentFrames <= ignoreFrames)
        {
            currentFrames++;
            yield return null;
        }
        isHittable = true;
    }

}
