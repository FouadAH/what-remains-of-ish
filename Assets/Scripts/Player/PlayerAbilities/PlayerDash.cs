using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private bool dashLock;
    private bool canDash;
    public bool isDashing;
    public bool isDashingAnimation;

    public float groundDashTime = 0.1f;
    public float airDashTime = 0.1f;

    public float effectTime = 0.2f;
    public float dashIFrames = 0.1f;

    public Material dashMaterial;
    [SerializeField] private ParticleSystem afterImage;
    public GameObject dashTrailParent;
    public ParticleSystem dashPaticles;

    public ParticleSystem AfterImage { get => afterImage; set => afterImage = value; }
    public ParticleSystem dashRechargeEffect;

    PlayerMovement playerMovement;
    Player player;
    SpriteRenderer spriteRenderer;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        player = GetComponent<Player>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        afterImage.Pause();
    }

    public void OnDashInput()
    {
        if (!dashLock)
        {
            canDash = true;
        }
    }
    bool wasAirborne = false;

    public bool DashController(ref Vector2 velocity, Player_Input playerInput, PlayerMovementSettings playerSettings)
    {
        if (canDash)
        {
            wasAirborne = playerMovement.isAirborne;

            if (velocity.y != 0)
            {
                velocity.y = 0;
            }

            float dashTime = (playerMovement.isAirborne) ? airDashTime : groundDashTime;
            float dashSpeedMod = (playerMovement.isAirborne) ? playerSettings.AirDashSpeed : playerSettings.DashSpeed;

            if (playerInput.directionalInput.x == 0)
            {
                playerMovement.dashSpeed = transform.localScale.x * dashSpeedMod;
            }
            else
            {
                playerMovement.dashSpeed = ((Mathf.Sign(playerInput.directionalInput.x) == -1) ? -1 : 1) * dashSpeedMod;
            }

            if (player.iFrameRoutine != null)
            {
                if (player.iFrames < dashIFrames)
                {
                    StopCoroutine(player.iFrameRoutine);
                    StartCoroutine(player.DamageIFrames(dashIFrames));
                }
            }
            else
            {
                StartCoroutine(player.DamageIFrames(dashIFrames));
            }

            StartCoroutine(FloatTime(dashTime));
            StartCoroutine(DashLogic(playerSettings.DashCooldown));
            StartCoroutine(DashEffect(effectTime));
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Dash", GetComponent<Transform>().position);
        }
        canDash = false;

        return isDashing;
    }

    public void ResetDash()
    {
        dashLock = false;
    }

    public void StopDash()
    {
        isDashing = false;
        isDashingAnimation = false;
    }

    public IEnumerator DashLogic(float dashCooldown)
    {
        dashLock = true;
        yield return new WaitForSeconds(dashCooldown);
        dashRechargeEffect.Play();
        yield return new WaitWhile(() => wasAirborne && playerMovement.isAirborne);
        dashLock = false;
    }

    public IEnumerator FloatTime(float floatTime)
    {
        isDashing = true;
        yield return new WaitForSeconds(floatTime);
        isDashing = false;
    }

    public IEnumerator DashIFrames(float floatTime)
    {
        player.invinsible = true;
        yield return new WaitForSeconds(floatTime);
        player.invinsible = false;
    }

    public IEnumerator DashEffect(float effectTime)
    {
        dashPaticles.Play();
        isDashingAnimation = true;
        foreach (TrailRenderer jumpTrail in dashTrailParent.GetComponentsInChildren<TrailRenderer>())
        {
            jumpTrail.emitting = true;
        }

        yield return new WaitForSeconds(effectTime);

        isDashingAnimation = false;
        foreach (TrailRenderer jumpTrail in dashTrailParent.GetComponentsInChildren<TrailRenderer>())
        {
            jumpTrail.emitting = false;
        }
    }
}
