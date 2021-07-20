using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootState : State
{
    protected bool isAnimationFinished;
    protected bool isPlayerInMinAgroRange;

    public ShootState(Entity etity, FiniteStateMachine stateMachine, string animBoolName) : base(etity, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void Enter()
    {
        base.Enter();

        isAnimationFinished = false;
        entity.SetVelocity(0f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
