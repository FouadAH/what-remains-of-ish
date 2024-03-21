using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour, FiringAI
{
    float FiringAI.fireRate { get => fireRate; set => _ = fireRate; }
    float FiringAI.nextFireTime { get => nextFireTime; set => _ = nextFireTime; }

    [Header("Projectile Firing Settings")]
    public float fireRate;
    public float nextFireTime;
    public event System.Action OnFire = delegate { };
    float lastFireTime;
    public void RaiseOnFireEvent()
    {
        if (CanFire())
        {
            lastFireTime = Time.time;
            OnFire?.Invoke();
        }
    }
    public bool CanFire()
    {
        return Time.time >= lastFireTime + fireRate;
    }
}
