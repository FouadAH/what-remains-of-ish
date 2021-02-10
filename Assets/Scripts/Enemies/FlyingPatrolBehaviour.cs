using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPatrolBehaviour : StateMachineBehaviour
{
    Controller_2D controller;
    Player player;
    Transform playerTransfrom;
    GameObject enemy;
    Flying_Enemy flyingEnemy;
    GameObject enemyHitBox;
    SpriteRenderer sprite;
    RaycastHit2D hit;
    public float gravity = -10;

    [HideInInspector]
    public Vector3 velocity;

    float velocityXSmoothing;

    public float accelerationTimeGrounded = .2f;

    float directionX = 1;
    float directionY = 1;

    public float moveSpeed = 5;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.gameObject.GetComponent<Flying_Enemy>().gm.player.GetComponent<Player>();
        playerTransfrom = player.transform;
        enemy = animator.gameObject;
        flyingEnemy = enemy.GetComponent<Flying_Enemy>();
        enemyHitBox = enemy.transform.GetChild(0).gameObject;
        controller = enemy.GetComponent<Controller_2D>();
        sprite = enemy.GetComponent<SpriteRenderer>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Flip();
        CalculateVelocity();
        controller.Move(velocity * Time.deltaTime);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public void CalculateVelocity()
    {
        if (Vector3.Distance(enemy.transform.position, flyingEnemy.flyPos) < flyingEnemy.flyRange)
        {
            if (controller.collitions.right)
            {
                velocity.x = 0;
                directionX = -1;
            }
            else if (controller.collitions.left)
            {
                velocity.x = 0;
                directionX = 1;
            }
            else if (controller.collitions.above)
            {
                velocity.y = 0;
                directionY = -1;
            }
            else if (controller.collitions.below)
            {
                velocity.y = 0;
                directionY = 1;
            }
        }
        else
        {
            velocity = Vector2.zero;
            directionY *= -1;
            directionX *= -1;
        }

        float targetVelocityX = moveSpeed * directionX;
        float targetVelocityY = moveSpeed * directionY;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTimeGrounded);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityXSmoothing, accelerationTimeGrounded);

    }

    public void Flip()
    {
        if (directionX == -1)
        {
            enemy.transform.localScale = new Vector2(-1, 1);
        }
        else if (directionX == 1)
        {
            enemy.transform.localScale = new Vector2(1, 1);
        }
    }
}
