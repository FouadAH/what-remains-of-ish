using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_LandState : PlayerState
{
    public Player_LandState(PlayerEntity entity, FiniteStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {

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

        stateMachine.ChangeState(entity.groundedState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.velocity.y = 0;
    }

}
