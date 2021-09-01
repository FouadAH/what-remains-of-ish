using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private bool dashLock;
    private bool canDash;

    public bool isDashing;
    public bool airborne;

    [SerializeField] private ParticleSystem afterImage;

    public ParticleSystem AfterImage { get => afterImage; set => afterImage = value; }
    public ParticleSystem dashRechargeEffect;

    void Start()
    {
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
            StartCoroutine(DashTime(0.15f));

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
        yield return new WaitWhile(() => airborne);
        dashLock = false;
    }

    public IEnumerator DashTime(float dashCooldown)
    {
        isDashing = true;
        afterImage.Play();
        yield return new WaitForSeconds(dashCooldown);
        afterImage.Stop();
        isDashing = false;
    }
}
