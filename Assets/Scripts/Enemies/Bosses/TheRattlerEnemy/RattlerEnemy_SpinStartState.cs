using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RattlerEnemy_SpinStartState : AttackState
{
    private RattlerEnemy enemy;

    public RattlerEnemy_SpinStartState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, RattlerEnemy enemy) : base(etity, stateMachine, animBoolName, attackPosition)
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
            stateMachine.ChangeState(enemy.spinAttackState);
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
