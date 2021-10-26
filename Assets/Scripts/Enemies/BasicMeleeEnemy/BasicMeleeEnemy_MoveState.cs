using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeEnemy_MoveState : MoveState
{
    private BasicMeleeAttackEnemy enemy;

    public BasicMeleeEnemy_MoveState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, BasicMeleeAttackEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        firstTake = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    bool firstTake = true;
    float initialTime;
    float maxTimeBeforeIdle = 10f;
    float minTimeBeforeIdle = 4f;
    float timeBeforeIdle;
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
        if (firstTake)
        {
            firstTake = false;
            initialTime = Time.time;
            timeBeforeIdle = Random.Range(minTimeBeforeIdle, maxTimeBeforeIdle);
        }

        if ((enemy.CanSeePlayer() && entity.CheckPlayerInMinAgroRange()) || enemy.IsAggro)
        {
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if(isDetectingWall || !isDetectingLedge)
        {
            enemy.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(enemy.idleState);
        }
        //else if (Time.time >= initialTime + timeBeforeIdle)
        //{
        //    bool flipAfterIdle = Random.Range(0,100) >= 35;
        //    enemy.idleState.SetFlipAfterIdle(flipAfterIdle);
        //    stateMachine.ChangeState(enemy.idleState);
        //}
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
