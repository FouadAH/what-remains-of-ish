using System.Collections;
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
    PlayerTeleport boomerangDash;
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

    [Header("Knockback settings")]
    public float kockbackDistance;
    public Vector3 dirKnockback;
    public bool isKnockedback;

    [Header("Teleport settings")]
    public float boostForceX = 5f;
    public float boostForceY = 5f;
    public float boostGravityMultiplier = 0.5f;

    [Header("Jump settings")]
    public float jumpForceX = 25f;
    public float jumpApexGravityModifier = 0.8f;
    public bool canJump;
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

    [Header("Effects")]
    public ParticleSystem dustParticles;
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

    private void Start()
    {
        transformToMove = transform;
        playerInput = transformToMove.GetComponent<Player_Input>();
        controller = transformToMove.GetComponent<Controller_2D>();
        playerDash = transformToMove.GetComponent<PlayerDash>();
        boomerangDash = transformToMove.GetComponent<PlayerTeleport>();
        spriteObj = GetComponentInChildren<SpriteRenderer>().transform;
        boomerangLauncher = GetComponentInChildren<BoomerangLauncher>();

        playerInput.OnJumpDown += OnJumpInputDown;
        playerInput.OnJumpUp += OnJumpInputUp;
        playerInput.OnDash += OnDashInput;
        playerInput.OnDashUp += OnDashInputUp;

        playerInput.OnBoomerangDash += OnBoomerangDashInput;

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

    private void FixedUpdate()
    {
        if (GameManager.instance.loading)
            return;

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
        if (isKnockedback)
        {
            Knockback(dirKnockback, kockbackDistance);
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

        if (boomerangDash.doBoost)
        {
            BoomerandBoost();
            CalculateVelocity(true);
            HandleWallSliding();
            return;
        }

        SetPlayerOrientation(playerInput.directionalInput);
        CalculateVelocity();

        HandleDash();
        HandleJumpInput();
        HandleWallSliding();
    }

    /// <summary>
    /// Method for calculating knockback 
    /// </summary>
    /// <param name="dir">Direction of knockback force</param>
    /// <param name="kockbackDistance">Knockback force amount</param>
    public void Knockback(Vector3 dir, float kockbackDistance)
    {
        velocity = Vector3.zero;
        velocity.x += dir.x * kockbackDistance * 1.5f;
        velocity.y += dir.y * kockbackDistance;
    }

    public void BoomerandBoost()
    {
        velocity = Vector3.zero;
        velocity.x += boomerangDash.boostDir.x * boostForceX;
        velocity.y += boomerangDash.boostDir.y * boostForceY;
    }

    void OnDashInput()
    {
        if(GameManager.instance.hasDashAbility)
            playerDash.OnDashInput();

        if (GameManager.instance.hasSprintAbility && controller.collitions.below)
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
            playerDash.DashController(ref velocity, playerInput, playerSettings);
        }
    }

    void OnBoomerangDashInput()
    {
        Boomerang boomerang = boomerangLauncher.boomerangReference;

        if (GameManager.instance.hasTeleportAbility && boomerang != null)
            boomerangDash.OnTeleportInput(transformToMove, ref velocity, boomerang);
    }

    /// <summary>
    /// Method that calculates the players' velocity based on the players' speed and input 
    /// </summary>
    /// <param name="velocityXSmoothing"></param>
    public void CalculateVelocity(bool isTeleporting = false)
    {
        float moveSpeed = isSprinting ? sprintSpeed : walkSpeed;
        moveSpeed = playerDash.isDashing ? dashSpeed : moveSpeed;

        float targetVelocityX = moveSpeed * playerInput.directionalInput.x;
        float smoothTime = (controller.collitions.below ? playerSettings.AccelerationTimeGrounded : playerSettings.AccelerationTimeAirborne);
        float smoothTimeReduced = (controller.collitions.below ? accelReducedGrounded : accelReducedAirborne);

        currentJumpHeight = transform.position.y - initialHeight;
        float mult = (currentJumpHeight >= playerSettings.MaxJumpHeight) ? jumpApexGravityModifier : 1f;
        mult = (isTeleporting) ? boostGravityMultiplier : mult;

        //Debug.Log("Velocity X: " + velocity.x);

        if (controller.collitions.below)
        {
            if (isSprinting && Mathf.Abs(velocity.x) > speedThreshold && Mathf.Sign(velocity.x) == playerInput.directionalInput.x * -1)
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
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, 0.2f);
        }
        else
        {
            if (Mathf.Abs(velocity.x) > Mathf.Abs(targetVelocityX) && Mathf.Sign(velocity.x) == playerInput.directionalInput.x && !playerDash.isDashing)
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTimeReduced);
            }
            else
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
            }
        }

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
        if (!GameManager.instance.hasWallJump)
            return;

        wallDirX = (controller.collitions.left) ? -1 : 1;
        WallSliding = false;
        if ((controller.collitions.left || controller.collitions.right) && !controller.collitions.below && velocity.y < 0)
        {
            WallSliding = true;

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

        if (!controller.collitions.below)
        {
            cayoteTimer += Time.deltaTime;
        }
        else
        {
            cayoteTimer = 0;
        }
        canJump = cayoteTimer < MAX_JUMP_ASSIST_TIME;

        if (jumpBufferCounter < MAX_JUMP_BUFFER_TIME)
        {
            jumpBufferCounter += 1;
            if (WallSliding && GameManager.instance.hasWallJump)
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

                if (controller.collitions.slidingDownMaxSlope)
                {
                    if (playerInput.directionalInput.x != -Mathf.Sign(controller.collitions.slopeNormal.x))
                    {
                        velocity.y = maxJumpVelocity * controller.collitions.slopeNormal.y;
                        velocity.x = maxJumpVelocity * controller.collitions.slopeNormal.x;
                    }
                }
                else
                {
                    velocity.y = maxJumpVelocity;

                    if ((!controller.collitions.left || !controller.collitions.right))
                    {
                        velocity.x += jumpForceX * playerInput.directionalInput.x;
                    }
                    spriteObj.localScale = new Vector2(.7f, 1.3f);
                }
            }
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

    /// <summary>
    /// Method for setting the players' orientation based on input
    /// </summary>
    /// <param name="input">Player input</param>
    public void SetPlayerOrientation(Vector2 input)
    {
        if (!playerDash.isDashing && !WallSliding && !IsAttacking)
        {
            playerInput.directionalInput = input;
            if (playerInput.directionalInput.x < 0)
            {
                transformToMove.localScale = new Vector3(Mathf.Abs(transformToMove.localScale.x)*-1, transformToMove.localScale.y, -1);
            }
            else if (playerInput.directionalInput.x > 0)
            {
                transformToMove.localScale = new Vector3(Mathf.Abs(transformToMove.localScale.x), transformToMove.localScale.y, -1);
            }
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
