using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemy_StunState : StunState
{
    private BasicMeleeAttackEnemy enemy;

    public BasicMeleeEnemy_StunState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_StunState stateData, BasicMeleeAttackEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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
        entity.SetVelocity(0);
        enemy.stunnedSprite.SetActive(true);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.stunnedSprite.SetActive(false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isStunTimeOver)
        {
            if (performCloseRangeAction)
            {
                stateMachine.ChangeState(enemy.meleeAttackState);
            }
            else
            {
                enemy.lookForPlayerState.SetTurnImmediately(true);
                stateMachine.ChangeState(enemy.lookForPlayerState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocity(0);
    }
}
