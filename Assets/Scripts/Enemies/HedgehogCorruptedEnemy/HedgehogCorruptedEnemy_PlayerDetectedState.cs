using System;
using UnityEngine;

public class HedgehogCorruptedEnemy_PlayerDetectedState : PlayerDetectedState
{
    private HedgehogCorruptedEnemy enemy;

    public HedgehogCorruptedEnemy_PlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, HedgehogCorruptedEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.isProtected = true;
        firstTake = true;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.isProtected = false;
    }

    float initialWaitTime = 0.5f;
    float lastDetectedTime;
    float waitTime = 1f;
    bool firstTake = true;
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (firstTake)
        {
            enemy.nextFireTime = Time.time + initialWaitTime;
            firstTake = false;
        }

        if (CanFire())
        {
            enemy.nextFireTime = Time.time + enemy.fireRate;
            enemy.RaiseOnFireEvent();
        }
        if (entity.CheckPlayerInMinAgroRange())
        {
            lastDetectedTime = Time.time;
        }
        else if (!entity.CheckPlayerInMinAgroRange() && lastDetectedTime + waitTime <= Time.time)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocity(0);
    }

    public bool CanFire()
    {
        return Time.time >= enemy.nextFireTime;
    }
}
