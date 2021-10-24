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

    float lastDetectedTime;

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (performCloseRangeAction)
        {
            stateMachine.ChangeState(enemy.meleeAttackState);
        }
        else if (!enemy.CheckPlayerInMaxAggroRadius() && lastDetectedTime + stateData.detectTime <= Time.time)
        {
            enemy.IsAggro = false;
            stateMachine.ChangeState(enemy.lookForPlayerState);
        }
        else if (!isDetectingLedge || isDetectingWall)
        {
            entity.Flip();
            enemy.IsAggro = false;
            stateMachine.ChangeState(enemy.idleState);
        }
        else if (enemy.CheckPlayerInMaxAggroRadius())
        {
            lastDetectedTime = Time.time;
        }

    }

    bool CanSeePlayer()
    {
        RaycastHit2D hit =  Physics2D.Linecast(enemy.transform.position, GameManager.instance.playerCurrentPosition.position, entity.entityData.whatIsGround);
        return hit.collider != null;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        int directionX = (entity.transform.position.x < GameManager.instance.player.transform.position.x) ? 1 : -1;
        if(entity.facingDirection != directionX){
            entity.Flip();
        }
        entity.SetVelocity(stateData.chaseSpeed);
    }
}
