using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttackTest : MonoBehaviour
{
    Rigidbody2D rb;
    public LayerMask whatIsGround;
    public Transform goundCheck;
    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    Transform playerPosition;
    public float jumpHeight = 20f;

    void JumpAttack()
    {
        Debug.Log("Jump attack");
        playerPosition = GameManager.instance.player.transform;
        float directionToPlayer = playerPosition.position.x - transform.position.x;
        //SetVelocity(50f, new Vector2(directionToPlayer, jumpHeight), 1);
        rb.AddForce(new Vector2(directionToPlayer, jumpHeight), ForceMode2D.Impulse);
    }

    public virtual bool CheckGround()
    {
        return Physics2D.OverlapCircle(goundCheck.position, 2f, whatIsGround);
    }

    public void FixedUpdate()
    {
        //base.FixedUpdate();
        if (Input.GetKeyDown(KeyCode.E) && CheckGround())
        {
            JumpAttack();
        }
    }
}
