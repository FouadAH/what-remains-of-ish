using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCharger_ChargeState : ChargeState
{
    private SpikeChargerEnemy enemy;

    public SpikeCharger_ChargeState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData, SpikeChargerEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LatePhysicsUpdate()
    {
        base.LatePhysicsUpdate();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isChargeTimeOver)
        {
            stateMachine.ChangeState(enemy.MoveState);
        }
        else if (isDetectingWall)
        {
            stateMachine.ChangeState(enemy.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}
