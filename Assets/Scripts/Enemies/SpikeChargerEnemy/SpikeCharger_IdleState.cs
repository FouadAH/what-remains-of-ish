using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCharger_IdleState : IdleState
{
    private SpikeChargerEnemy enemy;
    public SpikeCharger_IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, SpikeChargerEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if ((entity.CheckPlayerInMaxAgroRange() || enemy.IsAggro))
        {
            stateMachine.ChangeState(enemy.PlayerDetectedState);
        }
        else if (isPlayerInMinAgroRange)
        {
            entity.Flip();
            stateMachine.ChangeState(enemy.PlayerDetectedState);
        }
        else if (isIdleTimeOver)
        {
            stateMachine.ChangeState(enemy.MoveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocity(0);
    }
}
