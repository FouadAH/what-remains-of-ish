using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbMother : Entity
{
    public IdleState idleState { get; private set; }
    //public AttackState spinStartState { get; private set; }
    //public AttackState jumpInPlaceStartState { get; private set; }
    //public AttackState jumpTowardseStartState { get; private set; }
    public ChargeState downSpinAttackState { get; private set; }

    public ChargeState chargeAttackState { get; private set; }

    public ShootState roarAttackState { get; private set; }


    public DeadState deadState { get; private set; }

    [Header("States")]
    [SerializeField] private D_IdleState idleStateData;

    [SerializeField] private D_ChargeState downSpinAttackStateData;

    [SerializeField] private D_ChargeState chargeAttackStateData;

    [SerializeField] private D_RangedAttackState roarAttackStateData;

    [SerializeField] private D_DeadState deadStateData;

    [Header("Other")]
    public float phase2HealthThreshold = 80f;
    public ProjectileController projectileAttack;

    //[Header("Spin VFXs")]
    //public ParticleSystem spinEffect;
    //public ParticleSystem jumpDirtVFX;
    //public ParticleSystem jumpImpactVFX;
    //public ParticleSystem shockwaveVFX;

    public override void Start()
    {
        base.Start();

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameManager.instance.player.GetComponent<Collider2D>());

        //roarAttackState = new BirbMother_RoarState(this, stateMachine, "spit", null, roarAttackStateData, projectileAttack, this);

        //chargeAttackState = new BirbMother_ChargeAttackState(this, stateMachine, "jump", chargeAttackStateData, this, true);

        //downSpinAttackState = new BirbMother_DownSpinAttackState(this, stateMachine, "spin_loop", downSpinAttackStateData, this);

        idleState = new BirbMother_IdleState(this, stateMachine, "idle", idleStateData, this);
        //deadState = new BirbMother_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(idleState);
    }

    public bool hasEnteredPhase2 = false;

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

    public override void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        DamageHop(entityData.damageHopSpeed * dirX);
    }

    public override void ProcessHit(int amount, DamageType type)
    {
        base.ProcessHit(amount, DamageType.Melee);

        if (isDead && stateMachine.currentState != deadState)
        {
            stateMachine.ChangeState(deadState);
        }
    }

    public override void LoadDefaultData()
    {
        base.LoadDefaultData();
        stateMachine.Initialize(idleState);
    }

    public void RoarAttack()
    {
        projectileAttack.RaiseOnFireEvent();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        GetComponent<EnemyAudio>().StopPlayingEvent();
    }
}
