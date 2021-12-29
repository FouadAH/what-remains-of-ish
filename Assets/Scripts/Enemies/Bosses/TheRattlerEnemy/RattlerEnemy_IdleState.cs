using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RattlerEnemy_IdleState : IdleState
{
    private RattlerEnemy enemy;
    public RattlerEnemy_IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, RattlerEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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

    int spitAttackTimes;

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        float random = Random.Range(0, 100);

        if (isIdleTimeOver)
        {
            if (random <= 33)
                stateMachine.ChangeState(enemy.jumpState);
            else if (random > 33 && random <= 66)
                stateMachine.ChangeState(enemy.spinAttackState);
            else if(random >= 100)
                stateMachine.ChangeState(enemy.spitState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
