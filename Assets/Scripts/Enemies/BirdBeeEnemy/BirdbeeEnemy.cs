using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BirdbeeEnemy : Entity
{
    [HideInInspector]public AIDestinationSetter destinationSetter;
    [HideInInspector] public AIPath aIPath;
    Seeker seeker;

    [Header("Fly Settings")]
    public GameObject TargetPoint;
    [HideInInspector]public Vector2 startPosition;
    public int knockbackFrames = 5;

    [Header("BirdbeeEnemy Settings")]
    public GameObject attackDetectionPosition;
    public float attackDetectionRadius = 3f;

    public FlyState flyState { get; private set; }
    public PlayerDetectedState playerDetectedState { get; private set; }
    public MeleeAttackState attackState { get; private set; }
    public DeadState deadState { get; private set; }

    [Header("States")]
    [SerializeField] private D_FlyState flyStateData;
    [SerializeField] private D_PlayerDetected playerDetectedData;
    [SerializeField] private D_MeleeAttack attackStateData;
    [SerializeField] private D_DeadState deadStateData;

    [FMODUnity.EventRef] public string birdbeeFlyLoop;

    public override void Start()
    {
        base.Start();
        startPosition = transform.position;
        destinationSetter = GetComponent<AIDestinationSetter>();
        aIPath = GetComponent<AIPath>();

        flyState = new BirdbeeEnemy_FlyState(this, stateMachine, "move", flyStateData, this);
        playerDetectedState = new BirdbeeEnemy_PlayerDetectedState(this, stateMachine, "detected", playerDetectedData, this);
        attackState = new BirdbeeEnemy_AttackState(this, stateMachine, "attack", attackDetectionPosition.transform, attackStateData, this);
        deadState = new BirdbeeEnemy_DeadState(this, stateMachine, "dead", deadStateData, this);

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
        stateMachine.currentState.PhysicsUpdate();

        if (CheckGround())
        {
            rb.velocity = Vector2.zero;
        }

    }

    public override void DamageHop(float velocity){}

    public override void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        Vector3 knockbackForce = new Vector3(entityData.damageHopSpeed * dirX, entityData.damageHopSpeed * dirY, 0);
        StartCoroutine(KnockbackTimer(knockbackForce));
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);
        //rb.velocity = knockbackForce;
    }

    int frames;

    bool isKnockback = false;
    IEnumerator KnockbackTimer(Vector3 knockbackForce)
    {
        frames = 0;
        aIPath.canMove = false;
        aIPath.canSearch = false;
        isKnockback = true;

        yield return new WaitForEndOfFrame();

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

        if (stateMachine.currentState != attackState)
        {
            aIPath.canMove = true;
            aIPath.canSearch = true;
        }
        isKnockback = false;
    }

    [Header("Aggro Settings")]
    public bool IsAggro = false;

    public bool CheckPlayerInAttackLine()
    {
        return Physics2D.Linecast(transform.position, attackDetectionPosition.transform.position, entityData.whatIsPlayer);
    }


    public override void ProcessHit(int amount, DamageType type)
    {
        base.ProcessHit(amount, type);
        IsAggro = true;
        if (isDead && stateMachine.currentState != deadState)
        {
            GetComponent<EnemyAudio>().StopPlayingEvent();
            stateMachine.ChangeState(deadState);
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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackDetectionPosition.transform.position, 0.5f);
    }
}
