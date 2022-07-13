using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HedgehogCorruptedEnemy_MoveState : MoveState
{
    private HedgehogCorruptedEnemy enemy;

    public HedgehogCorruptedEnemy_MoveState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, HedgehogCorruptedEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        if (enemy.isVertical)
        {
            entity.SetVelocityY(-enemy.facingDirection * stateData.movementSpeed);
        }
        else
        {
            base.PhysicsUpdate();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void DoChecks()
    {
        if (enemy.isVertical)
        {
            isDetectingLedge = entity.CheckWallFront();
            isDetectingWall = entity.CheckLedge();

        }
        else
        {
            isDetectingLedge = entity.CheckLedge();
            isDetectingWall = entity.CheckWallFront();
        }

        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (entity.CheckPlayerInMaxAgroRange() || entity.CheckPlayerInMinAgroRange())
        {
            stateMachine.ChangeState(enemy.shootState);
        }

        if (enemy.isVertical)
        {
            if (isDetectingWall)
            {
                enemy.idleState.SetFlipAfterIdle(true);
                stateMachine.ChangeState(enemy.idleState);
            }

        }
        else
        {
            if (isDetectingWall || !isDetectingLedge)
            {
                enemy.idleState.SetFlipAfterIdle(true);
                stateMachine.ChangeState(enemy.idleState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        if (enemy.isVertical)
        {
            entity.SetVelocityY(-enemy.facingDirection * stateData.movementSpeed);
        }
        else
        {
            base.PhysicsUpdate();
        }
    }
}
