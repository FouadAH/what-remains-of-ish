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
    float maxTimeBeforeIdle = 6f;
    float minTimeBeforeIdle = 3f;
    float chanceToFlipAfterIdle = 80f;
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

        if ((enemy.CanSeePlayer() && (entity.CheckPlayerInMinAgroRange() || entity.CheckPlayerInMaxAgroRange())) || enemy.IsAggro)
        {
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if(isDetectingWall || !isDetectingLedge)
        {
            enemy.idleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(enemy.idleState);
        }
        else if (initialTime + timeBeforeIdle <= Time.time)
        {
            bool flipAfterIdle = Random.Range(0, 100) >= chanceToFlipAfterIdle;
            enemy.idleState.SetFlipAfterIdle(flipAfterIdle);
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
