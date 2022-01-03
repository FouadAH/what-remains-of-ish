using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RattlerEnemy_SpinState : ChargeState
{
    private RattlerEnemy enemy;
    int directionX;

    public RattlerEnemy_SpinState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData, RattlerEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LatePhysicsUpdate()
    {
        base.LatePhysicsUpdate();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();


        if (isChargeTimeOver)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        Vector2 playerPos = GameManager.instance.playerCurrentPosition.position;
        directionX = (entity.transform.position.x < playerPos.x) ? 1 : -1;
        entity.SetVelocity(stateData.chargeSpeed * directionX);

        //if (enemy.CheckWall() && !isCollidingWithWall)
        //{
        //    isCollidingWithWall = true;
        //    float wallBounceForce = (enemy.CheckWallFront()) ? -100 : 100;
        //    entity.SetVelocity(wallBounceForce);
        //}
        //else
        //{
        //    isCollidingWithWall = false;
        //    entity.SetVelocity(stateData.chargeSpeed * directionX);
        //}
    }
}
