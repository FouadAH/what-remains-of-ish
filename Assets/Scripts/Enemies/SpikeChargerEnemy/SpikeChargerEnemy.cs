using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeChargerEnemy : Entity, IAttacker
{
    public IdleState IdleState { get; private set; }
    public MoveState MoveState { get; private set; }
    public AttackState PlayerDetectedState { get; private set; }
    public ChargeState ChargeState { get; private set; }
    public StunState StunState { get; private set; }
    public DeadState DeadState { get; private set; }

    [SerializeField] private int meleeDamage;
    [SerializeField] private int hitKnockbackAmount;

    public int MeleeDamage { get => meleeDamage; set => meleeDamage = value; }
    public int HitKnockbackAmount { get => hitKnockbackAmount; set => hitKnockbackAmount = value; }

    [Header("States")]
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_MoveState moveStateData;
    [SerializeField] private D_PlayerDetected playerDetectedStateData;
    [SerializeField] private D_ChargeState chargeStateData;
    [SerializeField] private D_StunState stunStateData;
    [SerializeField] private D_DeadState deadStateData;

    [Header("Enemy Settings")]
    public float knockbackModifier = 1.5f;
    public AnimationCurve chargeSpeedCurve;
    public ParticleSystem chargeDustParticles;

    public override void Start()
    {
        base.Start();     

        MoveState = new SpikeCharger_MoveState(this, stateMachine, "move", moveStateData, this);
        IdleState = new SpikeCharger_IdleState(this, stateMachine, "idle", idleStateData, this);
        ChargeState = new SpikeCharger_ChargeState(this, stateMachine, "charge", chargeStateData, this);
        StunState = new SpikeCharger_StunState(this, stateMachine, "stun", stunStateData, this);
        DeadState = new SpikeCharger_DeadState(this, stateMachine, "idle", deadStateData, this);
        PlayerDetectedState = new SpikeCharger_PlayerDetectedState(this, stateMachine, "playerDetected", transform, this);

        stateMachine.Initialize(MoveState);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    [Header("Aggro Settings")]
    [SerializeField] public float AggroTime;
    private IEnumerator aggroRangeRoutine;
    public bool IsAggro = false;

    public bool CanSeePlayer()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, GameManager.instance.playerCurrentPosition.position, entityData.whatIsGround);
        return !hit;
    }

    public override void ProcessHit(int amount, DamageType type)
    {
        base.ProcessHit(amount, DamageType.Melee);

        IsAggro = true;

        if (isDead && stateMachine.currentState != DeadState)
        {
            stateMachine.ChangeState(DeadState);
        }
        else if (isStunned && stateMachine.currentState != StunState)
        {
            stateMachine.ChangeState(StunState);
        }
    }

    public override void LoadDefaultData()
    {
        base.LoadDefaultData();

        MoveState = new SpikeCharger_MoveState(this, stateMachine, "move", moveStateData, this);
        IdleState = new SpikeCharger_IdleState(this, stateMachine, "idle", idleStateData, this);
        ChargeState = new SpikeCharger_ChargeState(this, stateMachine, "charge", chargeStateData, this);
        StunState = new SpikeCharger_StunState(this, stateMachine, "stun", stunStateData, this);
        DeadState = new SpikeCharger_DeadState(this, stateMachine, "idle", deadStateData, this);
        PlayerDetectedState = new SpikeCharger_PlayerDetectedState(this, stateMachine, "playerDetected", transform, this);

        stateMachine.Initialize(MoveState);
    }

    public void KnockbackOnHit(int amount, float dirX, float dirY)
    {
        //Debug.Log("KnockbackOnHit");
    }

    public override void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        if (stateMachine.currentState == ChargeState)
        {
            DamageHop(entityData.damageHopSpeed * dirX * knockbackModifier);
        }
        else
        {
            base.KnockbackOnDamage(amount, dirX, dirY);
        }
    }
}
