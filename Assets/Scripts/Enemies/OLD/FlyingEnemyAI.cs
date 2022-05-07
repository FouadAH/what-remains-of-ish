using UnityEngine;
using Pathfinding;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class FlyingEnemyAI : Entity, FiringAI, IDamagable
{
    public float updateRate = 0.5f;

    private Vector2 Ntarget;
    private Vector2 circle;

    [SerializeField] private GameObject TargetPoint;

    private Rigidbody2D rb;
    private AIDestinationSetter destinationSetter;
    private AIPath path;
    public float radius;

    public int maxHealth;
    public int KnockbackTaken = 10;

    public float fireRate { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float nextFireTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool canShootProjectiles = true;

    [SerializeField] private Vector2 attackPos;
    [SerializeField] private float attackRange;

    public event Action OnFire = delegate { };

    Transform player;
    Transform target;
    float rotateSpeed;

    [Header("Aggro Settings")]
    [SerializeField] public float AggroTime;
    [SerializeField] public LayerMask PlayerMask;
    [HideInInspector] public bool IsAggro;
    private IEnumerator aggroRangeRoutine;


    public override void Start()
    {
        base.Start();
        destinationSetter = GetComponent<AIDestinationSetter>();
        rb = GetComponent<Rigidbody2D>();
        path = GetComponent<AIPath>();

        player = GameManager.instance.player.transform;
        target = TargetPoint.transform;

        circle.x = transform.position.x;
        circle.y = transform.position.y;

        Health = MaxHealth;
        UpdateTarget();

        StartCoroutine(UpdatePath());
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Rotation();
    }

    public override void Update()
    {
        base.Update();

        if (IsAggro)
        {
            path.endReachedDistance = 3f;

            //if (IsPlayerInAttackRange())
            //{
            //    Shoot();
            //}

            StopCoroutine(AggroRange());
            StartCoroutine(AggroRange());
        }
        else
        {
            path.endReachedDistance = 0.2f;
        }
    }

    public virtual IEnumerator AggroRange()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, minAggroRange, PlayerMask);
        if (player == null)
        {
            yield return new WaitForSeconds(AggroTime);
            IsAggro = false;
            StopCoroutine(aggroRangeRoutine);
        }
        yield return new WaitForSeconds(0.5f);
        if (aggroRangeRoutine != null)
            StopCoroutine(aggroRangeRoutine);

        aggroRangeRoutine = AggroRange();
        StartCoroutine(aggroRangeRoutine);
    }


    public void Aggro()
    {
        IsAggro = true;
        if (aggroRangeRoutine != null)
            StopCoroutine(aggroRangeRoutine);

        aggroRangeRoutine = AggroRange();
        StartCoroutine(aggroRangeRoutine);
    }

    public void Rotation()
    {
        float AngleRad = Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        Quaternion q = Quaternion.AngleAxis(AngleDeg - 90, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotateSpeed);
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            if (IsAggro == false)
            {
                target = TargetPoint.transform;
                UpdateTarget();
            }
            else
            {
                target = player;
            }
            destinationSetter.target = target;
            yield return new WaitForSeconds(1f / updateRate);
        }

    }

    public bool IsPlayerInAttackRange()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(new Vector2(transform.position.x + (attackPos.x * transform.localScale.x),
            transform.position.y + (attackPos.y * transform.localScale.y)), attackRange, PlayerMask);
        return (playerCollider != null);
    }


    void Shoot()
    {
        if (CanFire() && canShootProjectiles)
        {
            nextFireTime = Time.time + fireRate;
            RaiseOnFireEvent();
        }
    }

    private void UpdateTarget()
    {
        Ntarget = (UnityEngine.Random.insideUnitCircle * radius) + circle;
        TargetPoint.transform.position = new Vector3( Ntarget.x, Ntarget.y);
    }

    public void ProcessHit(int amount, DamageType type)
    {
        Aggro();
        Health -= amount;
        RaiseOnHitEnemyEvent(Health, maxHealth);
        SpawnDamagePoints(amount);
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void KnockbackOnDamage(int amount, int dirX, int dirY)
    {
        rb.velocity = new Vector2(amount * dirX, amount * dirY);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(circle, radius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minAggroRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + (attackPos.x * transform.localScale.x),
            transform.position.y + (attackPos.y * transform.localScale.y)), attackRange);

    }

    public void RaiseOnFireEvent()
    {
        throw new NotImplementedException();
    }

    public bool CanFire()
    {
        throw new NotImplementedException();
    }
}
