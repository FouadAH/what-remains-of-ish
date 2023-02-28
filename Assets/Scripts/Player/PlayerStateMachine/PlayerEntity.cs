using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Windows;
using static Pathfinding.SimpleSmoothModifier;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerEntity : MonoBehaviour, EntityBase
{
    public FiniteStateMachine stateMachine;

    public Player_IdleState idleState { get; private set; }
    public Player_AirborneState airborneState { get; private set; }
    public Player_LandState landState { get; private set; }
    public Player_GroundedState groundedState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_MoveState moveState { get; private set; }

    public Animator anim { get; set; }
    public Player_Input playerInput { get; private set; }
    public PlayerMovementSettings playerSettings;
    public PlayerRuntimeDataSO playerRuntimeData;

    //[Header("States")]
    //[SerializeField]
    //private D_IdleState idleStateData;
    //[SerializeField]
    //private D_MoveState moveStateData;
    //[SerializeField]
    //private D_DeadState deadStateData;

    [Header("Debug")]
    public bool debug;
    public TMP_Text stateDebugText;

    private Controller_2D controller;
    private PlayerAnimations playerAnimations;
    

    public Vector2 velocity;

    private float velocityXSmoothing;
    private float velocityYSmoothing;

    private float cayoteTimer = 0f;
    private int jumpBufferCounter = 10;

    public float DirectionalInputX { get => playerInput.directionalInput.x; }
    public float DirectionalInputY { get => playerInput.directionalInput.y; }
    public Vector2 DirectionalInput { get => playerInput.directionalInput; }

    public int FacingDirection { get; private set; }

    public Action jumpInputUp;
    public Action jumpInputDown;

    public void Awake()
    {
        if (stateMachine == null)
            stateMachine = new FiniteStateMachine();

        anim = transform.GetComponent<Animator>();
        controller = transform.GetComponent<Controller_2D>();
        playerInput = transform.GetComponent<Player_Input>();

        playerAnimations = new PlayerAnimations(anim, transform);

        InitStates();
    }

    private void OnEnable()
    {
        playerInput.OnJumpDown += OnJumpInputDown;
        playerInput.OnJumpUp += OnJumpInputUp;

        //playerInput.OnDash += OnDashInput;
        //playerInput.OnDashUp += OnDashInputUp;

        //playerInput.OnTeleport += OnBoomerangDashInput;

        //playerInput.OnDownAttack += PlayerInput_OnDownAttack;
    }

    private void OnDisable()
    {
        playerInput.OnJumpDown -= OnJumpInputDown;
        playerInput.OnJumpUp -= OnJumpInputUp;

        //playerInput.OnDash -= OnDashInput;
        //playerInput.OnDashUp -= OnDashInputUp;

        //playerInput.OnTeleport -= OnBoomerangDashInput;

        //playerInput.OnDownAttack -= PlayerInput_OnDownAttack;
    }
    void InitStates()
    {
        moveState = new Player_MoveState(this, stateMachine, "move");
        idleState = new Player_IdleState(this, stateMachine, "idle", null);
        airborneState = new Player_AirborneState(this, stateMachine, "idle");
        jumpState = new Player_JumpState(this, stateMachine, "jump");
        landState = new Player_LandState(this, stateMachine, "land");
        groundedState = new Player_GroundedState(this, stateMachine, "idle");

        stateMachine.Initialize(airborneState);
    }

    public void Update()
    {
        stateMachine.currentState.LogicUpdate();
    }

    public void FixedUpdate()
    {
        JumpBuffer();
        JumpAssist();

        controller.Move(velocity * Time.smoothDeltaTime, new Vector2(-1, -1));

        stateMachine.currentState.PhysicsUpdate();

        if (debug)
        {
            string state = stateMachine.currentState.ToString().Split('_')[1];
            stateDebugText.SetText(state);
        }
        //Debug.Log("Collisions bellow: " + controller.collitions.below);

        playerRuntimeData.velocity = velocity;
    }

    public void LateUpdate()
    {
        stateMachine.currentState.LatePhysicsUpdate();
    }

    public void SetVelocity(float velocityX, float velocityY)
    {
        velocity.x = velocityX;
        velocity.y = velocityY;
    }

    public void SetVelocityX(float targetVelocityX, float smoothTime)
    {
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
    }

    public void SetVelocityY(float targetVelocityY, float smoothTime)
    {
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, smoothTime);
    }


    public bool CheckGroundCollition()
    {
        return (controller.collitions.below);
    }

    public bool CheckCeilingCollition()
    {
        return controller.collitions.above;
    }

    public bool CheckVeritcalCollisions()
    {
        return controller.collitions.above || controller.collitions.below;
    }
    public bool CheckWallCollition()
    {
        return controller.collitions.left || controller.collitions.right;
    }

    public bool CheckMaxSlope()
    {
        return controller.collitions.slidingDownMaxSlope;
    }

    public bool CheckCanJump()
    {
        //Debug.Log(cayoteTimer < playerSettings.MaxJumpAssistanceTime && jumpBufferCounter < playerSettings.MaxJumpBufferFrames);
        return playerInput.CheckJumpBuffer();
    }

    void JumpAssist()
    {
        if (!CheckGroundCollition())
        {
            cayoteTimer += Time.deltaTime;
        }
        else
        {
            cayoteTimer = 0;
        }
    }

    void JumpBuffer()
    {
        if (jumpBufferCounter < playerSettings.MaxJumpBufferFrames)
        {
            jumpBufferCounter += 1;
        }
    }

    public void OnJumpInputDown()
    {
        jumpBufferCounter = 0;
        jumpInputDown?.Invoke();
    }

    public void OnJumpInputUp()
    {
        jumpInputUp?.Invoke();
    }

    public void CheckIfShouldFlip()
    {
        int inputX = (int)DirectionalInputX;

        if (inputX != 0 && inputX != FacingDirection)
        {
            FacingDirection *= -1;

            if (inputX < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y, -1);
            }
            else if (inputX > 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, -1);
            }
        }
    }
}
