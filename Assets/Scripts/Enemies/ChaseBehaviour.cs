using UnityEngine;

public class ChaseBehaviour : BaseEnemyFSM
{

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hit = Physics2D.Raycast(animator.transform.position + Vector3.right / 2f * directionX, Vector3.down, .5f + animator.transform.localScale.y, controller.collitionMask);
        directionX = (animator.transform.position.x < player.transform.position.x) ? 1 : -1;
        CalculateVelocity();
        HandleMaxSlope();
    }

    void CalculateVelocity()
    {
        if ((!hit && !controller.collitions.desendingSlope) || controller.collitions.right || controller.collitions.left )
        {
            enemy.GetComponent<BaseEnemy>().velocity.x = 0;
            enemy.GetComponent<BaseEnemy>().IsAggro = false;
            enemy.GetComponent<BaseEnemy>().anim.SetBool("isChasing", false);
        }
    }

}
