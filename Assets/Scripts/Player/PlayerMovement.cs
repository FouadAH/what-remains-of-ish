using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class responsible for handling player movement calculations</summary>
public class PlayerMovement : MonoBehaviour
{
    public PlayerMovementSettings playerSettings;
    public PlayerDataSO playerData;
    public PlayerRuntimeDataSO playerRuntimeData;

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

    [Range(0f, 1f)]
    public float jumpAttackGravityModifier = 0.5f;

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

    [Header("Down Attack Settings")]

    public float downAttackDuration = 0.35f;

    public float downAttackForceX = 25;
    public float downAttackForceY = -35;

    public float downAttackSmoothingY = 0.15f;
    public float downAttackSmoothingX = 0.15f;

    [Range(1, 2)]
    public float downAttackDirectionInfluenceMax = 1.5f;

    [Range(0, 1)]
    public float downAttackDirectionInfluenceMin = 0f;

    [Range(0, 1)]
    public float downAttackDirectionInfluenceAcceleration = 0.05f;

    float downAttackDirection = 0;
    float downAttackCurrentTime;
    float directionInfluence = 1;

    bool canDownAttack = true;

    [Header("LedgeDetection")]
    public Transform ledgeDetectionOrigin;
    public float ledgeDetectionDistance;

    [Header("Effects")]
    public ParticleSystem dustParticles;
    public ParticleSystem jumpLandParticles;
    public ParticleSystem jumpDustTrail;
    public ParticleSystem[] downAttackEffects;

    public GameObject jumpDustPostion;

    public GameObject jumpTrailParent;

    float spriteScaleXSmoothing;
    float spriteScaleYSmoothing;
    float spritePosXSmoothing;
    float spritePosYSmoothing;

    bool landed;
    float currentJumpHeight;
    float initialHeight;

    [Header("States")]
    public bool isAirborne;
    public bool isDead;
    public bool isPaused;
    public bool inDownAttack;

    [Header("Scene Loading Settings")]
    public AnimationCurve exitVelocityXCurve;
    public AnimationCurve exitVelocityYCurve;

    [HideInInspector] public float exitVelocityX;
    [HideInInspector] public float exitVelocityY;

    [HideInInspector] public bool isLoadingVertical;
    [HideInInspector] public bool isLoadingHorizontal;

    PlayerMovementState movementState;
    Player player;

    private void Start()
    {
        transformToMove = transform;
        player = transformToMove.GetComponent<Player>();
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

        playerInput.OnDownAttack += PlayerInput_OnDownAttack;

        gravity = -(2 * playerSettings.MaxJumpHeight) / Mathf.Pow(playerSettings.TimeToJumpApex, 2);

        maxJumpVelocity = Mathf.Abs(gravity) * playerSettings.TimeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * playerSettings.MinJumpHeight);

        MAX_JUMP_ASSIST_TIME = playerSettings.MaxJumpAssistanceTime;
        MAX_JUMP_BUFFER_TIME = playerSettings.MaxJumpBufferFrames;
        maxFallSpeed = playerSettings.MaxFallSpeed;

