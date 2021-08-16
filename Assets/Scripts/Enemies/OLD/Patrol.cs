using UnityEngine;

public class Patrol : BaseEnemyFSM
{
    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hit = Physics2D.Raycast(animator.transform.position + Vector3.right / 2f * directionX, Vector3.down, .5f + animator.transform.localScale.y, controller.collitionMask);
        CalculateVelocity();
        HandleMaxSlope();
    }
   
    public void CalculateVelocity()
    {
        if (!hit && !controller.collitions.desendingSlope)
        {
            enemy.GetComponent<BaseEnemy>().velocity.x = 0;
            directionX *= -1;
        }
        else if (controller.collitions.right)
        {
            enemy.GetComponent<BaseEnemy>().velocity.x = 0;
            directionX = -1;
        }
        else if (controller.collitions.left)
        {
            enemy.GetComponent<BaseEnemy>().velocity.x = 0;
            directionX = 1;
        }
    }

}
