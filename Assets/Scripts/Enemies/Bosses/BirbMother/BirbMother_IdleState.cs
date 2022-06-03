using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbMother_IdleState : IdleState
{
    private BirbMother enemy;
    public BirbMother_IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, BirbMother enemy) : base(etity, stateMachine, animBoolName, stateData)
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
        float random = Random.Range(0, 100);

        if (isIdleTimeOver)
        {
            //if (random <= 10)
            //{
            //    stateMachine.ChangeState(enemy.roarAttackState);
            //}
            //if (random > 10 && random <= 60)
            //{
            //    stateMachine.ChangeState(enemy.chargeAttackState);
            //}
            //else if (random > 60)
            //{
            //    stateMachine.ChangeState(enemy.downSpinAttackState);
            //}

        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