        playerAnimations = new PlayerAnimations(GetComponent<Animator>(), transform);
    }

    private void OnEnable()
    {
        IsAttacking = false;
        isKnockedback_Damage = false;
        isKnockedback_Hit = false;
        isSprinting = false;
    }

    private void PlayerInput_OnDownAttack()
    {
        if (!canDownAttack)
            return;

        if (isAirborne && !IsAttacking)
        {
            if (canDownAttack)
            {
                if (playerInput.directionalInput.x != 0)
                    downAttackDirection = playerInput.directionalInput.x;
                else
                    downAttackDirection = transformToMove.localScale.x;

                directionInfluence = 1;
                StartCoroutine(DownAttackLock());
                ExecuteDownAttack();
                StartCoroutine(DownAttackTimer());
            }
        }
    }

    void ExecuteDownAttack()
    {
        inDownAttack = true;
        playerAnimations.DownAttack();
        foreach (ParticleSystem ps in downAttackEffects)
        {
            ps.Play();
        }
    }

    void CancelDownAttack()
    {
        //Debug.Log("DOWN ATTACK CANCEL");

        //if (controller.collitions.below || controller.collitions.right || controller.collitions.left)
        //{
        //    isKnockedback_Hit = false;
        //}

        StartCoroutine(player.DamageIFrames(0.15f));
        StartCoroutine(CancelDownAttackRoutine());

        if (isKnockedback_Hit)
        {
            StopCoroutine(DownAttackLock());
            canDownAttack = true;
        }

        IsAttacking = false;
        inDownAttack = false;
        velocity.y = 0;

        foreach (ParticleSystem ps in downAttackEffects)
        {
            ps.Stop();
        }
    }

    IEnumerator CancelDownAttackRoutine()
    {
        yield return new WaitForFixedUpdate();
        playerAnimations.CancelDownAttack();
    }
    IEnumerator DownAttackLock()
    {
        canDownAttack = false;
        yield return new WaitWhile(() => isAirborne || IsAttacking);
        canDownAttack = true;
    }

    IEnumerator DownAttackTimer()
    {
        while (downAttackCurrentTime < downAttackDuration && inDownAttack)
        {
            downAttackCurrentTime += Time.deltaTime;
            yield return null;
        }

        downAttackCurrentTime = 0;
        inDownAttack = false;
        IsAttacking = false;
        playerAnimations.CancelDownAttack();

        foreach (ParticleSystem ps in downAttackEffects)
        {
            ps.Stop();
        }
        yield return null;
    }

    void HandleDownAttack()
    {
        //Calculating down attack direction influence. Total velocity must remain the same
        float directionInfluenceY = 1;
        if (playerInput.directionalInput.x != 0)
        {
            if (Mathf.Sign(playerInput.directionalInput.x) != Mathf.Sign(downAttackDirection))
            {
                directionInfluence = Mathf.Lerp(directionInfluence, downAttackDirectionInfluenceMin, downAttackDirectionInfluenceAcceleration);
                directionInfluenceY = 1  + Mathf.Abs(1 - Mathf.Abs(directionInfluence));
            }
            else
            {
                directionInfluence = Mathf.Lerp(directionInfluence, downAttackDirectionInfluenceMax, downAttackDirectionInfluenceAcceleration);
                directionInfluenceY = 1 - Mathf.Abs(1 - Mathf.Abs(directionInfluence));
            }
        }
        else
        {
            directionInfluence = Mathf.Lerp(directionInfluence, 1, downAttackDirectionInfluenceAcceleration);
            directionInfluenceY = 1;
        }

        float targetVelocityX = downAttackForceX * (MathF.Sign(downAttackDirection) * directionInfluence);
        float targetVelocityY = downAttackForceY * directionInfluenceY;

        if (controller.collitions.below || isKnockedback_Hit || isKnockedback_Damage)
        {
            CancelDownAttack();
            return;
        }

        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, downAttackSmoothingX);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, downAttackSmoothingY);
    }

    bool wasThouchingGround = false;

    public bool movePlayerLoadingState;
    float elapsedTime = 0f;
    float totalTime = 0.35f;

    private void FixedUpdate()
    {
        playerRuntimeData.playerPosition = transform.position;

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

        playerRuntimeData.velocity = velocity;

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

        if(controller.collitions.below)
        {
            playerRuntimeData.lastPlayerGroundedPosition = transform.position;
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
        spriteObj.localScale = new Vector2(spriteScaleX, spriteScaleY);
    }

    /// <summary>
    /// Main movement method, responsible for calculating and executing player movement as well as jumping, 
    /// wall sliding, slope, orientation and dash logic gets called on every frame (fixed update)
    /// </summary>
    public void Movement()
    {
        if (isDead || GameManager.instance.isPaused || DialogManager.instance.dialogueIsActive)
        {
            movementState = PlayerMovementState.Dead;

            velocity.x = 0;
            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Clamp(velocity.y, maxFallSpeed, 1000);

            IsAttacking = false;
            isKnockedback_Damage = false;
            isKnockedback_Hit = false;
            isSprinting = false;
            IsInAttackAnimation = false;
            inDownAttack = false;
            return;
        }

        if (inDownAttack)
        {
            HandleDownAttack();
            HandleWallSliding();

            movementState = PlayerMovementState.DownAttack;
            return;
        }

        if (isKnockedback_Damage)
        {
            Knockback(dirKnockback, knockbackDistance);
            HandleWallSliding();

            movementState = PlayerMovementState.KnockbackDamage;
            return;
        }

        if (isKnockedback_Hit)
        {
            Knockback(dirKnockback, knockbackDistance);
            HandleWallSliding();

            movementState = PlayerMovementState.KnockbackHit;
            return;
        }

        if (playerTeleport.doBoost)
        {
            movementState = PlayerMovementState.Teleport;

            SetPlayerOrientation(playerInput.directionalInput);
            BoomerandBoost();
            CalculateVelocity();
            HandleWallSliding();
            return;
        }

        if (playerDash.isDashing)
        {
            movementState = PlayerMovementState.Dash;

            SetPlayerOrientation(playerInput.directionalInput);
            CalculateVelocity();

            HandleDash();
            HandleWallSliding();
            return;
        }

        movementState = PlayerMovementState.Move;

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

        if (!isAirborne &&  playerData.hasDashAbility)
            playerDash.OnDashInput();

        if (isAirborne && playerData.hasAirDashAbility)
            playerDash.OnDashInput();

        if (playerData.hasSprintAbility && controller.collitions.below)
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
                if (isAirborne && playerData.hasAirDashAbility)
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

        if (playerData.hasTeleportAbility && boomerang != null)
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
        float gravityMult = (currentJumpHeight >= playerSettings.MaxJumpHeight) ? jumpApexGravityModifier : 1f;
        gravityMult = (playerTeleport.doBoost) ? boostGravityMultiplier : gravityMult;

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

            if (IsAttacking && attackStop && !isKnockedback_Damage && !isKnockedback_Hit)
            {
                if (velocity.y < 0) 
                    gravityMult = jumpAttackGravityModifier;
            }
        }

        //Calculating Y velocity
        if (playerDash.isDashing)
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y += gravity * gravityMult * Time.deltaTime;
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
        if (!playerData.hasWallJumpAbility)
            return;

        wallDirX = (controller.collitions.left) ? -1 : 1;
        WallSliding = false;
        if ((controller.collitions.left || controller.collitions.right) && !controller.collitions.below && velocity.y < 0)
        {
            WallSliding = true;
            canDoubleJump = playerData.hasDoubleJumpAbility;

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
        isPressingJump = true;

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

    bool isPressingJump = false;
    /// <summary>
    /// Method that handles jump logic when the player lets go of the jump button, allows the player to control the jump amount 
    /// </summary>
    public void OnJumpInputUp()
    {
        isPressingJump = false;

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
            canDoubleJump = playerData.hasDoubleJumpAbility;
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
            if (WallSliding && playerData.hasWallJumpAbility)
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

        if (!isPressingJump)
        {
            velocity.y = minJumpVelocity;
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

    private void OnDestroy()
    {
        playerAnimations.UnsubscribeAnimationsFromInput();

        playerInput.OnJumpDown -= OnJumpInputDown;
        playerInput.OnJumpUp -= OnJumpInputUp;
        playerInput.OnDash -= OnDashInput;
        playerInput.OnDashUp -= OnDashInputUp;

        playerInput.OnTeleport -= OnBoomerangDashInput;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Collider2D collider = GetComponent<Collider2D>();
        Vector3 colliderCenter = collider.bounds.center;
        Vector3 colliderExtents = collider.bounds.extents;

        Vector3 startPos = new Vector3(colliderCenter.x, colliderCenter.y - colliderExtents.y, colliderCenter.z);
        Gizmos.DrawWireSphere(startPos, playerSettings.MaxJumpHeight);

        Gizmos.color = Color.red;
        startPos = new Vector3(colliderCenter.x, colliderCenter.y + colliderExtents.y, colliderCenter.z);
        Gizmos.DrawLine(startPos, new Vector3(startPos.x, startPos.y + playerSettings.MaxJumpHeight, startPos.z));

        Gizmos.color = Color.yellow;

        startPos = new Vector3(colliderCenter.x + colliderExtents.x, colliderCenter.y - colliderExtents.y, colliderCenter.z);
        Gizmos.DrawLine(startPos, new Vector3(startPos.x + playerSettings.MoveSpeed, startPos.y, startPos.z));

        startPos = new Vector3(colliderCenter.x - colliderExtents.x, colliderCenter.y - colliderExtents.y, colliderCenter.z);
        Gizmos.DrawLine(startPos, new Vector3(startPos.x - playerSettings.MoveSpeed, startPos.y, startPos.z));
    }
}

public enum PlayerMovementState
{
    Idle,
    Move,
    Jump,
    Dash,
    KnockbackDamage,
    KnockbackHit,
    Teleport,
    DownAttack,
    Dead,
    Paused
}
