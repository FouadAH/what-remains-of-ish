using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdbeeEnemy_PlayerDetectedState : PlayerDetectedState
{
    private BirdbeeEnemy enemy;

    public BirdbeeEnemy_PlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, BirdbeeEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.aIPath.maxSpeed = stateData.chaseSpeed;
    }

    public override void Exit()
    {
        base.Exit();
    }

    float lastAttackTime;
    float lastDetectedTime;


    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (enemy.CheckPlayerInAttackLine() && lastAttackTime + stateData.attackCooldown <= Time.time)
        {
            lastAttackTime = Time.time;
            stateMachine.ChangeState(enemy.attackState);
        }
        else if (enemy.CheckPlayerInMaxAggroRadius())
        {
            lastDetectedTime = Time.time;
        }
        else if (!enemy.CheckPlayerInMaxAggroRadius() && lastDetectedTime + stateData.detectTime <= Time.time)
        {
            stateMachine.ChangeState(enemy.flyState);
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

        enemy.TargetPoint.transform.position = targetPos;

        int directionX = (enemy.transform.position.x < playerPos.x) ? 1 : -1;
        if (entity.facingDirection != directionX)
        {
            entity.Flip();
        }
    }
}
