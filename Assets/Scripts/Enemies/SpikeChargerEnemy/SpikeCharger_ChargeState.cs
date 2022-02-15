using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCharger_ChargeState : ChargeState
{
    private SpikeChargerEnemy enemy;

    public SpikeCharger_ChargeState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData, SpikeChargerEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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
        enemy.accelerationTimeGrounded = 0.4f;
        enemy.chargeDustParticles.Play();
    }

    public override void Exit()
    {
        base.Exit();
        enemy.chargeDustParticles.Stop();
        enemy.accelerationTimeGrounded = 0.1f;
        
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
            stateMachine.ChangeState(enemy.MoveState);
        }
        
        if (entity.CheckWallFront())
        {
            Debug.Log("WALLLLL");
            enemy.SetVelocityX(0);
            stateMachine.ChangeState(enemy.IdleState);
        }
    }

    public override void PhysicsUpdate()
    {
        if (entity.CheckWallFront())
        {
            enemy.SetVelocityX(0);
            return;
        }

        Vector2 playerPos = GameManager.instance.playerCurrentPosition.position;
        int directionX = (entity.transform.position.x < playerPos.x) ? 1 : -1;

        if(directionX != entity.facingDirection)
        {
            entity.Flip();
        }

        entity.SetVelocity(stateData.chargeSpeed);

        //Debug.Log(enemy.chargeSpeedCurve.Evaluate(Time.time - startTime));

    }

}
