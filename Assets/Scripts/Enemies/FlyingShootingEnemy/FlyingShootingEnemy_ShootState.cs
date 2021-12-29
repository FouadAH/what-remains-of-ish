using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingShootingEnemy_ShootState : ShootState
{
    private FlyingShootingEnemy enemy;

    public FlyingShootingEnemy_ShootState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_RangedAttackState stateData, ProjectileController projectileController, FlyingShootingEnemy enemy) : base(etity, stateMachine, animBoolName, attackPosition, stateData, projectileController)
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

        //if (enemy.CanFire())
        //{
        //    enemy.nextFireTime = Time.time + enemy.fireRate;
        //    enemy.RaiseOnFireEvent();
        //}

        if (isAnimationFinished)
        {
            if (!enemy.CheckPlayerInMinAggroRadius())
            {
                stateMachine.ChangeState(enemy.flyState);
            }
            else
            {
                stateMachine.ChangeState(enemy.playerDetectedState);
            }
        }

        //if (!enemy.CheckPlayerInMinAggroRadius())
        //{
        //    stateMachine.ChangeState(enemy.flyState);
        //}
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
