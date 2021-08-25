using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogEnemy : Entity, IBaseStats
{
    public IdleState idleState { get; private set; }
    public MoveState moveState { get; private set; }
    public PlayerDetectedState playerDetectedState { get; private set; }
    public DeadState deadState { get; private set; }

    [SerializeField] private int meleeDamage;
    [SerializeField] private int hitKnockbackAmount;
    public int MeleeDamage { get => meleeDamage; set => meleeDamage = value; }
    public int HitKnockbackAmount { get => hitKnockbackAmount; set => hitKnockbackAmount = value; }

    [Header("States")]
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_PlayerDetected playerDetectedData;
    [SerializeField] private D_DeadState deadStateData;

    public override void Start()
    {
        base.Start();

        moveState = new HedgehogEnemy_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new HedgehogEnemy_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new HedgehogEnemy_PlayerDetectedState(this, stateMachine, "block", playerDetectedData, this);
        deadState = new HedgehogEnemy_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(moveState);

    }

    public float gravity = -1;

    public bool isProtected = false;
    public override void ModifyHealth(int amount)
    {
        if (isProtected)
            return;

        Health -= amount;
        RaiseOnHitEnemyEvent(Health, MaxHealth);

        if (Health <= 0)
        {
            isDead = true;
            UI_HUD.instance.RefillFlask(entityData.flaskReffilAmount);

        }
        else
        {
            //Aggro();
            SpawnDamagePoints(amount);
            anim.SetTrigger("Hit");
        }

        if (isDead)
        {
            stateMachine.ChangeState(deadState);
        }
    }

    public void KnockbackOnHit(int amount, float dirX, float dirY)
    {
        Debug.Log("KnockbackOnHit");
    }
}
