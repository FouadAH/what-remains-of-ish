using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootState : AttackState
{
    protected D_RangedAttackState stateData;
    protected ProjectileController projectileController;

    public ShootState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RangedAttackState stateData, ProjectileController projectileController) : base(etity, stateMachine, animBoolName, attackPosition)
    {
        this.stateData = stateData;
        this.projectileController = projectileController;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void Enter()
    {
        base.Enter();
        entity.atsm.attackState = this;
        isAnimationFinished = false;
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
    }

    public override void FinishAttack()
    {
        isAnimationFinished = true;
    }
}
