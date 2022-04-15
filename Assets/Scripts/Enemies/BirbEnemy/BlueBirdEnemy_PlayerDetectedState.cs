using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBirdEnemy_PlayerDetectedState : PlayerDetectedState
{
    private BirbEnemy enemy;

    public BlueBirdEnemy_PlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_PlayerDetected stateData, BirbEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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
        
        if (enemy.CheckPlayerInMaxAggroRadius())
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

        Vector2 playerPos = GameManager.instance.player.transform.position;
        Vector2 targetPos = new Vector2(playerPos.x, playerPos.y + 0.5f);
        enemy.TargetPoint.transform.position = targetPos;

        int directionX = (enemy.transform.position.x < playerPos.x) ? 1 : -1;
        if (entity.facingDirection != directionX)
        {
            entity.Flip();
        }
    }
}
