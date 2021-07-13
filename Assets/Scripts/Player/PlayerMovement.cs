using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Class responsible for handling player movement calculations</summary>
public class PlayerMovement : MonoBehaviour
{
    public Player_Input playerInput;
    private Transform transformToMove;
    public PlayerMovementSettings playerSettings;
    private Controller_2D controller;
    private PlayerDash playerDash;
    private BoomerangDash boomerangDash;

    BoomerangLauncher boomerangLauncher;

    private float gravity;

    private float maxJumpVelocity;
    private float minJumpVelocity;

    private float wallDirX;

    private Vector2 velocity;
    private float velocityXSmoothing;
    private float velocityYSmoothing;
    public bool isDead;

    public bool WallSliding { get; private set; }
    public Vector2 Velocity { get => velocity; set => velocity = value; }

    public bool IsAttacking { get; set; }
    public bool IsSwinging { get; set; }
    public Vector2 RopeHook { get; set; }

    float MAX_JUMP_ASSIST_TIME;
    bool canJump;
    float cayoteTimer = 0f;

    int MAX_JUMP_BUFFER_TIME;
    int jumpBufferCounter = 10;

    float maxFallSpeed = -30;

    public bool isPaused = false;

    [Header("Effects")]
    public ParticleSystem dustParticles;

    private void Start()
    {
        transformToMove = transform;
        playerInput = transformToMove.GetComponent<Player_Input>();
        controller = transformToMove.GetComponent<Controller_2D>();
        playerDash = transformToMove.GetComponent<PlayerDash>();
        boomerangDash = transformToMove.GetComponent<BoomerangDash>();

        boomerangLauncher = GetComponentInChildren<BoomerangLauncher>();

        playerInput.OnJumpDown += OnJumpInputDown;
        playerInput.OnJumpUp += OnJumpInputUp;
        playerInput.OnDash += OnDashInput;
        playerInput.OnBoomerangDash += OnBoomerangDashInput;

        gravity = -(2 * playerSettings.MaxJumpHeight) / Mathf.Pow(playerSettings.TimeToJumpApex, 2);

        maxJumpVelocity = Mathf.Abs(gravity) * playerSettings.TimeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * playerSettings.MinJumpHeight);

