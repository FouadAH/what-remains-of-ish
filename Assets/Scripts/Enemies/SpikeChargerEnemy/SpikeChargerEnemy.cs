using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeChargerEnemy : Entity, IAttacker
{
    public IdleState idleState { get; private set; }
    public MoveState moveState { get; private set; }
    public AttackState playerDetectedState { get; private set; }
    public ChargeState chargeState { get; private set; }
    public StunState stunState { get; private set; }
    public DeadState deadState { get; private set; }

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

    public override void Start()
    {
        base.Start();

        moveState = new SpikeCharger_MoveState(this, stateMachine, "move", moveStateData, this);
        idleState = new SpikeCharger_IdleState(this, stateMachine, "idle", idleStateData, this);
        //playerDetectedState = new SpikeCharger_PlayerDetectedState(this, stateMachine, "playerDetected", transform, this);
        chargeState = new SpikeCharger_ChargeState(this, stateMachine, "charge",  chargeStateData, this);
        stunState = new SpikeCharger_StunState(this, stateMachine, "stun", stunStateData, this);
        deadState = new SpikeCharger_DeadState(this, stateMachine, "idle", deadStateData, this);

        stateMachine.Initialize(moveState);
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

    public override void ModifyHealth(int amount)
    {
        base.ModifyHealth(amount);

        IsAggro = true;

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
        //Debug.Log("KnockbackOnHit");
    }
}
