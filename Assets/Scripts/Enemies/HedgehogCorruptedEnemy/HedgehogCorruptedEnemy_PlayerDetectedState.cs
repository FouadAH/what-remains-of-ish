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
    }

    public override void Exit()
    {
        base.Exit();
        enemy.isProtected = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.CanFire())
        {
            enemy.nextFireTime = Time.time + enemy.fireRate;
            enemy.RaiseOnFireEvent();
        }

        if (!entity.CheckPlayerInMinAgroRange())
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocity(0);
    }
}
