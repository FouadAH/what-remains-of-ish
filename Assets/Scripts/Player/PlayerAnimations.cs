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

    public PlayerAnimations(Animator animator, Transform player)
    {
        playerInput = player.GetComponent<Player_Input>();
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
        else if( jumping || falling )
        {
            animator.SetTrigger("AirAttack");
        }
        else
        {
            animator.SetTrigger("Attack");
        }
    }

    public void Animate(PlayerMovement playerMovement)
    {
        animator.SetFloat("Speed", Mathf.Abs(playerInput.directionalInput.x));
        
        if (playerMovement.Velocity.y < 0)
        {
            falling = true;
            jumping = false;
            animator.SetBool("isFalling", true);
            animator.SetBool("isJumping", false);
        }
        if (playerMovement.Velocity.y == 0)
        {
            falling = false;
            animator.SetBool("isFalling", false);
        }

        animator.SetBool("isWallSliding", playerMovement.WallSliding);

        //if (!invinsible)
        //{
        //    animator.setbool("invinsible", false);
        //}
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
