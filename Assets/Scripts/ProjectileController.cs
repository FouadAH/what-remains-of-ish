using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour, FiringAI
{
    float FiringAI.fireRate { get => fireRate; set => _ = fireRate; }
    float FiringAI.nextFireTime { get => nextFireTime; set => _ = nextFireTime; }

    [Header("Projectile Firing Settings")]
    public float fireRate;
    [HideInInspector] public float nextFireTime;
    public event System.Action OnFire = delegate { };

    public void RaiseOnFireEvent()
    {
        var eh = OnFire;
        if (eh != null)
            OnFire();
    }
    public bool CanFire()
    {
        return Time.time >= nextFireTime;
    }
}
