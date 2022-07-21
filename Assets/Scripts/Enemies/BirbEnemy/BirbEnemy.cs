using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbEnemy : Entity
{
    [HideInInspector] public AIDestinationSetter destinationSetter;
    [HideInInspector] public AIPath aIPath;
    Seeker seeker;

    [Header("Fly Settings")]
    public GameObject TargetPoint;
    [HideInInspector] public Vector2 startPosition;
    public int knockbackFrames = 5;

    public FlyState flyState { get; private set; }
    public PlayerDetectedState playerDetectedState { get; private set; }
    public DeadState deadState { get; private set; }

    [Header("Aggro Settings")]
    public bool IsAggro = false;

    [Header("States")]
    [SerializeField] private D_FlyState flyStateData;
    [SerializeField] private D_PlayerDetected playerDetectedData;
    [SerializeField] private D_DeadState deadStateData;

    [FMODUnity.EventRef] public string birdbeeFlyLoop;

    public override void Start()
    {
        base.Start();
        startPosition = transform.position;
        destinationSetter = GetComponent<AIDestinationSetter>();
        aIPath = GetComponent<AIPath>();

        flyState = new BlueBirbEnemy_FlyState(this, stateMachine, "move", flyStateData, this);
        playerDetectedState = new BlueBirdEnemy_PlayerDetectedState(this, stateMachine, "detected", playerDetectedData, this);
        deadState = new BirbEnemy_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(flyState);
        GetComponent<EnemyAudio>().PlayEventOnce(birdbeeFlyLoop);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        if (!isKnockback)
        {
            return;
        }

        if (CheckGround())
        {
            rb.velocity = Vector2.zero;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        Vector3 knockbackForce = new Vector3(entityData.damageHopSpeed * dirX, entityData.damageHopSpeed * dirY, 0);
        StartCoroutine(KnockbackTimer(knockbackForce));
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);
    }

    bool isKnockback = false;
    IEnumerator KnockbackTimer(Vector3 knockbackForce)
    {
        isKnockback = true;
        aIPath.canMove = false;
        aIPath.canSearch = false;

        yield return new WaitForFixedUpdate();

        rb.velocity = knockbackForce;
        knockbackTimeElapsed = 0;
        while (knockbackTimeElapsed < knockbackTime)
        {
            knockbackTimeElapsed += Time.deltaTime;
            rb.velocity = Vector2.SmoothDamp(rb.velocity, Vector2.zero, ref velocitySmoothing, smoothTime);
            if (CheckGround())
            {
                rb.velocity = Vector2.zero;
                break;
            }

            yield return null;
        }
        rb.velocity = Vector2.zero;
        TargetPoint.transform.position = transform.position;
        aIPath.canMove = true;
        aIPath.canSearch = true;
        isKnockback = false;
    }
    
    public override void ProcessHit(int amount, DamageType type)
    {
        base.ProcessHit(amount, type);
        if (isDead && stateMachine.currentState != deadState)
        {
            GetComponent<EnemyAudio>().StopPlayingEvent();
            stateMachine.ChangeState(deadState);
        }
        else
        {
            stateMachine.ChangeState(playerDetectedState);
        }
    }

    public override void LoadDefaultData()
    {
        base.LoadDefaultData();
    }


    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(startPosition, flyStateData.radius);

        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
    }
}
