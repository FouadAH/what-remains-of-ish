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
        playerInput.OnUpAttack += UpAttack;
    }

    public void DownAttack()
    {
        animator.Play("Down_Attack", 3);
    }

    public void UpAttack()
    {
        animator.SetTrigger("isAttackingUp");
    }

    private void OnAttack()
    {

        if (animator.GetCurrentAnimatorStateInfo(3).IsName("Attack_1"))
        {
            animator.SetBool("Attack_2", true);
        }
        else
        {
            animator.SetBool("Attack_1", true);
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

    public void CancelDownAttack()
    {
        //animator.SetBool("DownAttack", false);
        //animator.SetTrigger("isAttackingDown");
        animator.Play("Default", 3);

    }

    public void UnsubscribeAnimationsFromInput()
    {
        playerInput.OnJumpUp -= OnJumpInputUp;
        playerInput.OnJumpDown -= OnJumpInputDown;
        playerInput.OnAttack -= OnAttack;
    }
}
