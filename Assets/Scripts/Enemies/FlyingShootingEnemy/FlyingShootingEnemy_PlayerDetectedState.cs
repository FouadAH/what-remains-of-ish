using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingShootingEnemy_PlayerDetectedState : PlayerDetectedState
{
    private FlyingShootingEnemy enemy;

    public FlyingShootingEnemy_PlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, FlyingShootingEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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

    float lastDetectedTime;

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (enemy.CanFire() && enemy.CheckPlayerInAttackLine())
        {
            enemy.nextFireTime = Time.time + enemy.fireRate;
            stateMachine.ChangeState(enemy.shootState);
        }
        else if (enemy.CheckPlayerInMaxAggroRadius())
        {
            lastDetectedTime = Time.time;
        }
        else if (!enemy.CheckPlayerInMaxAggroRadius() && lastDetectedTime + stateData.detectTime <= Time.time)
        {
            enemy.IsAggro = false;
            stateMachine.ChangeState(enemy.flyState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Vector2 playerPos = enemy.playerRuntimeDataSO.playerPosition;

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
