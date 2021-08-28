using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeAttackEnemy : Entity, IBaseStats
{
    public IdleState idleState { get; private set; }
    public MoveState moveState { get; private set; }
    public PlayerDetectedState playerDetectedState { get; private set; }
    public LookForPlayerState lookForPlayerState { get; private set; }
    public MeleeAttackState meleeAttackState { get; private set; }
    public StunState stunState { get; private set; }
    public DeadState deadState { get; private set; }

    [SerializeField] private int meleeDamage;
    [SerializeField] private int hitKnockbackAmount;
    public int MeleeDamage { get => meleeDamage; set => meleeDamage = value; }
    public int HitKnockbackAmount { get => hitKnockbackAmount; set => hitKnockbackAmount = value; }

    [Header("States")]
    [SerializeField]
    private D_IdleState idleStateData;
    [SerializeField]
    private D_MoveState moveStateData;
    [SerializeField]
    private D_PlayerDetected playerDetectedData;
    [SerializeField]
    private D_LookForPlayer lookForPlayerStateData;
    [SerializeField]
    private D_MeleeAttack meleeAttackStateData;
    [SerializeField]
    private D_StunState stunStateData;
    [SerializeField]
    private D_DeadState deadStateData;

    [SerializeField]
    private Transform meleeAttackPosition;

    public override void Start()
    {
        base.Start();

        moveState = new BasicMeleeEnemy_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new BasicMeleeEnemy_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new BasicMeleeEnemy_PlayerDetectedState(this, stateMachine, "move", playerDetectedData, this);
        lookForPlayerState = new BasicMeleeEnemy_LookForPlayerState(this, stateMachine, "idle", lookForPlayerStateData, this);
        meleeAttackState = new BasicMeleeEnemy_MeleeAttackState(this, stateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData, this);
        stunState = new BasicMeleeEnemy_StunState(this, stateMachine, "stun", stunStateData, this);
        deadState = new BasicMeleeEnemy_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(moveState);

    }

    public float gravity = -1;
    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //if (!CheckGround())
        //{
        //    SetVelocityY(rb.velocity.y + gravity * Time.deltaTime);
        //}
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(meleeAttackPosition.position, meleeAttackStateData.attackRadius);
    }

    [Header("Aggro Settings")]
    [SerializeField] public float AggroTime;
    private IEnumerator aggroRangeRoutine;
    public bool IsAggro = false;

    public virtual IEnumerator AggroRange()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, aggroRange, entityData.whatIsPlayer);
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
    public override void ModifyHealth(int amount)
    {
        base.ModifyHealth(amount);
        Aggro();
        if (isDead)
        {
            stateMachine.ChangeState(deadState);
        }
        else if (isStunned && stateMachine.currentState != stunState)
        {
            stateMachine.ChangeState(stunState);
        }
    }

    public void KnockbackOnHit(int amount, float dirX, float dirY)
    {
        Debug.Log("KnockbackOnHit");
    }
}
