using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Controller_2D))]
public class BaseEnemy : IEnemy, IBaseStats
{
    private Controller_2D controller;

    [SerializeField] private Vector2 attackPos;
    
    [SerializeField] private float attackRange;

    public Vector3 velocity;
    private AttackProcessor attackProcessor;
    public Animator anim;
    
    private bool isDead = false;

    public int coinDrop = 5;
    [SerializeField] private GameObject coinPrefab;

    [SerializeField] private int minMeleeDamage;
    [SerializeField] private int maxMeleeDamage;
    [SerializeField] private float meleeAttackMod;

    [SerializeField] private float rangedAttackMod;
    [SerializeField] private int baseRangeDamage;

    [SerializeField] private int hitKnockbackAmount;
    [SerializeField] private int damageKnockbackAmount;

    [SerializeField] private int maxHealth;

    private float velocityXSmoothing;
    private float velocityYSmoothing;

    public int MinMeleeDamage { get => minMeleeDamage; set => minMeleeDamage = value; }
    public int MaxMeleeDamage { get => maxMeleeDamage; set => maxMeleeDamage = value; }
    public float MeleeAttackMod { get => meleeAttackMod; set => meleeAttackMod = value; }

    public float RangedAttackMod { get => rangedAttackMod; set => rangedAttackMod = value; }
    public int BaseRangeDamage { get => baseRangeDamage; set => baseRangeDamage = value; }

    public int HitKnockbackAmount { get => hitKnockbackAmount; set => hitKnockbackAmount = value; }
    public int knockbackGiven { get => damageKnockbackAmount; set => damageKnockbackAmount = value; }

    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float Health { get; set; }

    
    public float gravity = -3;
    public float accelerationTimeGrounded = 0f;
    public float moveSpeed = 5;
    public bool attaking;


    void Start()
    {
        attackProcessor = new AttackProcessor();
        anim = GetComponent<Animator>();
        Health = MaxHealth;
        controller = GetComponent<Controller_2D>();
    }

    private void Update()
    {
        if (isDead)
            return;
        
        AiStates();
    }

    private void LateUpdate()
    {
        if (isDead)
            return;

        if (attaking)
        {
            velocity.x = 0;
        }
        CalculateVelocity();
        controller.Move(velocity * Time.deltaTime);
        //Debug.DrawRay(transform.position, 10f * Vector2.right * transform.localScale, Color.blue);
    }

    void AiStates()
    {
        if (IsAggro)
        {
            anim.SetBool("isChasing", true);
            anim.SetBool("isPatroling", false);
        }
        else
        {
            anim.SetBool("isChasing", false);
            anim.SetBool("isPatroling", true);
        }
        if (InAttackRange() && controller.collitions.below)
        {
            anim.SetBool("Attack", true);
        }
        else
        {
            anim.SetBool("Attack", false);
        }
    }

    public void CoinSpawner()
    {
        for (int i = 0; i < coinDrop; i++)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }

    public bool InAttackRange()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(new Vector2(transform.position.x + (attackPos.x*transform.localScale.x), transform.position.y + (attackPos.y * transform.localScale.y)), attackRange, PlayerMask);
        return (playerCollider != null);
    }
    
    private IEnumerator Die()
    {
        isDead = true;
        anim.SetLayerWeight(0, 0f);
        anim.SetLayerWeight(1, 0f);
        anim.SetLayerWeight(2, 1f);
        anim.SetBool("isDead", true);
        CoinSpawner();
        gameObject.GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        Destroy(gameObject);
    }
    
    public void ModifyHealth(int amount)
    {
        Health -= amount;
        RaiseOnHitEnemyEvent(Health, maxHealth);
        if (Health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            Aggro();
            SpawnDamagePoints(amount);
            anim.SetTrigger("Hit");
        }
    }

    public override bool CanSeePlayer()
    {
        return Physics2D.Raycast(transform.position, Vector2.right * transform.localScale, 10f, PlayerMask);
    }
    
    public void AddForce(Vector2 force)
    {
        velocity.x = Mathf.SmoothDamp(velocity.x, force.x, ref velocityXSmoothing, 0);
        velocity.y = Mathf.SmoothDamp(velocity.y, force.y, ref velocityYSmoothing, 0);
    }

    public void KnockbackOnHit(int amount, int dirX, int dirY)
    {
        AddForce(new Vector2(dirX * amount, 0));
    }

    public void KnockbackOnDamage(int amount, int dirX, int dirY)
    {
        AddForce(new Vector2(dirX * amount, 0));
    }

    public void CalculateVelocity()
    {
        float targetVelocity = moveSpeed * transform.localScale.x;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocity, ref velocityXSmoothing, accelerationTimeGrounded);
        velocity.y += gravity * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x + (attackPos.x * transform.localScale.x), transform.position.y + (attackPos.y * transform.localScale.y)), attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }
}
