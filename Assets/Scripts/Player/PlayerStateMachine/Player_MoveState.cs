using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player_MoveState : Player_GroundedState
{
    public Player_MoveState(PlayerEntity entity, FiniteStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
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
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        float targetVelocityX = entity.playerSettings.MoveSpeed * entity.DirectionalInputX;
        float smoothTime = (entity.CheckGroundCollition() ? entity.playerSettings.AccelerationTimeGrounded : entity.playerSettings.AccelerationTimeAirborne);

        entity.SetVelocityX(targetVelocityX, smoothTime);
    }
}
