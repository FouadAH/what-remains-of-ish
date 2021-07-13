﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemy_MoveState : MoveState
{
    private BasicEnemy enemy;

    public BasicMeleeEnemy_MoveState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, BasicEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (entity.IsAggro)
        {
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if(isDetectingWall || !isDetectingLedge)
        {
            enemy.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
