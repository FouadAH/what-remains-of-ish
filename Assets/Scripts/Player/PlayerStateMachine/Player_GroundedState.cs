using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player_GroundedState : PlayerState
{
    bool isGrounded;
    bool isMoving;
    bool jumpTrigger;

    public Player_GroundedState(PlayerEntity entity, FiniteStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {

    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = entity.CheckGroundCollition();
        isMoving = entity.DirectionalInput.x != 0;

        entity.CheckIfShouldFlip();
    }

    public override void Enter()
    {
        base.Enter();

        entity.jumpInputDown += OnJumpInputDown;
    }

    public override void Exit()
    {
        base.Exit();

        entity.jumpInputDown -= OnJumpInputDown;
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

        if (entity.CheckVeritcalCollisions())
        {
            entity.velocity.y = 0;
        }

        if (jumpTrigger)
        {
            jumpTrigger = false;
            stateMachine.ChangeState(entity.jumpState);
            return;
        }

        if (!entity.CheckGroundCollition())
        {
            stateMachine.ChangeState(entity.airborneState);
            return;
        }

        if (entity.DirectionalInput.x != 0)
        {
            stateMachine.ChangeState(entity.moveState);
            return;
        }
        else
        {
            stateMachine.ChangeState(entity.idleState);
            return;
        }
    }

    void OnJumpInputDown()
    {
        if (entity.CheckCanJump())
        {
            jumpTrigger = true;
        }
    }

    void HandleMaxSlope()
    {
        //Debug.Log(controller.collitions.below);
        if (entity.CheckVeritcalCollisions())
        {
            entity.velocity.y = 0;
        }
    }
}
