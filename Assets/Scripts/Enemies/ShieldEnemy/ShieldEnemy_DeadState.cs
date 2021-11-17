using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy_DeadState : DeadState
{
    private ShieldEnemy enemy;

    public ShieldEnemy_DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData, ShieldEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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
