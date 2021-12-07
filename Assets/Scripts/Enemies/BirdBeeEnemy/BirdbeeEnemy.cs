using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BirdbeeEnemy : Entity
{
    [HideInInspector]public AIDestinationSetter destinationSetter;
    [HideInInspector] public AIPath aIPath;

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
    }

    public override void DamageHop(float velocity){}

    public override void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        Vector3 knockbackForce = new Vector3(entityData.damageHopSpeed * dirX, entityData.damageHopSpeed * dirY, 0);
        StartCoroutine(KnockbackTimer(knockbackForce));
    }

    int frames;
    IEnumerator KnockbackTimer(Vector3 knockbackForce)
    {
        frames = 0;
        aIPath.canMove = false;

        yield return new WaitForEndOfFrame();

        rb.velocity = knockbackForce;
        while (frames < knockbackFrames)
        {
            frames++;
            yield return null;
        }
        rb.velocity = Vector3.zero;

        if (stateMachine.currentState != attackState)
            aIPath.canMove = true;
    }

    [Header("Aggro Settings")]
    public bool IsAggro = false;

    public bool CheckPlayerInAttackLine()
    {
        return Physics2D.Linecast(transform.position, attackDetectionPosition.transform.position, entityData.whatIsPlayer);
    }


    public override void ProcessHit(int amount)
    {
        base.ProcessHit(amount);
        IsAggro = true;
        if (isDead)
        {
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
