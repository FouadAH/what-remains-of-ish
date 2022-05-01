using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private bool dashLock;
    private bool canDash;
    public bool isDashing;
    public bool isDashingAnimation;

    public float floatTime = 0.1f;
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

    public bool DashController(ref Vector2 velocity, Player_Input playerInput, PlayerMovementSettings playerSettings)
    {
        if (canDash)
        {
            if (velocity.y != 0)
            {
                velocity.y = 0;
            }

            StartCoroutine(DashIFrames(dashIFrames));
            StartCoroutine(FloatTime(floatTime));
            StartCoroutine(DashLogic(playerSettings.DashCooldown));
            StartCoroutine(DashEffect(effectTime));
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Dash", GetComponent<Transform>().position);

            float dashSpeedMod = (playerMovement.isAirborne) ? 3 : playerSettings.DashSpeedModifier;
            if (playerInput.directionalInput.x == 0)
            {
                velocity.x = transform.localScale.x * playerSettings.MoveSpeed * dashSpeedMod;
            }
            else
            {
                velocity.x = ((Mathf.Sign(playerInput.directionalInput.x) == -1) ? -1 : 1) * playerSettings.MoveSpeed * dashSpeedMod;
            }
        }
        canDash = false;

        return isDashing;
    }

    public void ResetDash()
    {
        dashLock = false;
    }

    public IEnumerator DashLogic(float dashCooldown)
    {
        dashLock = true;
        yield return new WaitForSeconds(dashCooldown);
        dashRechargeEffect.Play();
        yield return new WaitWhile(() => playerMovement.isAirborne);
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
