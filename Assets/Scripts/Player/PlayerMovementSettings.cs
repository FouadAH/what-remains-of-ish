using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable object that holds all the players' movement values (movement speed, jump height, dash settings, health ...) 
/// </summary>
[CreateAssetMenu(menuName = "Player/Stats", fileName ="PlayerStats")]
public class PlayerMovementSettings : ScriptableObject
{
    [SerializeField] private Vector2 wallJumpclimb;
    [SerializeField] private Vector2 wallJumpOff;
    [SerializeField] private Vector2 wallLeap;

    [SerializeField] private float wallSlideSpeedMax = 3;
    [SerializeField] private float wallStickTime = 1f;
    [SerializeField] private float timeToWallUnstick;

    [SerializeField] private float accelerationTimeAirborne = .2f;
    [SerializeField] private float accelerationTimeGrounded = .1f;
    [SerializeField] private float accelerationTimeSwing = .1f;

    [SerializeField] private float maxJumpHeight = 5f;
    [SerializeField] private float minJumpHeight = .5f;
    [SerializeField] private float timeToJumpApex = .4f;

    [SerializeField] private float moveSpeed = 6;
    [SerializeField] private float dashFactor = 10;

    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;

    [SerializeField] private float attackCooldown;
    [SerializeField] private Vector2 swordKnockback;
    [SerializeField] private Vector2 damageKnockback;

    [SerializeField] private float iFrameTime = 1f;
 
    [SerializeField] private float maxHealth;

    [SerializeField] private float aggroRange;

    [SerializeField] private float dashCooldown = .3f;

    [SerializeField] private float swingForce = 10f;

    public float TimeToJumpApex { get => timeToJumpApex; }
    public float MinJumpHeight { get => minJumpHeight; }
    public float MaxJumpHeight { get => maxJumpHeight; }
    public float MoveSpeed { get => moveSpeed; }
    public float AccelerationTimeAirborne { get => accelerationTimeAirborne; }
    public float AccelerationTimeGrounded { get => accelerationTimeGrounded; }
    public float WallSlideSpeedMax { get => wallSlideSpeedMax; }
    public float WallStickTime { get => wallStickTime; }
    public float TimeToWallUnstick { get => timeToWallUnstick; set => timeToWallUnstick = value; }
    public Vector2 WallJumpclimb { get => wallJumpclimb; }
    public Vector2 WallJumpOff { get => wallJumpOff; }
    public Vector2 WallLeap { get => wallLeap; }
    public float DashFactor { get => dashFactor; }
    public float DashCooldown { get => dashCooldown; }
    public float SwingForce { get => swingForce; }
    public float AccelerationTimeSwing { get => accelerationTimeSwing; }
}
