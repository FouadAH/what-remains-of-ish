using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : FiringAI, IDamagable
{
    public bool targeting;

    public int maxHealth;
    public int KnockbackGiven = 0;

    public float Health { get; set; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int knockbackGiven { get => KnockbackGiven; set => KnockbackGiven = value; }

    void Start()
    {
        Health = MaxHealth;
        player = target = GameManager.instance.player.transform;
    }

    void Update()
    {
        if (IsAggro)
        {
            if (targeting)
            {
                Rotation();
            }
            else
            {
                RotationNoTarget();
            }
            if (CanFire())
            {
                nextFireTime = Time.time + fireRate;
                RaiseOnFireEvent();
            }
        }
    }

    public void ModifyHealth(int amount)
    {
        Aggro();
        Health -= amount;
        RaiseOnHitEnemyEvent(Health, maxHealth);
        SpawnDamagePoints(amount);
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void KnockbackOnDamage(int amount, int dirX, int dirY)
    {
        //no knockback
    }

}
