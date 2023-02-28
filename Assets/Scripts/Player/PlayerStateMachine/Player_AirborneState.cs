using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player_AirborneState : PlayerState
{
    bool isGrounded;

    public Player_AirborneState(PlayerEntity entity, FiniteStateMachine stateMachine, string animBoolName) : base(entity, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
        isGrounded = entity.CheckGroundCollition();
        entity.CheckIfShouldFlip();
    }

    public override void Enter()
    {
        base.Enter();

        cayoteTimer = 0;
        jumpBufferCounter = 100;

        entity.jumpInputDown += OnJumpInputDown;
        entity.jumpInputUp += OnJumpInputUp;
    }

    public override void Exit()
    {
        base.Exit();
        entity.velocity.y = 0;
        entity.jumpInputDown -= OnJumpInputDown;
        entity.jumpInputUp -= OnJumpInputUp;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        JumpBuffer();
        //if (isGrounded)
        //{
        //    stateMachine.ChangeState(entity.landState);
        //}
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if (!isGrounded)
        {
            float targetVelocityX = entity.playerSettings.MoveSpeed * entity.DirectionalInputX;
            float smoothTime = entity.playerSettings.AccelerationTimeAirborne;
            float smoothTimeReduced = entity.playerSettings.AccelerationTimeAirborne;

            if (Mathf.Abs(entity.velocity.x) > Mathf.Abs(targetVelocityX) && Mathf.Sign(entity.velocity.x) == entity.DirectionalInput.x)
            {
                entity.SetVelocityX(targetVelocityX, 0.3f);
            }
            else if (Mathf.Abs(entity.velocity.x) > 8 && Mathf.Sign(entity.velocity.x) == entity.DirectionalInput.x * -1)
            {
                entity.SetVelocityX(0, smoothTime);
            }
            else
            {
                entity.SetVelocityX(targetVelocityX, smoothTime);
            }

            entity.velocity.y += entity.playerSettings.Gravity * Time.deltaTime;
            entity.velocity.y = Mathf.Clamp(entity.velocity.y, entity.playerSettings.Gravity, 1000);
        }
    }

    bool jumpTrigger;

    void OnJumpInputDown()
    {
        jumpBufferCounter = 0;
    }

    private float cayoteTimer = 0f;

    private int jumpBufferCounter = 100;
    private int MaxJumpBufferFrames = 4;

    public bool CheckJumpBuffer()
    {
        return jumpBufferCounter < MaxJumpBufferFrames;
    }

    void JumpBuffer()
    {
        cayoteTimer += Time.deltaTime;

        if (CheckJumpBuffer())
        {
            Debug.Log("JUMP BUFFERED");

            if (CheckCanJump() || isGrounded)
            {
                jumpTrigger = false;
                cayoteTimer = entity.playerSettings.MaxJumpAssistanceTime;
                Debug.Log("JUMP");

                stateMachine.ChangeState(entity.jumpState);
                return;
            }
        }

        if (isGrounded)
        {
            stateMachine.ChangeState(entity.landState);
        }
    }

    bool CheckCanJump()
    {
        return cayoteTimer < entity.playerSettings.MaxJumpAssistanceTime;
    }

    void OnJumpInputUp()
    {
        if (entity.velocity.y > entity.playerSettings.MinJumpVelocity)
        {
            entity.velocity.y = entity.playerSettings.MinJumpVelocity;
        }
    }
}
