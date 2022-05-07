using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogCorruptedEnemy : Entity, FiringAI
{
    public IdleState idleState { get; private set; }
    public MoveState moveState { get; private set; }
    public PlayerDetectedState playerDetectedState { get; private set; }
    public DeadState deadState { get; private set; }
    float FiringAI.fireRate { get => fireRate; set => _ = fireRate; }
    float FiringAI.nextFireTime { get => nextFireTime; set => _ = nextFireTime; }

    [Header("Projectile Firing Settings")]
    public float fireRate;
    [HideInInspector] public float nextFireTime;
    public event Action OnFire = delegate { };

    [HideInInspector] public bool isProtected = false;

    [Header("States")]
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_PlayerDetected playerDetectedData;
    [SerializeField] private D_DeadState deadStateData;

    public bool isVertical = false;

    public override void Start()
    {
        base.Start();

        moveState = new HedgehogCorruptedEnemy_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new HedgehogCorrutedEnemy_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new HedgehogCorruptedEnemy_PlayerDetectedState(this, stateMachine, "block", playerDetectedData, this);
        deadState = new HedgehogCorruptedEnemy_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(moveState);
    }

    public override void ProcessHit(int amount, DamageType type)
    {
        if (isProtected)
            return;

        base.ProcessHit(amount, type);

        if (isDead && stateMachine.currentState != deadState)
        {
            stateMachine.ChangeState(deadState);
        }
    }

    public override void LoadDefaultData()
    {
        base.LoadDefaultData();
    }

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
