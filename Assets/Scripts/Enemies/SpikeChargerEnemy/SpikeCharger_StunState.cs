using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCharger_StunState : StunState
{
    private SpikeChargerEnemy enemy;

    public SpikeCharger_StunState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData, SpikeChargerEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isStunTimeOver)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
