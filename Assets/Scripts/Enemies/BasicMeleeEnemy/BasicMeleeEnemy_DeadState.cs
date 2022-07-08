using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemy_DeadState : DeadState
{
    private BasicMeleeAttackEnemy enemy;

    public BasicMeleeEnemy_DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData, BasicMeleeAttackEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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
        Debug.Log("Dead state PU");
        entity.SetVelocity(0);
    }
}
