using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Entity : MonoBehaviour, IDamagable
{
    public FiniteStateMachine stateMachine;
    public AnimationToStatemachine atsm;

    public D_Entity entityData;

    public int facingDirection { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public Animator anim { get; private set; }
    public GameObject aliveGO { get; private set; }
    public int lastDamageDirection { get; private set; }

    [SerializeField] private int maxHealth;
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float Health { get; set; }
    public int knockbackGiven { get => (int)entityData.damageHopSpeed; set => knockbackGiven = value; }

    [Header("Checkers")]
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Transform ledgeCheck;
    [SerializeField]
    private Transform playerCheck;
    [SerializeField]
    private Transform groundCheck;

    private float currentHealth;
    private float currentStunResistance;
    private float lastDamageTime;

    private Vector2 velocityWorkspace;

    protected bool isStunned;
    protected bool isDead;

    public TMP_Text stateDebugText;

    public virtual void Start()
    {
        facingDirection = 1;
        currentHealth = entityData.maxHealth;
        currentStunResistance = entityData.stunResistance;

        Health = MaxHealth;

        aliveGO = gameObject;
        rb = aliveGO.GetComponent<Rigidbody2D>();
        anim = aliveGO.GetComponent<Animator>();
        atsm = aliveGO.GetComponent<AnimationToStatemachine>();
        stateMachine = new FiniteStateMachine();
    }

    public virtual void Update()
    {
        string state = stateMachine.currentState.ToString().Split('_')[1];
        stateDebugText.SetText(state);
        stateMachine.currentState.LogicUpdate();

        anim.SetFloat("yVelocity", rb.velocity.y);

        if(Time.time >= lastDamageTime + entityData.stunRecoveryTime)
        {
            ResetStunResistance();
        }
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public virtual void SetVelocity(float velocity)
    {
        velocityWorkspace.Set(facingDirection * velocity, rb.velocity.y);
        rb.velocity = velocityWorkspace;
    }

    public virtual void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        velocityWorkspace.Set(angle.x * velocity * direction, angle.y * velocity);
        rb.velocity = velocityWorkspace;
    }

    public virtual bool CheckWall()
    {
        return Physics2D.Raycast(wallCheck.position, aliveGO.transform.right, entityData.wallCheckDistance, entityData.whatIsGround);
    }

    public virtual bool CheckLedge()
    {
        return Physics2D.Raycast(ledgeCheck.position, Vector2.down, entityData.ledgeCheckDistance, entityData.whatIsGround);
    }

    public virtual bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, entityData.groundCheckRadius, entityData.whatIsGround);
    }

    public virtual bool CheckPlayerInMinAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGO.transform.right, entityData.minAgroDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInMaxAgroRange()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGO.transform.right, entityData.maxAgroDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGO.transform.right, entityData.closeRangeActionDistance, entityData.whatIsPlayer);
    }

    public virtual void DamageHop(float velocity)
    {
        //velocityWorkspace.Set(rb.velocity.x, velocity);
        //rb.velocity = velocityWorkspace;
    }

    public virtual void ResetStunResistance()
    {
        isStunned = false;
        currentStunResistance = entityData.stunResistance;
    }

    [SerializeField] public bool IsAggro;
    [SerializeField] public float AggroTime;
    [SerializeField] public LayerMask PlayerMask;
    [SerializeField] private GameObject damageNumberPrefab;
    public float aggroRange = 2f;

    public event Action<float, float> OnHitEnemy = delegate { };

    private IEnumerator aggroRangeRoutine;

    protected void RaiseOnHitEnemyEvent(float health, float maxHealth)
    {
        var eh = OnHitEnemy;
        if (eh != null)
            OnHitEnemy(health, maxHealth);
    }

    public virtual IEnumerator AggroRange()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, aggroRange, PlayerMask);
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

    public void SpawnDamagePoints(int damage)
    {
        float x = UnityEngine.Random.Range(transform.position.x - 1f, transform.position.x + 1f);
        float y = UnityEngine.Random.Range(transform.position.y - 0.5f, transform.position.y + 0.5f);
        GameObject damageNum = Instantiate(damageNumberPrefab, new Vector3(x, y, 0), Quaternion.identity);
        damageNum.GetComponent<DamageNumber>().Setup(damage);
    }

    //public abstract bool CanSeePlayer();

    public virtual void Flip()
    {
        facingDirection *= -1;
        aliveGO.transform.Rotate(0f, 180f, 0f);
        stateDebugText.transform.Rotate(0f, 180f, 0f);
    }

    public virtual void ModifyHealth(int amount)
    {
        Health -= amount;
        RaiseOnHitEnemyEvent(Health, MaxHealth);
        if (Health <= 0)
        {
            isDead = true;
        }
        else
        {
            Aggro();
            SpawnDamagePoints(amount);
            anim.SetTrigger("Hit");
        }
    }

    private IEnumerator Die()
    {
        anim.SetLayerWeight(0, 0f);
        anim.SetLayerWeight(1, 0f);
        anim.SetLayerWeight(2, 1f);
        anim.SetBool("isDead", true);
        CoinSpawner();
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        gameObject.SetActive(false);
    }

    public GameObject coinPrefab;
    public int coinDrop;
    public void CoinSpawner()
    {
        for (int i = 0; i < coinDrop; i++)
        {
            GameObject.Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }

    public void KnockbackOnDamage(int amount, int dirX, int dirY)
    {
        DamageHop(entityData.damageHopSpeed);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * entityData.ledgeCheckDistance));

        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.closeRangeActionDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.minAgroDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.maxAgroDistance), 0.2f);
    }

}
