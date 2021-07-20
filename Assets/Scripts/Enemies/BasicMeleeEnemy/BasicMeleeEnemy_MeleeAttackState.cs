using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemy_MeleeAttackState : MeleeAttackState
{
    private BasicMeleeAttackEnemy enemy;

    public BasicMeleeEnemy_MeleeAttackState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttack stateData, BasicMeleeAttackEnemy enemy) : base(etity, stateMachine, animBoolName, attackPosition, stateData)
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

    public override void FinishAttack()
    {
        base.FinishAttack();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            if (isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocity(0);
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }
}
