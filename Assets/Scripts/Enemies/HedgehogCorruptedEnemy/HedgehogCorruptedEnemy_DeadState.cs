using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogCorruptedEnemy_DeadState : DeadState
{
    private HedgehogCorruptedEnemy enemy;

    public HedgehogCorruptedEnemy_DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData, HedgehogCorruptedEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocity(0);
    }
}
