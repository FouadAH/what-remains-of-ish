using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Controller_2D))]
public class Enemy : MonoBehaviour {

    Controller_2D controller;

    Player player;
    Transform playerTransfrom;
    public LayerMask viewMask;

    public float lookRadius;
    float distance;
    
    public Transform pathHolder;
    Vector3[] waypoints;

    public float waitTime;

    float Health;
    public float maxHealth;

    public Vector2 knockback;

    [HideInInspector]
    public Vector3 velocity;

    float velocityXSmoothing;

    float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .2f;

    public float moveSpeed = 5;

    public float gravity = -10;
    float directionX =1;
    float playerX;
    RaycastHit2D hit;

    SpriteRenderer sprite;
    Color colorStart = Color.white;
    Color colorEnd = Color.red;
    float duration = .02F;
    public float damageDealt;

    bool isPaused = true;
    int targetWayPointIndex = 0;

    Animator anim;

    GameManager gm;

    private void Awake()
    {
        gm = FindObjectOfType<GameManager>();
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        player = gm.player.GetComponent<Player>();
        playerTransfrom = player.transform;
        Health = maxHealth;
        controller = GetComponent<Controller_2D>();
        waypoints = new Vector3[pathHolder.childCount];
        for(int i =0; i<pathHolder.childCount; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
        }
        StartCoroutine(AutoMove());
        
    }

    private void Update()
    {
        hit = Physics2D.Raycast(transform.position + Vector3.right / 2f * directionX, Vector3.down, .5f + transform.localScale.y, controller.collitionMask);
        distance = Vector2.Distance(playerTransfrom.position, transform.position);
        playerX = (transform.position.x < playerTransfrom.position.x) ? 1 : -1;
        Flip();
        if (CanSeePlayer())
        {
            anim.SetBool("isChasing", true);
        }
        else
        {
            anim.SetBool("isChasing", false);
        }
        CalculateVelocity();
        controller.Move(velocity * Time.deltaTime);

        if (controller.collitions.above || controller.collitions.below)
        {
            if (controller.collitions.slidingDownMaxSlope)
            {
                
                velocity.y += controller.collitions.slopeNormal.y * gravity * -1 * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }

        }
        pathHolder.position = new Vector3(pathHolder.position.x, transform.position.y, 0);
        

    }
    public void Flip()
    {
        if (directionX == -1)
        {
            sprite.flipX = true;
        }
        else if (directionX == 1)
        {
            sprite.flipX = false;
        }
    }


    IEnumerator AutoMove()
    {
        while (true)
        {
            
            if (isPaused)
            {
                yield return null;
            }
            if (!hit && !controller.collitions.desendingSlope)
            {
                velocity.x = 0;
                directionX *=-1;
                yield return new WaitForSeconds(waitTime);
            }
            else if (controller.collitions.right)
            {
                velocity.x = 0;
                directionX = -1;
                yield return new WaitForSeconds(waitTime);
            }
            else if (controller.collitions.left)
            {
                velocity.x = 0;
                directionX = 1;
                yield return new WaitForSeconds(waitTime);
            }
            float targetVelocity = moveSpeed * directionX;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity, ref velocityXSmoothing, (controller.collitions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
            yield return null;
        }
    }
    
    IEnumerator FollowPath(Vector3[] waypoints) 
    {   
        Vector3 targetWaypoint = waypoints[targetWayPointIndex];
        directionX = (transform.position.x < targetWaypoint.x) ? 1 : -1;
       
        while (true)
        {
            if(isPaused){
                yield return null;
            }
            else if ((transform.position.x >= targetWaypoint.x && directionX==1) || (transform.position.x <= targetWaypoint.x && directionX == -1))
            {
                velocity.x = 0;
                targetWayPointIndex = (targetWayPointIndex + 1) % waypoints.Length;
                targetWaypoint = waypoints[targetWayPointIndex];
                directionX = (transform.position.x < targetWaypoint.x) ? 1 : -1;
                yield return new WaitForSeconds(waitTime);
            }
            yield return null;

        }
    }
    
    void Aggro()
    {
        directionX = playerX;
        float targetVelocity = moveSpeed * directionX;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity, ref velocityXSmoothing, (controller.collitions.below ? accelerationTimeGrounded : accelerationTimeAirborne));
    }

    bool CanSeePlayer()
    {
        RaycastHit2D hit= Physics2D.Linecast(transform.position, playerTransfrom.position, viewMask);
        if (distance < lookRadius && !hit)
        {
            return true;
        }
        return false;   
    }
        
    void CalculateVelocity()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    public void TakeDamage(int dmg)
    {
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            Aggro();
            velocity += 20 * Vector3.Normalize(transform.position - player.transform.position);
            Health -= dmg;
        }
        
        float lerp = Mathf.PingPong(Time.deltaTime, duration) / duration;
        sprite.material.color = Color.Lerp(colorStart, colorEnd, lerp);
        
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        float size = .3f;

        Gizmos.color = new Color(255,0,0);
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawLine(waypoint.position - Vector3.up * size, waypoint.position + Vector3.up * size);
            Gizmos.DrawLine(waypoint.position - Vector3.left * size, waypoint.position + Vector3.left * size);
            previousPosition = waypoint.position;
        }
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger != true && collision.tag == "Player" )
        {
            //player.DealDamage(damageDealt, Vector3.Normalize(player.transform.position - transform.position));
        }
    }
}
