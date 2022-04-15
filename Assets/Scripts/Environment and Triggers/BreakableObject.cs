using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour, IHittable
{
    [Header("Damagable Stats")]
    public float health;
    public int knockbackForce;

    [Header("Particles")]
    public ParticleSystem spearHitEffect;
    public ParticleSystem breakEffect;

    public event System.Action OnBreak = delegate { };

    void IHittable.ProcessHit(int hitAmount)
    {
        health -= hitAmount;

        Vector3 randomEulerRotation = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
        Quaternion randomQuaternionRotation = Quaternion.Euler(randomEulerRotation.x, randomEulerRotation.y, randomEulerRotation.z);

        if (spearHitEffect) {
            ParticleSystem hitEffectInstance = Instantiate(spearHitEffect, transform.position, randomQuaternionRotation);
            hitEffectInstance.Play();
        }

        if (breakEffect)
        {
            ParticleSystem breakEffectInstance = Instantiate(breakEffect, transform.position, Quaternion.identity);
            breakEffectInstance.Play();
        }

        if (health <= 0)
        {
            OnBreak();
            gameObject.SetActive(false);
        }
    }

}
