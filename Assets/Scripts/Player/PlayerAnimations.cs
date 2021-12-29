using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations
{
    private Animator animator;
    private Player_Input playerInput;
    private bool falling;
    private bool jumping;

    PlayerMovement playerMovement;
    PlayerDash playerDash;
    public PlayerAnimations(Animator animator, Transform player)
    {
        playerInput = player.GetComponent<Player_Input>();
        playerMovement = player.GetComponent<PlayerMovement>();
        playerDash = player.GetComponent<PlayerDash>();
        this.animator = animator;
        playerInput.OnJumpUp += OnJumpInputUp;
        playerInput.OnJumpDown += OnJumpInputDown;
        playerInput.OnAttack += OnAttack;
    }

    private void OnAttack()
    {
        if (playerInput.directionalInput.y > 0)
        {
            animator.SetTrigger("isAttackingUp");
        }
        else if (playerInput.directionalInput.y < 0)
        {
            animator.SetTrigger("isAttackingDown");
        }
        else
        {
            animator.SetTrigger("Attack");
        }
    }

    public void Animate()
    {
        animator.SetFloat("Speed", Mathf.Abs(playerInput.directionalInput.x));
        if (playerMovement.Velocity.y < 0)
        {
            falling = true;
            jumping = false;
        }
        else if(playerMovement.Velocity.y > 0)
        {
            jumping = true;
            falling = false;
        }
        else if (playerMovement.Velocity.y == 0)
        {
            falling = false;
            jumping = false;
        }

        animator.SetBool("isFalling", falling);
        animator.SetBool("isJumping", jumping);
        animator.SetBool("isWallSliding", playerMovement.WallSliding);
        animator.SetBool("isDashing", playerDash.isDashingAnimation);

    }

    public void OnJumpInputDown()
    {
        animator.SetBool("isJumping", true);
        jumping = true;
    }

    public void OnJumpInputUp()
    {
        animator.SetBool("isJumping", false);
        jumping = false;
    }
}
