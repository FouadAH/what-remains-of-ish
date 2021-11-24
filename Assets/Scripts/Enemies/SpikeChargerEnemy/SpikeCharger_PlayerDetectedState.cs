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
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            stateMachine.ChangeState(enemy.chargeState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocityX(0);
    }
}
