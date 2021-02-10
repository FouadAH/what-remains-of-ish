using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    private bool dashLock;
    private bool canDash;
    public bool dashHover;
    public bool airborne;

    [SerializeField] private ParticleSystem afterImage;

    public ParticleSystem AfterImage { get => afterImage; set => afterImage = value; }

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
            if (velocity.y > 0)
            {
                velocity.y = 0;
            }

            StartCoroutine(DashLogic(playerSettings.DashCooldown));

            if (playerInput.directionalInput.x == 0)
            {
                velocity.x = transform.localScale.x * playerSettings.MoveSpeed * playerSettings.DashFactor;
            }
            else
            {
                velocity.x = ((Mathf.Sign(playerInput.directionalInput.x) == -1) ? -1 : 1) * playerSettings.MoveSpeed * playerSettings.DashFactor;
            }
        }
        canDash = false;

        return dashHover;
    }

    public IEnumerator DashLogic(float dashCooldown)
    {
        dashLock = true;
        dashHover = true;
        afterImage.Play();
        yield return new WaitForSeconds(dashCooldown);
        afterImage.Stop();
        dashHover = false;
        yield return new WaitWhile(() => airborne);
        dashLock = false;
    }
}
