using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogEnemy : Entity
{
    public IdleState idleState { get; private set; }
    public MoveState moveState { get; private set; }
    public PlayerDetectedState playerDetectedState { get; private set; }
    public DeadState deadState { get; private set; }

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

    public bool isProtected = false;

    public override void DamageHop(float velocity)
    {
        if (isProtected)
            return;

        base.DamageHop(velocity);
    }

    public override void ModifyHealth(int amount)
    {
        if (isProtected)
            return;

        base.ModifyHealth(amount);

        if (isDead)
        {
            stateMachine.ChangeState(deadState);
        }
    }
}
