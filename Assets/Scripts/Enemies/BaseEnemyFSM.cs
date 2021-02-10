using UnityEngine;

public class BaseEnemyFSM : StateMachineBehaviour
{
    public Controller_2D controller;
    public Player player;
    public GameObject enemyGameObject;
    public IEnemy enemy;
    public float directionX;

    public RaycastHit2D hit;
    [HideInInspector] public Vector3 velocity;

    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemyGameObject = animator.gameObject;
        enemy = enemyGameObject.GetComponent<IEnemy>();
        velocity = enemyGameObject.GetComponent<BaseEnemy>().velocity;
        player = GameManager.instance.player.GetComponent<Player>();
        controller = enemyGameObject.GetComponent<Controller_2D>();
    }

    public void Flip()
    {
        if (directionX == -1)
        {
            enemy.transform.localScale = new Vector3(Mathf.Abs(enemy.transform.localScale.x) * -1, Mathf.Abs(enemy.transform.localScale.y), -1);
        }
        else if (directionX == 1)
        {
            enemy.transform.localScale = new Vector3(Mathf.Abs(enemy.transform.localScale.x) * 1, Mathf.Abs(enemy.transform.localScale.y), -1);
        }
    }


    public void HandleMaxSlope()
    {
        if (controller.collitions.above || controller.collitions.below)
        {
            Flip();
            velocity.y = 0;
        }
    }
}
