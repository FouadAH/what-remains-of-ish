using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemy_IdleState : IdleState
{
    private BasicMeleeAttackEnemy enemy;
    public BasicMeleeEnemy_IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, BasicMeleeAttackEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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

        if ((entity.CheckPlayerInMaxAgroRange() || isPlayerInMinAgroRange || enemy.IsAggro) && !entity.CheckLedge())
        {
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
    }
}
