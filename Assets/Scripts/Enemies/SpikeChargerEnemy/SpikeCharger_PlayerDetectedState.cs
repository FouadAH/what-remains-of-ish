using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeCharger_PlayerDetectedState : AttackState
{
    private SpikeChargerEnemy enemy;

    public SpikeCharger_PlayerDetectedState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPos, SpikeChargerEnemy enemy) : base(etity, stateMachine, animBoolName, attackPos)
    {
        this.enemy = enemy;
    }

     
    public override void Enter()
    {
        base.Enter();
        enemy.accelerationTimeGrounded = 0.5f;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.accelerationTimeGrounded = 0.1f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (entity.CheckPlayerInMinAgroRange())
        {
            entity.Flip();
        }
        if (isAnimationFinished)
        {
            stateMachine.ChangeState(enemy.ChargeState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocityX(0);
    }
}
