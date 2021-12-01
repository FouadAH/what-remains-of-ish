using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemy_PlayerDetectedState : PlayerDetectedState
{
    private BasicMeleeAttackEnemy enemy;

    public BasicMeleeEnemy_PlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, BasicMeleeAttackEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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

    float lastAttackTime = 0f;

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (performCloseRangeAction && CanMeleeAttack())
        {
            lastAttackTime = Time.time;
            stateMachine.ChangeState(enemy.meleeAttackState);
        }
        else if ((!enemy.CheckPlayerInMaxAgroRange() && !enemy.CheckPlayerInMinAgroRange()) || !enemy.CanSeePlayer())
        {
            enemy.IsAggro = false;
            stateMachine.ChangeState(enemy.moveState);
        }
        else if ((!isDetectingLedge || isDetectingWall))
        {
            enemy.idleState.SetFlipAfterIdle(true);
            enemy.IsAggro = false;
            stateMachine.ChangeState(enemy.idleState);
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Vector2 playerPos = GameManager.instance.playerCurrentPosition.position;

        Vector2 targetPositionRight = new Vector2(playerPos.x - stateData.playerOffset.x, playerPos.y + stateData.playerOffset.y);
        Vector2 targetPositionLeft = new Vector2(playerPos.x + stateData.playerOffset.x, playerPos.y + stateData.playerOffset.y);

        Vector2 targetPos = (Vector2.Distance(enemy.transform.position, targetPositionLeft) < Vector2.Distance(enemy.transform.position, targetPositionRight))
            ? targetPositionLeft : targetPositionRight;

        int directionX = (entity.transform.position.x < targetPos.x) ? 1 : -1;
        if (entity.facingDirection != directionX )
        {
            if(!performCloseRangeAction)
                entity.Flip();
        }   

        if (performCloseRangeAction)
        {
            entity.anim.SetBool("idle", true);
            entity.anim.SetBool("move", false);

            entity.SetVelocity(0);
        }
        else
        {
            entity.anim.SetBool("idle", false);
            entity.anim.SetBool("move", true);

            entity.SetVelocity(stateData.chaseSpeed);
        }
    }

    private bool CanMeleeAttack()
    {
        return (lastAttackTime + stateData.attackCooldown <= Time.time);
    }
}
