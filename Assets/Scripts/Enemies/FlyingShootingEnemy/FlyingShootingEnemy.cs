using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingShootingEnemy : Entity, FiringAI
{
    [HideInInspector] public AIDestinationSetter destinationSetter;
    [HideInInspector] public AIPath aIPath;

    [Header("Fly Settings")]
    public GameObject TargetPoint;
    [HideInInspector] public Vector2 startPosition;
    public int knockbackFrames = 5;

    [Header("Projectile Firing Settings")]
    public float fireRate;
    [HideInInspector] public float nextFireTime;
    public event System.Action OnFire = delegate { };
    float FiringAI.fireRate { get => fireRate; set => _ = fireRate; }
    float FiringAI.nextFireTime { get => nextFireTime; set => _ = nextFireTime; }

    [Header("Attack Settings")]
    public GameObject attackDetectionPosition;
    public float attackDetectionRadius = 3f;
    
    public FlyState flyState { get; private set; }
    public PlayerDetectedState playerDetectedState { get; private set; }
    public ShootState shootState { get; private set; }
    public DeadState deadState { get; private set; }

    [Header("States")]
    [SerializeField] private D_FlyState flyStateData;
    [SerializeField] private D_PlayerDetected playerDetectedData;
    [SerializeField] private D_RangedAttackState rangedAttackStateData;
    [SerializeField] private D_DeadState deadStateData;

    public ProjectileController projectileController;

    public override void Start()
    {
        base.Start();
        startPosition = transform.position;
        destinationSetter = GetComponent<AIDestinationSetter>();
        aIPath = GetComponent<AIPath>();

        flyState = new FlyingShootingEnemy_FlyState(this, stateMachine, "fly", flyStateData, this);
        playerDetectedState = new FlyingShootingEnemy_PlayerDetectedState(this, stateMachine, "fly", playerDetectedData, this);
        shootState = new FlyingShootingEnemy_ShootState(this, stateMachine, "attack", attackDetectionPosition.transform, rangedAttackStateData, projectileController, this);
        deadState = new FlyingShootingEnemy_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(flyState);
    }

    public override void DamageHop(float velocity) { }

    public override void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        Vector3 knockbackForce = new Vector3(entityData.damageHopSpeed * dirX, entityData.damageHopSpeed * dirY, 0);
        StartCoroutine(KnockbackTimer(knockbackForce));
    }

    public bool CheckPlayerInAttackLine()
    {
        return Physics2D.Linecast(transform.position, attackDetectionPosition.transform.position, entityData.whatIsPlayer);
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

        if (stateMachine.currentState != shootState)
            aIPath.canMove = true;
    }

    [Header("Aggro Settings")]
    public bool IsAggro = false;

    public void RaiseOnFireEvent()
    {
        var eh = OnFire;
        if (eh != null)
            OnFire();
    }

    public bool CanFire()
    {
        return Time.time >= nextFireTime;
    }

    public override void ProcessHit(int amount, DamageType type)
    {
        base.ProcessHit(amount, DamageType.Melee);
        IsAggro = true;
        if (isDead && stateMachine.currentState != deadState)
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
        Gizmos.DrawWireSphere(attackDetectionPosition.transform.position, attackDetectionRadius);

    }
}