        MAX_JUMP_ASSIST_TIME = playerSettings.MaxJumpAssistanceTime;
        MAX_JUMP_BUFFER_TIME = playerSettings.MaxJumpBufferFrames;
        maxFallSpeed = playerSettings.MaxFallSpeed;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.loading)
            return;

        Movement();
    }

    /// <summary>
    /// Main movement method, responsible for calculating 
    /// and executing player movement as well as jumping, 
    /// wall sliding, slope, orientation and dash logic
    /// gets called on every frame (fixed update)
    /// </summary>
    public void Movement()
    {
        if (isDead || isPaused)
        {
            velocity.x = 0;
            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Clamp(velocity.y, maxFallSpeed, 1000);

            controller.Move(velocity * Time.deltaTime, new Vector2(-1, playerInput.directionalInput.y));
            return;
        }

        if (isKnockedback)
        {
            Knockback(dirKnockback, kockbackDistance);
            HandleWallSliding(velocityXSmoothing);

            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Clamp(velocity.y, maxFallSpeed, 1000);

            controller.Move(velocity * Time.deltaTime, new Vector2(-1, playerInput.directionalInput.y));
            return;
        }

        if (boomerangDash.doBoost)
        {
            BoomerandBoost();
            HandleWallSliding(velocityXSmoothing);
            controller.Move(velocity * Time.deltaTime, new Vector2(-1, playerInput.directionalInput.y));
            return;
        }

        SetPlayerOrientation(playerInput.directionalInput);
        CalculateVelocity(velocityXSmoothing);
        
        HandleWallSliding(velocityXSmoothing);
        HandleDash();

        controller.Move(velocity * Time.deltaTime, new Vector2(-1, playerInput.directionalInput.y));

        HandleMaxSlope();
        HandleJumpInput();
    }

    public float kockbackDistance;
    public Vector3 dirKnockback;
    public bool isKnockedback;

    /// <summary>
    /// Method for calculating knockback 
    /// </summary>
    /// <param name="dir">Direction of knockback force</param>
    /// <param name="kockbackDistance">Knockback force amount</param>
    public void Knockback(Vector3 dir, float kockbackDistance)
    {
        velocity = Vector3.zero;
        velocity.x += dir.x * kockbackDistance;
        velocity.y += dir.y * kockbackDistance;
        //controller.Move(velocity * Time.deltaTime, new Vector2(-1, playerInput.directionalInput.y));
    }

    public float boostForceX = 5f;
    public float boostForceY = 5f;


    public void BoomerandBoost()
    {
        velocity = Vector3.zero;
        velocity.x += boomerangDash.boostDir.x * boostForceX;
        velocity.y += boomerangDash.boostDir.y * boostForceY;
    }


    /// <summary>
    /// Method for handling jump logic
    /// </summary>
    void HandleJump()
    {
        if (playerInput.jumping)
        {
            OnJumpInputDown();
        }
        else if(!playerInput.jumping && !IsSwinging)
        {
            OnJumpInputUp();
        }
    }

    void OnDashInput()
    {
        if(GameManager.instance.hasDashAbility)
            playerDash.OnDashInput();
    }

    void HandleDash()
    {
        playerDash.airborne = (!controller.collitions.below && !WallSliding);
        boomerangDash.airborne = (!controller.collitions.below && !WallSliding);

        playerDash.DashController(ref velocity, playerInput, playerSettings);
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
    public void CalculateVelocity(float velocityXSmoothing)
    {
        float targetVelocityX = playerSettings.MoveSpeed * playerInput.directionalInput.x;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collitions.below ? playerSettings.AccelerationTimeGrounded : playerSettings.AccelerationTimeAirborne));
        if (playerDash.isDashing)
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Clamp(velocity.y, maxFallSpeed, 1000);
        }

    }

    /// <summary>
    /// Method for handling player movement on a slope, the player slides down if he is standing on a max slope  
    /// </summary>
    void HandleMaxSlope()
    {
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
    void HandleWallSliding(float velocityXSmoothing)
    {
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
    }

    void HandleJumpInput()
    {
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
                //Debug.Log("Jump");
                dustParticles.Play();
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
                }
            }
        }
    }
    /// <summary>
    /// Method that handles jump logic when the player lets go of the jump button, allows the player to control the jump amount 
    /// </summary>
    public void OnJumpInputUp()
    {
        //Debug.Log("OnJumpInputUp");
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = 0;
        }
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
                transformToMove.localScale = new Vector3(-1, 1, -1);
                playerDash.AfterImage.transform.localScale = new Vector3(-4, 4, 0);
            }
            else if (playerInput.directionalInput.x > 0)
            {
                transformToMove.localScale = new Vector3(1, 1, -1);
                playerDash.AfterImage.transform.localScale = new Vector3(4, 4, 0);
            }
        }
    }

    /// <summary>
    /// Method that pulls the player towards the hook point
    /// </summary>
    public void Swing()
    {
        var playerToHookDirection = (RopeHook - (Vector2)transformToMove.position).normalized;
        var pullforce = playerToHookDirection * playerSettings.SwingForce;
        AddForce(pullforce);
    }

    /// <summary>
    /// Helper method for changing the players'velocity based on a force
    /// </summary>
    /// <param name="force">Force acting opon player</param>
    public void AddForce(Vector2 force)
    {
        velocity.x = Mathf.SmoothDamp(velocity.x, force.x, ref velocityXSmoothing, playerSettings.AccelerationTimeSwing);
        velocity.y = Mathf.SmoothDamp(velocity.y, force.y, ref velocityYSmoothing, playerSettings.AccelerationTimeSwing);
    }

}
