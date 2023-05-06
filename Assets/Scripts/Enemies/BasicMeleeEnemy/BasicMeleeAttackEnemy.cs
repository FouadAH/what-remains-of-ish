using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeAttackEnemy : Entity, IAttacker
{
    public IdleState idleState { get; private set; }
    public MoveState moveState { get; private set; }
    public PlayerDetectedState playerDetectedState { get; private set; }
    public LookForPlayerState lookForPlayerState { get; private set; }
    public MeleeAttackState meleeAttackState { get; private set; }
    public StunState stunState { get; private set; }
    public DeadState deadState { get; private set; }
    public GameObject stunnedSprite;
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

    //hitboxes
    public List<Hitbox> hitboxes;

    public override void Start()
    {
        base.Start();

        moveState = new BasicMeleeEnemy_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new BasicMeleeEnemy_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new BasicMeleeEnemy_PlayerDetectedState(this, stateMachine, "move", playerDetectedData, this);
        lookForPlayerState = new BasicMeleeEnemy_LookForPlayerState(this, stateMachine, "idle", lookForPlayerStateData, this);
        meleeAttackState = new BasicMeleeEnemy_MeleeAttackState(this, stateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData, this);
        stunState = new BasicMeleeEnemy_StunState(this, stateMachine, "idle", stunStateData, this);
        deadState = new BasicMeleeEnemy_DeadState(this, stateMachine, "idle", deadStateData, this);

        stateMachine.Initialize(moveState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.white;
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    [Header("Aggro Settings")]
    [SerializeField] public float AggroTime;
    private IEnumerator aggroRangeRoutine;

    public bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, playerRuntimeDataSO.playerPosition, entityData.whatIsGround);
        return !hit;
    }

    public override void ProcessHit(int amount, DamageType type)
    {
        base.ProcessHit(amount, type);

        IsAggro = true;

        if (isDead && stateMachine.currentState != deadState)
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
        //Debug.Log("KnockbackOnHit");
    }

    public override void LoadDefaultData()
    {
        base.LoadDefaultData();
        IsAggro = false;

        moveState = new BasicMeleeEnemy_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new BasicMeleeEnemy_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new BasicMeleeEnemy_PlayerDetectedState(this, stateMachine, "move", playerDetectedData, this);
        lookForPlayerState = new BasicMeleeEnemy_LookForPlayerState(this, stateMachine, "idle", lookForPlayerStateData, this);
        meleeAttackState = new BasicMeleeEnemy_MeleeAttackState(this, stateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData, this);
        stunState = new BasicMeleeEnemy_StunState(this, stateMachine, "idle", stunStateData, this);
        deadState = new BasicMeleeEnemy_DeadState(this, stateMachine, "idle", deadStateData, this);

        stateMachine.Initialize(idleState);
    }
}
