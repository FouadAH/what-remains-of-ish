using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RattlerEnemy_JumpStartState : AttackState
{
    private RattlerEnemy enemy;
    bool jumpTowards;

    public RattlerEnemy_JumpStartState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, RattlerEnemy enemy, bool jumpTowards) : base(etity, stateMachine, animBoolName, attackPosition)
    {
        this.enemy = enemy;
        this.jumpTowards = jumpTowards;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        isAnimationFinished = false;
        entity.SetVelocity(0f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            if (jumpTowards)
            {
                stateMachine.ChangeState(enemy.jumpTowardsState);
            }
            else 
            {
                stateMachine.ChangeState(enemy.jumpInPlaceState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
    }
}
