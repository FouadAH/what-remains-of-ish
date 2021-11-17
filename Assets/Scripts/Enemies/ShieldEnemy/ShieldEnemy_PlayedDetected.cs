using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy_PlayedDetected : PlayerDetectedState
{
    private ShieldEnemy enemy;

    public ShieldEnemy_PlayedDetected(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, ShieldEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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
            stateMachine.ChangeState(enemy.blockState);
        }
        else if (enemy.PlayerCheckAbove())
        {
            stateMachine.ChangeState(enemy.blockStateUp);
        }   
        else if ((!enemy.CheckPlayerInMaxAggroRadius() && lastDetectedTime + stateData.detectTime <= Time.time) || !enemy.CanSeePlayer())
        {
            enemy.IsAggro = false;
            stateMachine.ChangeState(enemy.moveState);
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

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Vector2 playerPos = GameManager.instance.playerCurrentPosition.position;

        Vector2 targetPositionRight = new Vector2(playerPos.x - stateData.playerOffset.x, playerPos.y + stateData.playerOffset.y);
        Vector2 targetPositionLeft = new Vector2(playerPos.x + stateData.playerOffset.x, playerPos.y + stateData.playerOffset.y);

        Vector2 targetPos = (Vector2.Distance(enemy.transform.position, targetPositionLeft) < Vector2.Distance(enemy.transform.position, targetPositionRight))
            ? targetPositionLeft : targetPositionRight;

        int directionX = (entity.transform.position.x < targetPos.x) ? 1 : -1;
        if (entity.facingDirection != directionX)
        {
            entity.Flip();
        }
        entity.SetVelocity(stateData.chaseSpeed);
    }
}
