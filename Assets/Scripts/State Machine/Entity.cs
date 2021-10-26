using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;

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
    public int MaxHealth { get; set; }
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

    public DamageBox damageBox;

    private float currentStunResistance;
    private float lastDamageTime;

    private Vector2 velocityWorkspace;

    protected bool isStunned;
    public bool isDead;

    [Header("Debug")]
    public TMP_Text stateDebugText;

    [Header("Hit Effects")]
    [SerializeField] private GameObject damageNumberPrefab;

    public event Action<float, float> OnHitEnemy = delegate { };

    [Header("Aggro Settings")]
    public float minAggroRange = 5f;
    public float maxAggroRange = 8f;


    [Header("Movement Settings")]
    public bool isAffectedByGravity;
    public float gravity = -12;
    public float accelerationTimeGrounded = 0.05f;
    private float velocityXSmoothing = 0;

    protected ColouredFlash colouredFlash;
    protected CinemachineImpulseSource impulseListener;
    public virtual void Start()
    {
        facingDirection = 1;
        MaxHealth = (int)entityData.maxHealth;

        currentStunResistance = entityData.stunResistance;

        Health = MaxHealth;

        aliveGO = gameObject;
        rb = aliveGO.GetComponent<Rigidbody2D>();
        anim = aliveGO.GetComponent<Animator>();
        atsm = aliveGO.GetComponent<AnimationToStatemachine>();
        colouredFlash = GetComponent<ColouredFlash>();
        impulseListener = GetComponent<CinemachineImpulseSource>();
        stateMachine = new FiniteStateMachine();
    }

    public virtual void Update()
    {
        string state = stateMachine.currentState.ToString().Split('_')[1];
        stateDebugText.SetText(state);
        stateMachine.currentState.LogicUpdate();

        //anim.SetFloat("yVelocity", rb.velocity.y);

        if(Time.time >= lastDamageTime + entityData.stunRecoveryTime)
        {
            ResetStunResistance();
        }
    }

    public virtual void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();

        if (isAffectedByGravity)
        {
            float velocityY = CheckGround() ? 0 : rb.velocity.y + gravity * Time.deltaTime;
            SetVelocityY(velocityY);
        }
    }

    public virtual void SetVelocity(float velocity)
    {
        float targetVelocity = velocity * facingDirection;
        velocityWorkspace.x = Mathf.SmoothDamp(velocityWorkspace.x, targetVelocity, ref velocityXSmoothing, accelerationTimeGrounded);
        velocityWorkspace.y = rb.velocity.y;

        rb.velocity = velocityWorkspace;
    }


    public void SetVelocityY(float velocity)
    {
        velocityWorkspace.Set(velocityWorkspace.x, velocity);
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
        bool hit = Physics2D.Linecast(transform.position, GameManager.instance.player.transform.position, entityData.whatIsGround);
        return !hit && Physics2D.Raycast(playerCheck.position, aliveGO.transform.right, entityData.minAgroDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInMaxAgroRange()
    {
        bool hit = Physics2D.Linecast(transform.position, GameManager.instance.player.transform.position, entityData.whatIsGround);
        return !hit && Physics2D.Raycast(playerCheck.position, aliveGO.transform.right, entityData.maxAgroDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInCloseRangeAction()
    {
        return Physics2D.Raycast(playerCheck.position, aliveGO.transform.right, entityData.closeRangeActionDistance, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInMinAggroRadius()
    {
        return Physics2D.OverlapCircle(playerCheck.position, minAggroRange, entityData.whatIsPlayer);
    }

    public virtual bool CheckPlayerInMaxAggroRadius()
    {
        return Physics2D.OverlapCircle(playerCheck.position, maxAggroRange, entityData.whatIsPlayer);
    }

    public virtual void DamageHop(float velocity)
    {
        velocityWorkspace.Set(velocity, rb.velocity.y);
        rb.velocity = velocityWorkspace;
    }

    public virtual void ResetStunResistance()
    {
        isStunned = false;
        currentStunResistance = entityData.stunResistance;
    }

    protected void RaiseOnHitEnemyEvent(float health, float maxHealth)
    {
        var eh = OnHitEnemy;
        if (eh != null)
            OnHitEnemy(health, maxHealth);
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
        impulseListener.GenerateImpulse();
        if (colouredFlash != null)
        {
            colouredFlash.Flash(Color.white);
        }

        if (Health <= 0)
        {
            isDead = true;
            UI_HUD.instance.RefillFlask(entityData.flaskReffilAmount);
        }
    }

    public virtual void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        DamageHop(entityData.damageHopSpeed*dirX);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(playerCheck.position, minAggroRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerCheck.position, maxAggroRange);

        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.closeRangeActionDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.minAgroDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * entityData.maxAgroDistance), 0.2f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.transform.position, entityData.groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, wallCheck.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
        Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + (Vector3)(Vector2.down * entityData.ledgeCheckDistance));

    }

}
