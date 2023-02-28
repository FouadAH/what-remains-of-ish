using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_JumpState : PlayerState
{

    public Player_JumpState(PlayerEntity entity, FiniteStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    bool jumpTrigger = false;
    public override void Enter()
    {
        base.Enter();
        jumpTrigger= true;
        //entity.jumpInputUp += OnJumpInputUp;
        //Jump(entity.playerSettings.MaxJumpVelocity, entity.playerSettings.JumpBoostX);
    }

    public override void Exit()
    {
        base.Exit();
        //entity.jumpInputUp -= OnJumpInputUp;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!entity.CheckGroundCollition())
        {
            stateMachine.ChangeState(entity.airborneState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if(jumpTrigger)
        {
            jumpTrigger = false;
            Jump(entity.playerSettings.MaxJumpVelocity, entity.playerSettings.JumpBoostX);
        }
    }

    void Jump(float jumpVelocityY, float jumpVelocityX)
    {
        entity.velocity.y = jumpVelocityY;

        if (!entity.CheckWallCollition())
        {
            //entity.SetVelocityX((entity.velocity.x) + (jumpVelocityX * entity.DirectionalInputX), entity.playerSettings.AccelerationTimeAirborne);
            entity.velocity.x += jumpVelocityX * entity.DirectionalInputX;
        }
    }

    void OnJumpInputUp()
    {
        if (entity.velocity.y > entity.playerSettings.MinJumpVelocity)
        {
            entity.velocity.y = entity.playerSettings.MinJumpVelocity;
        }
    }
}
