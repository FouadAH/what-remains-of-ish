using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogEnemy_PlayerDetectedState : PlayerDetectedState
{
    private HedgehogEnemy enemy;

    public HedgehogEnemy_PlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, HedgehogEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        //enemy.hurtbox
        enemy.isProtected = true;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.isProtected = false;
    }

    float lastDetectedTime;
    float waitTime = 0.2f;

    public override void LogicUpdate()
    {
        base.LogicUpdate();

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
}
