using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy : Entity, IAttacker
{
    public IdleState idleState { get; private set; }
    public MoveState moveState { get; private set; }
    public PlayerDetectedState playerDetectedState { get; private set; }
    public LookForPlayerState lookForPlayerState { get; private set; }
    public MeleeAttackState meleeAttackState { get; private set; }
    public BlockState blockState { get; private set; }
    public BlockState blockStateUp { get; private set; }
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
    private D_BlockState blockStateData;
    [SerializeField]
    private D_StunState stunStateData;
    [SerializeField]
    private D_DeadState deadStateData;
    [SerializeField]
    private Transform meleeAttackPosition;

    //hitboxes
    public List<Hitbox> hitboxes;

    [Header("Blocking Settings")]
    public bool isProtected = false;
    public Transform playerCheckUp;
    public Vector2 playerCheckUpSize;
    public Transform playerCheckBehind;
    public Vector2 playerCheckBehindSize;

    [Header("Aggro Settings")]
    [SerializeField] public float AggroTime;
    private IEnumerator aggroRangeRoutine;
    public bool IsAggro = false;

    public event Action Hit = delegate { };

    public override void Start()
    {
        base.Start();

        moveState = new ShieldEnemy_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new ShieldEnemy_IdleState(this, stateMachine, "idle", idleStateData, this);
        playerDetectedState = new ShieldEnemy_PlayedDetected(this, stateMachine, "move", playerDetectedData, this);
        meleeAttackState = new ShieldEnemy_MeleeAttackState(this, stateMachine, "meleeAttack", meleeAttackPosition, meleeAttackStateData, this);
        blockState = new ShieldEnemy_BlockState(this, stateMachine, "blockForward", blockStateData, this);
        blockStateUp = new ShieldEnemy_BlockUpState(this, stateMachine, "blockUp", blockStateData, this);
        stunState = new ShieldEnemy_StunState(this, stateMachine, "stun", stunStateData, this);
        deadState = new ShieldEnemy_DeadState(this, stateMachine, "idle", deadStateData, this);

        stateMachine.Initialize(moveState);
    }

    public bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, GameManager.instance.playerCurrentPosition.position, entityData.whatIsGround);
        return !hit;
    }

    public bool PlayerCheckAbove()
    {
        return Physics2D.OverlapBox(playerCheckUp.position, playerCheckUpSize, playerCheckUp.eulerAngles.z, entityData.whatIsPlayer);
    }

    public bool PlayerCheckBehind()
    {
        return Physics2D.OverlapBox(playerCheckBehind.position, playerCheckBehindSize, playerCheckBehind.eulerAngles.z, entityData.whatIsPlayer);
    }

    public override void DamageHop(float velocity)
    {
        if (isProtected)
            return;

        base.DamageHop(velocity);
    }

    public override void ProcessHit(int amount)
    {
        Hit();
        IsAggro = true;

        if (isProtected)
            return;

        base.ProcessHit(amount);

        if (isDead)
        {
            stateMachine.ChangeState(deadState);
        }
    }

    public override void LoadDefaultData()
    {
        base.LoadDefaultData();
    }

    public void KnockbackOnHit(int amount, float dirX, float dirY)
    {
        //Debug.Log("KnockbackOnHit");
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(playerCheckUp.position, playerCheckUpSize);
        Gizmos.DrawWireCube(playerCheckBehind.position, playerCheckBehindSize);
    }
}
