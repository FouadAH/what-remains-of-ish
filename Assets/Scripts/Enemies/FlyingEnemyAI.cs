using UnityEngine;
using Pathfinding;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class FlyingEnemyAI : FiringAI, IDamagable
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

    public float Health { get; set; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int knockbackGiven { get => KnockbackTaken; set => KnockbackTaken = value; }

    private void Start()
    {
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

    void FixedUpdate()
    {
        Rotation();
    }

    void Update()
    {
        if (IsAggro)
        {
            path.endReachedDistance = 3f;
            if (CanFire())
            {
                nextFireTime = Time.time + fireRate;
                RaiseOnFireEvent();
            }
            StopCoroutine(AggroRange());
            StartCoroutine(AggroRange());
        }
        else
        {
            path.endReachedDistance = 0.2f;
        }
    }
    
    IEnumerator UpdatePath()
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
        StartCoroutine(UpdatePath());

    }
    
    private void UpdateTarget()
    {
        Ntarget = (UnityEngine.Random.insideUnitCircle * radius) + circle;
        TargetPoint.transform.position = new Vector3( Ntarget.x, Ntarget.y);
    }

    public void ModifyHealth(int amount)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(circle, radius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
