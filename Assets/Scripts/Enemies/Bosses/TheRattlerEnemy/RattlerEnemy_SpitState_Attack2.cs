using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RattlerEnemy_SpitState_Attack2 : ShootState
{
    private RattlerEnemy enemy;

    public RattlerEnemy_SpitState_Attack2(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RangedAttackState stateData, ProjectileController projectileController, RattlerEnemy enemy) : base(etity, stateMachine, animBoolName, attackPosition, stateData, projectileController)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
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

        //projectileController.RaiseOnFireEvent();
        if(isAnimationFinished)
            stateMachine.ChangeState(enemy.idleState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocity(0f);
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
        projectileController.RaiseOnFireEvent();
    }
}
