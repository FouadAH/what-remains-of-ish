using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy_BlockUpState : BlockState
{
    private ShieldEnemy enemy;
    protected bool isBlockOver;
    protected int hitAmount;

    public ShieldEnemy_BlockUpState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_BlockState stateData, ShieldEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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
        enemy.isProtected = true;
        isBlockOver = false;
        hitAmount = 0;
        enemy.Hit += Enemy_Hit;
    }

    private void Enemy_Hit()
    {
        hitAmount++;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.isProtected = false;

    }

    float lastDetectedTime;

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (hitAmount >= 1)
        {
            stateMachine.ChangeState(enemy.meleeAttackState);
        }

        if (enemy.CheckPlayerInCloseRangeAction())
        {
            stateMachine.ChangeState(enemy.blockState);
        }
        else if (enemy.PlayerCheckBehind())
        {
            entity.Flip();
            stateMachine.ChangeState(enemy.blockState);
        }

        if (enemy.CheckPlayerInMinAggroRadius())
        {
            lastDetectedTime = Time.time;
        }
        else if (Time.time >= lastDetectedTime + stateData.blockTime)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocity(0);
    }
}
