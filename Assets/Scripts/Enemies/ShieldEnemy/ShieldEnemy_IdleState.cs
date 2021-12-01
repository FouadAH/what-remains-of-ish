using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy_IdleState : IdleState
{
    private ShieldEnemy enemy;
    public ShieldEnemy_IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, ShieldEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        entity.SetVelocityX(0);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (((enemy.CanSeePlayer() && (entity.CheckPlayerInMinAgroRange() || entity.CheckPlayerInMaxAgroRange())) || enemy.IsAggro))
        {
            Debug.Log("Player detected idle");
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if (isIdleTimeOver)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocityX(0);
    }
}
