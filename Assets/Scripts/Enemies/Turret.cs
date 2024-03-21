﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Entity;

public class Turret : MonoBehaviour, FiringAI, IDamagable
{
    public bool targeting;

    public bool isDamagable = true;
    public bool isStatic = false;

    public float rotateSpeed;
    [HideInInspector] public Transform target;

    public float fireRate = 1f;
    public float delay = 0f;

    float nextFireTime;

    float FiringAI.fireRate { get => fireRate; set => value = fireRate; }
    float FiringAI.nextFireTime { get => nextFireTime; set => value = nextFireTime; }

    public int maxHealth;

    public int MaxHealth { get; set; }
    public float Health { get; set; }
    public int knockbackGiven { get; set; }

    public event Action OnFire = delegate { };

    [Header("Hit Effects")]
    [SerializeField] private GameObject damageNumberPrefab;

    public event Action<float, float> OnHitEnemy = delegate { };

    ColouredFlash colouredFlash;
    public float flaskRefillAmount = 5f;

    private float currentStunResistance;
    private float lastDamageTime;
    public float stunResistance = 20f;
    public float stunRecoveryTime = 1f;
    protected bool isStunned;

    void Start()
    {
        target = GameManager.instance.player.transform;
        colouredFlash = GetComponentInChildren<ColouredFlash>();
        MaxHealth = maxHealth;
        Health = MaxHealth;
    }

    bool firstTake = true;
    void Update()
    {
        if (isStatic)
        {
            if (CanFire())
            {
                nextFireTime = Time.time + fireRate;
                RaiseOnFireEvent();
            }
            return;
        }


        if (Time.time >= lastDamageTime + stunRecoveryTime && isStunned)
        {
            ResetStunResistance();
        }

        if (!isStunned)
        {
            currentStunResistance = Mathf.Lerp(currentStunResistance, stunResistance, 0.004f);
        }

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

    public void RaiseOnFireEvent()
    {
        var eh = OnFire;
        if (eh != null)
            OnFire();
    }

    public void Rotation()
    {
        float AngleRad = Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        Quaternion q = Quaternion.AngleAxis(AngleDeg - 90, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotateSpeed);
    }

    public void RotationNoTarget()
    {
        transform.Rotate(Vector3.forward * (rotateSpeed * Time.deltaTime));
    }

    public bool CanFire()
    {
        if (isStunned)
        {
            return false;
        }

        if (firstTake)
        {
            nextFireTime = Time.time + delay;
            firstTake = false;
        }

        return Time.time >= nextFireTime;
    }

    public void ProcessHit(int amount, DamageType type)
    {
        if (!isDamagable)
            return;

        lastDamageTime = Time.time;

        //Health -= amount;
        RaiseOnHitEnemyEvent(Health, MaxHealth);

        if (colouredFlash != null)
        {
            colouredFlash.Flash(Color.white);
        }

        //if (type == DamageType.Melee)
        //{
        //    UI_HUD.instance.RefillFlask(flaskRefillAmount);
        //}

        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected void RaiseOnHitEnemyEvent(float health, float maxHealth)
    {
        var eh = OnHitEnemy;
        if (eh != null)
            OnHitEnemy(health, maxHealth);
    }

    public void SpawnDamagePoints(int damage)
    {
        float x = UnityEngine.Random.Range(transform.position.x - 1f, transform.position.x + 1f);
        float y = UnityEngine.Random.Range(transform.position.y - 0.5f, transform.position.y + 0.5f);
        GameObject damageNum = Instantiate(damageNumberPrefab, new Vector3(x, y, 0), Quaternion.identity);
        damageNum.GetComponent<DamageNumber>().Setup(damage);
    }

    public void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        return;
    }

    public virtual void ResetStunResistance()
    {
        isStunned = false;
        currentStunResistance = stunResistance;
    }

    public void ProcessStunDamage(int amount, float stunDamageMod = 1)
    {
        isStunned = true;
    }
}