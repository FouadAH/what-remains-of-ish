﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class responsible for handling player movement calculations</summary>
public class PlayerMovement : MonoBehaviour
{
    public PlayerMovementSettings playerSettings;

    Controller_2D controller;
    Player_Input playerInput;
    Transform transformToMove;
    Transform spriteObj;
    PlayerDash playerDash;
    PlayerTeleport playerTeleport;
    PlayerAnimations playerAnimations;
    BoomerangLauncher boomerangLauncher;

    private float gravity;

    private float maxJumpVelocity;
    private float minJumpVelocity;

    private float wallDirX;

    private Vector2 velocity;
    private float velocityXSmoothing;
    private float velocityYSmoothing;
    public bool WallSliding { get; private set; }
    public Vector2 Velocity { get => velocity; set => velocity = value; }
    public bool IsAttacking { get; set; }
    public bool IsInAttackAnimation { get; set; }
    public bool attackStop = true; 

    public float AttackSpeed = 2f;
    public Vector2 attackDir;

    [Header("Knockback settings")]
    public Vector3 dirKnockback;
    public bool isKnockedback_Damage;
    public bool isKnockedback_Hit;

    [HideInInspector] public float knockbackDistance;


    [Header("Teleport settings")]
    public float boostForceX = 5f;
    public float boostForceY = 5f;
    public float boostGravityMultiplier = 0.5f;

    [Header("Jump settings")]
    public float jumpForceX = 25f;
    public float jumpApexGravityModifier = 0.8f;
    public bool canJump;
    public bool canDoubleJump;
    float MAX_JUMP_ASSIST_TIME;
    float cayoteTimer = 0f;
    int MAX_JUMP_BUFFER_TIME;
    int jumpBufferCounter = 10;

    [Header("Run settings")]
    public float walkSpeed = 16f;
    public float sprintSpeed = 18f;
    public float dashSpeed = 16f;
    public bool isSprinting = false;

    public float runAccel = 1f;
    public float runDeccelTime = 0.1f;

    public float accelReducedGrounded = 0.3f;
    public float accelReducedAirborne = 0.1f;

    public float stopFriction = 0.05f;
    public float stopFrictionSprinting = 0.1f;

    public float speedThreshold = 8f;
    public float halfGravThreshold;

    public float maxSpeed = 30f;
    float maxFallSpeed = -30;

    [Header("LedgeDetection")]
    public Transform ledgeDetectionOrigin;
    public float ledgeDetectionDistance;


    [Header("Effects")]
    public ParticleSystem dustParticles;
    public ParticleSystem jumpLandParticles;
    public ParticleSystem jumpDustTrail;
    public GameObject jumpDustPostion;

    public GameObject jumpTrailParent;

    TMPro.TMP_Text velocityXDebug;
    TMPro.TMP_Text velocityYDebug;

    float spriteScaleXSmoothing;
    float spriteScaleYSmoothing;
    float spritePosXSmoothing;
    float spritePosYSmoothing;

    bool landed = false;
    float currentJumpHeight;
    float initialHeight;

    [Header("States")]
    public bool isAirborne;
    public bool isDead;
    public bool isPaused = false;

    [Header("Scene Loading Settings")]
    public AnimationCurve exitVelocityXCurve;
    public AnimationCurve exitVelocityYCurve;

    [HideInInspector] public float exitVelocityX;
    [HideInInspector] public float exitVelocityY;

    [HideInInspector] public bool isLoadingVertical;
    [HideInInspector] public bool isLoadingHorizontal;

    private void Start()
    {
        transformToMove = transform;
        playerInput = transformToMove.GetComponent<Player_Input>();
        controller = transformToMove.GetComponent<Controller_2D>();
        playerDash = transformToMove.GetComponent<PlayerDash>();
        playerTeleport = transformToMove.GetComponent<PlayerTeleport>();
        spriteObj = GetComponentInChildren<SpriteRenderer>().transform;
        boomerangLauncher = GetComponentInChildren<BoomerangLauncher>();

        playerInput.OnJumpDown += OnJumpInputDown;
        playerInput.OnJumpUp += OnJumpInputUp;
        playerInput.OnDash += OnDashInput;
        playerInput.OnDashUp += OnDashInputUp;

        playerInput.OnTeleport += OnBoomerangDashInput;

        gravity = -(2 * playerSettings.MaxJumpHeight) / Mathf.Pow(playerSettings.TimeToJumpApex, 2);

        maxJumpVelocity = Mathf.Abs(gravity) * playerSettings.TimeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * playerSettings.MinJumpHeight);

