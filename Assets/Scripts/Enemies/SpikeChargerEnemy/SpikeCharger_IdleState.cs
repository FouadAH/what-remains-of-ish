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
            if (!entity.CheckLedge())
            {
                return;
            }
            else
            {
                stateMachine.ChangeState(enemy.chargeState);
            }
        }
        else if (isPlayerInMinAgroRange)
        {
            entity.Flip();
            stateMachine.ChangeState(enemy.chargeState);
        }
        else if (isIdleTimeOver)
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
