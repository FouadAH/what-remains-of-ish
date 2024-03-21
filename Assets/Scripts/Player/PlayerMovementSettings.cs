using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object that holds all the players' movement values (movement speed, jump height, dash settings, health ...) 
/// </summary>
[CreateAssetMenu(menuName = "Player/Stats", fileName ="PlayerStats")]
public class PlayerMovementSettings : ScriptableObject
{
    [Header("Wall Climb Settings")]

    [SerializeField] private Vector2 wallJumpclimb;
    [SerializeField] private Vector2 wallLeap;

    [Header("Wall Slide Settings")]

    [SerializeField] private float wallSlideSpeedMax = 3;
    [SerializeField] private float wallStickTime = 1f;
    [SerializeField] private float timeToWallUnstick;

    [Header("Acceleration Settings")]

    [SerializeField] private float accelerationTimeAirborne = .2f;
    [SerializeField] private float accelerationTimeGrounded = .1f;
    [SerializeField] private float accelerationTimeSwing = .1f;

    [Header("Jump Settings")]

    [SerializeField] private float maxJumpHeight = 5f;
    [SerializeField] private float minJumpHeight = .5f;
    [SerializeField] private float timeToJumpApex = .4f;
    [SerializeField] private float jumpForceX = 25f;

    [Header("Jump Assist Settings")]

    [SerializeField] private float maxJumpAssistanceTime = .1f;
    [SerializeField] private int maxJumpBufferFrames = 10;

    [SerializeField] private float maxFallSpeed = -30f;

    [Header("Move Settings")]

    [SerializeField] private float moveSpeed = 6;

    [Header("Dash Settings")]

    [SerializeField] private float groundDashSpeed = 10;
    [SerializeField] private float airDashSpeed = 10;
    [SerializeField] private float dashSpeedTime = 0.1f;


    [SerializeField] private float iFrameTime = 1f;

    [SerializeField] private float aggroRange;

    [SerializeField] private float dashCooldown = .3f;

    [SerializeField] private float swingForce = 10f;

    public float TimeToJumpApex { get => timeToJumpApex; }
    public float MinJumpHeight { get => minJumpHeight; }
    public float MaxJumpHeight { get => maxJumpHeight; }
    public float JumpBoostX { get => jumpForceX; }
    public float MoveSpeed { get => moveSpeed; }
    public float AccelerationTimeAirborne { get => accelerationTimeAirborne; }
    public float AccelerationTimeGrounded { get => accelerationTimeGrounded; }
    public float WallSlideSpeedMax { get => wallSlideSpeedMax; }
    public float WallStickTime { get => wallStickTime; }
    public float TimeToWallUnstick { get => timeToWallUnstick; set => timeToWallUnstick = value; }
    public Vector2 WallJumpclimb { get => wallJumpclimb; }
    public Vector2 WallLeap { get => wallLeap; }
    public float DashSpeed { get => groundDashSpeed; }
    public float AirDashSpeed { get => airDashSpeed; }
    public float DashSpeedTime { get => dashSpeedTime; }
    public float DashCooldown { get => dashCooldown; }
    public float SwingForce { get => swingForce; }
    public float AccelerationTimeSwing { get => accelerationTimeSwing; }
    public float MaxJumpAssistanceTime { get => maxJumpAssistanceTime; set => maxJumpAssistanceTime = value; }
    public int MaxJumpBufferFrames { get => maxJumpBufferFrames; set => maxJumpBufferFrames = value; }
    public float MaxFallSpeed { get => maxFallSpeed; set => maxFallSpeed = value; }
}