        MAX_JUMP_ASSIST_TIME = playerSettings.MaxJumpAssistanceTime;
        MAX_JUMP_BUFFER_TIME = playerSettings.MaxJumpBufferFrames;
        maxFallSpeed = playerSettings.MaxFallSpeed;

        playerAnimations = new PlayerAnimations(GetComponent<Animator>(), transform);

        velocityXDebug = UI_HUD.instance.velocityXDebug;
        velocityYDebug = UI_HUD.instance.velocityYDebug;
    }
    bool wasThouchingGround = false;

    public bool movePlayerLoadingState;
    float elapsedTime = 0f;
    float totalTime = 0.35f;

    private void FixedUpdate()
    {
        if (movePlayerLoadingState == true)
        {
            if (elapsedTime < totalTime)
            {
                Debug.Log("Scene Load Movement State. Time: " + elapsedTime);
                elapsedTime += Time.deltaTime;

                if (isLoadingHorizontal)
                {
                    float targetVelocityX = 15 * Mathf.Sign(exitVelocityX);
                    float smoothTime = (controller.collitions.below ? playerSettings.AccelerationTimeGrounded : playerSettings.AccelerationTimeAirborne);

                    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
                    velocity.y += gravity * Time.deltaTime;

                    HandleMaxSlope();
                }
                else if (isLoadingVertical)
                {
                    velocity.y += gravity * exitVelocityYCurve.Evaluate(elapsedTime / totalTime) * Time.deltaTime;
                    velocity.x = exitVelocityX * exitVelocityXCurve.Evaluate(elapsedTime / totalTime);
                }

                SpriteUpdate();
                controller.Move(velocity * Time.smoothDeltaTime, new Vector2(-1, -1));
                playerAnimations.Animate();
                SetPlayerOrientation(new Vector2(Mathf.Sign(velocity.x), 0));
                return;
            }
            else
            {
                elapsedTime = 0f;
                movePlayerLoadingState = false;
                GetComponent<Player_Input>().EnablePlayerInput();
            }
        }

        if (GameManager.instance.isLoading)
        {
            SpriteUpdate();
            controller.Move(velocity * Time.smoothDeltaTime, new Vector2(-1, -1));
            playerAnimations.Animate();
            SetPlayerOrientation(new Vector2(Mathf.Sign(velocity.x), 0));
            return;
        }

        SpriteUpdate();
        controller.Move(velocity * Time.smoothDeltaTime, new Vector2(-1, -1));
        HandleMaxSlope();

        playerAnimations.Animate();

        if (UI_HUD.instance.debugMode)
        {
            velocityXDebug.SetText("Velocity X: " + Mathf.Round(velocity.x));
            velocityYDebug.SetText("Velocity Y: " + Mathf.Round(velocity.y));
        }

        if (controller.collitions.below && !landed)
        {
            landed = true;
            //spriteObj.localScale = new Vector2(1.4f, .9f);
            //spriteObj.transform.position = new Vector2(1f, -0.32f);
        }
        else if (!controller.collitions.below)
        {
            landed = false;
        }

        if (landed && !wasThouchingGround)
        {
            jumpLandParticles.Play();
        }

        wasThouchingGround = controller.collitions.below;

        Movement();
    }

    void SpriteUpdate()
    {
        float spriteScaleX = Mathf.SmoothDamp(spriteObj.localScale.x, Mathf.Sign(spriteObj.localScale.x), ref spriteScaleXSmoothing, 0.2f);
        float spriteScaleY = Mathf.SmoothDamp(spriteObj.localScale.y, 1, ref spriteScaleYSmoothing, 0.2f);

        //float spritePosX = Mathf.SmoothDamp(spriteObj.transform.position.x, 0, ref spritePosXSmoothing, 0.2f);
        //float spritePosY = Mathf.SmoothDamp(spriteObj.transform.position.y, 0, ref spritePosYSmoothing, 0.2f);

        spriteObj.localScale = new Vector2(spriteScaleX, spriteScaleY);
        //spriteObj.transform.position = new Vector2(spritePosX, spritePosY);
    }

    /// <summary>
    /// Main movement method, responsible for calculating and executing player movement as well as jumping, 
    /// wall sliding, slope, orientation and dash logic gets called on every frame (fixed update)
    /// </summary>
    public void Movement()
    {
        if (isKnockedback_Damage)
        {
            Knockback(dirKnockback, knockbackDistance);
            HandleWallSliding();
            return;
        }

        if (isKnockedback_Hit)
        {
            Knockback(dirKnockback, knockbackDistance);
            HandleWallSliding();
            return;
        }

        if (isDead || GameManager.instance.isPaused || DialogManager.instance.dialogueIsActive)
        {
            velocity.x = 0;
            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Clamp(velocity.y, maxFallSpeed, 1000);
            return;
        }

        if (playerTeleport.doBoost)
        {
            SetPlayerOrientation(playerInput.directionalInput);
            BoomerandBoost();
            CalculateVelocity();
            HandleWallSliding();
            return;
        }

        if (playerDash.isDashing)
        {
            SetPlayerOrientation(playerInput.directionalInput);
            CalculateVelocity();

            HandleDash();
            HandleWallSliding();
            return;
        }

        SetPlayerOrientation(playerInput.directionalInput);
        CalculateVelocity();
        HandleDash();
        HandleJumpInput();
        HandleWallSliding();
    }

    bool hasDetectedLedge = false;

    public void DetectLedges()
    {
        if (controller.collitions.below)
        {
            RaycastHit2D hit = Physics2D.Raycast(ledgeDetectionOrigin.position, Vector2.down, ledgeDetectionDistance, controller.collitionMask);
            Debug.DrawRay(ledgeDetectionOrigin.position, Vector2.down * ledgeDetectionDistance, Color.red);
            hasDetectedLedge = !hit;
        }
    }

    /// <summary>
    /// Method for calculating knockback 
    /// </summary>
    /// <param name="dir">Direction of knockback force</param>
    /// <param name="kockbackDistance">Knockback force amount</param>
    public void Knockback(Vector3 dir, float kockbackDistance)
    {
        //Debug.Log("Knockback: Direction: " + dir.ToString() + " velocity: " + knockbackDistance);

        if (dir.x != 0)
            velocity.x = dir.x * kockbackDistance * 1.5f;

        velocity.y = dir.y * kockbackDistance;
    }

    public void BoomerandBoost()
    {
        velocity = Vector3.zero;
        velocity.x += playerTeleport.boostDir.x * boostForceX;
        velocity.y += playerTeleport.boostDir.y * boostForceY;
    }

    void OnDashInput()
    {
        isAirborne = (!controller.collitions.below && !WallSliding);

        if (!isAirborne &&  GameManager.instance.playerData.hasDashAbility)
            playerDash.OnDashInput();

        if (isAirborne && GameManager.instance.playerData.hasAirDashAbility)
            playerDash.OnDashInput();

        if (GameManager.instance.playerData.hasSprintAbility && controller.collitions.below)
        {
            isSprinting = true;
        }
    }

    void OnDashInputUp()
    {
        isSprinting = false;
    }

    void HandleDash()
    {
        isAirborne = (!controller.collitions.below && !WallSliding);

        if ((!controller.collitions.left || !controller.collitions.right)){

            if (!IsAttacking)
            {
                if (isAirborne && GameManager.instance.playerData.hasAirDashAbility)
                {
                    playerDash.DashController(ref velocity, playerInput, playerSettings);
                }
                else if (!isAirborne)
                {
                    playerDash.DashController(ref velocity, playerInput, playerSettings);
                }
            }
        }

        if(playerDash.isDashing && IsAttacking)
        {
            playerDash.StopDash();
        }
    }

    void OnBoomerangDashInput()
    {
        Boomerang boomerang = boomerangLauncher.boomerangReference;

        if (GameManager.instance.playerData.hasTeleportAbility && boomerang != null)
            playerTeleport.OnTeleportInput(transformToMove, ref velocity, boomerang);
    }

    /// <summary>
    /// Method that calculates the players' velocity based on the players' speed and input 
    /// </summary>
    /// <param name="velocityXSmoothing"></param>
    public void CalculateVelocity()
    {
        float moveSpeed = isSprinting ? sprintSpeed : walkSpeed;
        moveSpeed = playerDash.isDashing ? dashSpeed : moveSpeed;

        float targetVelocityX = moveSpeed * playerInput.directionalInput.x;
        float smoothTime = (controller.collitions.below ? playerSettings.AccelerationTimeGrounded : playerSettings.AccelerationTimeAirborne);
        float smoothTimeReduced = (controller.collitions.below ? accelReducedGrounded : accelReducedAirborne);

        currentJumpHeight = transform.position.y - initialHeight;
        float mult = (currentJumpHeight >= playerSettings.MaxJumpHeight) ? jumpApexGravityModifier : 1f;
        mult = (playerTeleport.doBoost) ? boostGravityMultiplier : mult;

        DetectLedges();

        //Calculating X velocity
        if (controller.collitions.below)
        {
            if (IsAttacking && attackStop && !isKnockedback_Damage && !isKnockedback_Hit && !hasDetectedLedge)
            {
                if (attackDir.y >= 0)
                {
                    velocity.x = Mathf.SmoothDamp(velocity.x, AttackSpeed * transform.localScale.x, ref velocityXSmoothing, 0);
                }
            }
            else if (isSprinting && Mathf.Abs(velocity.x) > speedThreshold && Mathf.Sign(velocity.x) == playerInput.directionalInput.x * -1)
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref velocityXSmoothing, stopFrictionSprinting);
            }
            else if (Mathf.Abs(velocity.x) > speedThreshold && Mathf.Sign(velocity.x) == playerInput.directionalInput.x * -1)
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref velocityXSmoothing, stopFriction);
            }
            else if (playerInput.directionalInput.x == 0 && !isSprinting)
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref velocityXSmoothing, runDeccelTime);
            }
            else
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
            }

        }
        else if (playerDash.isDashing)
        {
            targetVelocityX = dashSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, playerSettings.DashSpeedTime);
        }
        else
        {
            if (Mathf.Abs(velocity.x) > Mathf.Abs(targetVelocityX) && Mathf.Sign(velocity.x) == playerInput.directionalInput.x && !playerDash.isDashing)
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTimeReduced);
            }
            else if (Mathf.Abs(velocity.x) > speedThreshold && Mathf.Sign(velocity.x) == playerInput.directionalInput.x*-1 && !playerDash.isDashing)
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref velocityXSmoothing, stopFriction);
            }
            else
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
            }
        }

        //Calculating Y velocity
        if (playerDash.isDashing)
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y += gravity * mult * Time.deltaTime;
            velocity.y = Mathf.Clamp(velocity.y, maxFallSpeed, 1000);
        }
    }

    /// <summary>
    /// Method for handling player movement on a slope, the player slides down if he is standing on a max slope  
    /// </summary>
    void HandleMaxSlope()
    {
        //Debug.Log(controller.collitions.below);
        if (controller.collitions.above || controller.collitions.below)
        {
            if (controller.collitions.slidingDownMaxSlope)
            {
                velocity.y += controller.collitions.slopeNormal.y * gravity * -1 * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }

    /// <summary>
    /// Method that handles wall sliding logic
    /// </summary>
    /// <param name="velocityXSmoothing"></param>    
    void HandleWallSliding()
    {
        if (!GameManager.instance.playerData.hasWallJumpAbility)
            return;

        wallDirX = (controller.collitions.left) ? -1 : 1;
        WallSliding = false;
        if ((controller.collitions.left || controller.collitions.right) && !controller.collitions.below && velocity.y < 0)
        {
            WallSliding = true;
            canDoubleJump = GameManager.instance.playerData.hasDoubleJumpAbility;

            if (velocity.y < playerSettings.WallSlideSpeedMax)
            {
                velocity.y = -playerSettings.WallSlideSpeedMax;
            }
            if (playerSettings.TimeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (playerInput.directionalInput.x != wallDirX && playerInput.directionalInput.x != 0)
                {
                    playerSettings.TimeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    playerSettings.TimeToWallUnstick = playerSettings.WallStickTime;
                }
            }
            else
            {
                playerSettings.TimeToWallUnstick = playerSettings.WallStickTime;
            }
        }
    }

    /// <summary>
    /// Method that handles jump logic and amount when the jump button is pressed down
    /// </summary>
    public void OnJumpInputDown()
    {
        jumpBufferCounter = 0;
        initialHeight = transform.position.y;

        if (!canJump && canDoubleJump && !WallSliding)
        {
            canDoubleJump = false;
            jumpLandParticles.Play();
            cayoteTimer = MAX_JUMP_ASSIST_TIME;
            Jump(maxJumpVelocity * 0.75f, jumpForceX * 0.75f);
        }
    }

    /// <summary>
    /// Method that handles jump logic when the player lets go of the jump button, allows the player to control the jump amount 
    /// </summary>
    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    void HandleJumpInput()
    {
        if (!controller.collitions.below || playerDash.isDashing)
        {
            cayoteTimer += Time.deltaTime;
        }
        else
        {
            canDoubleJump = GameManager.instance.playerData.hasDoubleJumpAbility;
            cayoteTimer = 0;
        }
        canJump = cayoteTimer < MAX_JUMP_ASSIST_TIME;

        if (playerDash.isDashing)
        {
            cayoteTimer = 100;
            canJump = false;
        }

        if(velocity.y <= 0)
        {
            foreach (TrailRenderer jumpTrail in jumpTrailParent.GetComponentsInChildren<TrailRenderer>())
            {
                jumpTrail.emitting = false;
            }
        }
        else
        {
            foreach (TrailRenderer jumpTrail in jumpTrailParent.GetComponentsInChildren<TrailRenderer>())
            {
                jumpTrail.emitting = true;
            }
        }

        if (jumpBufferCounter < MAX_JUMP_BUFFER_TIME)
        {
            
            if (canJump)
            {
                jumpLandParticles.Play();
                StartCoroutine(JumpTrail());
            }

            jumpBufferCounter += 1;
            if (WallSliding && GameManager.instance.playerData.hasWallJumpAbility)
            {
                if (wallDirX == playerInput.directionalInput.x)
                {
                    velocity.x = -wallDirX * playerSettings.WallJumpclimb.x;
                    velocity.y = playerSettings.WallJumpclimb.y;
                }
                else
                {
                    velocity.x = -wallDirX * playerSettings.WallLeap.x;
                    velocity.y = playerSettings.WallLeap.y;
                }
            }

            if (canJump)
            {
                cayoteTimer = MAX_JUMP_ASSIST_TIME;
                Jump(maxJumpVelocity, jumpForceX);
            }

          
        }
    }

    void Jump(float jumpVelocityY, float jumpVelocityX)
    {
        if (controller.collitions.slidingDownMaxSlope)
        {
            if (playerInput.directionalInput.x != -Mathf.Sign(controller.collitions.slopeNormal.x))
            {
                velocity.y = jumpVelocityY * controller.collitions.slopeNormal.y;
                velocity.x = jumpVelocityX * controller.collitions.slopeNormal.x;
            }
        }
        else
        {
            velocity.y = jumpVelocityY;

            if ((!controller.collitions.left || !controller.collitions.right))
            {
                velocity.x += jumpVelocityX * playerInput.directionalInput.x;
            }
            spriteObj.localScale = new Vector2(.7f, 1.3f);
        }
    }

    float jumpTrailTimeCurrent;
    public float jumpTrailTime = 0.1f;
    IEnumerator JumpTrail()
    {
        jumpTrailTimeCurrent = jumpTrailTime;

        while(jumpTrailTimeCurrent > 0f)
        {
            jumpTrailTimeCurrent -= Time.deltaTime;
            jumpDustTrail.Play();
            yield return null;
        }
    }

    public void OnAttackStart()
    {
        IsAttacking = true;
    }

    public void OnAttackEnd()
    {
        IsAttacking = false;
    }

    public void OnAttackAnimationStart()
    {
        IsInAttackAnimation = true;
    }

    public void OnAttackAnimationEnd()
    {
        IsInAttackAnimation = false;
    }

    int facingDirection;
    /// <summary>
    /// Method for setting the players' orientation based on input
    /// </summary>
    /// <param name="input">Player input</param>
    public void SetPlayerOrientation(Vector2 input)
    {
        if (!playerDash.isDashing && !WallSliding && !IsInAttackAnimation)
        {
            if (input.x < 0)
            {
                transformToMove.localScale = new Vector3(Mathf.Abs(transformToMove.localScale.x)*-1, transformToMove.localScale.y, -1);
            }
            else if (input.x > 0)
            {
                transformToMove.localScale = new Vector3(Mathf.Abs(transformToMove.localScale.x), transformToMove.localScale.y, -1);
            }
        }
    }

    public void Flip()
    {
        facingDirection *= -1;
        transformToMove.localScale = new Vector3(Mathf.Abs(transformToMove.localScale.x) * facingDirection, transformToMove.localScale.y, -1);
    }

    public void FlipOnAttack()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < transform.position.x)
        {
            Flip();
        }
    }

    ///// <summary>
    ///// Method that pulls the player towards the hook point
    ///// </summary>
    //public void Swing()
    //{
    //    var playerToHookDirection = (RopeHook - (Vector2)transformToMove.position).normalized;
    //    var pullforce = playerToHookDirection * playerSettings.SwingForce;
    //    AddForce(pullforce);
    //}

    ///// <summary>
    ///// Helper method for changing the players'velocity based on a force
    ///// </summary>
    ///// <param name="force">Force acting opon player</param>
    //public void AddForce(Vector2 force)
    //{
    //    velocity.x = Mathf.SmoothDamp(velocity.x, force.x, ref velocityXSmoothing, playerSettings.AccelerationTimeSwing);
    //    velocity.y = Mathf.SmoothDamp(velocity.y, force.y, ref velocityYSmoothing, playerSettings.AccelerationTimeSwing);
    //}

}
