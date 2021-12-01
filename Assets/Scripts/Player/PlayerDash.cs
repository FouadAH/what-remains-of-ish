using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private bool dashLock;
    private bool canDash;
    public bool isDashing;
    public float floatTime = 0.1f;

    [SerializeField] private ParticleSystem afterImage;

    public ParticleSystem AfterImage { get => afterImage; set => afterImage = value; }
    public ParticleSystem dashRechargeEffect;

    PlayerMovement playerMovement;
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
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

            StartCoroutine(DashLogic(playerSettings.DashCooldown));
            StartCoroutine(FloatTime(floatTime));

            if (playerInput.directionalInput.x == 0)
            {
                velocity.x = transform.localScale.x * playerSettings.MoveSpeed * playerSettings.DashSpeedModifier;
            }
            else
            {
                velocity.x = ((Mathf.Sign(playerInput.directionalInput.x) == -1) ? -1 : 1) * playerSettings.MoveSpeed * playerSettings.DashSpeedModifier;
            }
        }
        canDash = false;

        return isDashing;
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
        afterImage.Play();
        yield return new WaitForSeconds(floatTime);
        afterImage.Stop();
        isDashing = false;
    }
}
