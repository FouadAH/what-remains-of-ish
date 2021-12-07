using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCharger_MoveState : MoveState
{
    private SpikeChargerEnemy enemy;

    public SpikeCharger_MoveState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_MoveState stateData, SpikeChargerEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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

        if (entity.CheckPlayerInMaxAgroRange())
        {
            stateMachine.ChangeState(enemy.PlayerDetectedState);
        }
        else if(entity.CheckPlayerInMinAgroRange())
        {
            entity.Flip();
            stateMachine.ChangeState(enemy.PlayerDetectedState);
        }
        else if (isDetectingWall || !isDetectingLedge)
        {
            enemy.IdleState.SetFlipAfterIdle(true);
            stateMachine.ChangeState(enemy.IdleState);
        }
    }

    private bool CheckPlayerPos()
    {
        Vector2 playerPos = GameManager.instance.playerCurrentPosition.position;
        int directionX = (entity.transform.position.x < playerPos.x) ? 1 : -1;
        return (entity.facingDirection != directionX);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
