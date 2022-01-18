using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RattlerEnemy : Entity
{
    public IdleState idleState { get; private set; }
    public AttackState spinStartState { get; private set; }
    public AttackState jumpStartState { get; private set; }
    public ChargeState spinAttackState { get; private set; }
    public JumpState jumpState { get; private set; }
    public ShootState spitStateAttack_1 { get; private set; }
    public ShootState spitStateAttack_2 { get; private set; }


    public DeadState deadState { get; private set; }

    [Header("States")]
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_IdleState idleStateData_Phase2;

    [SerializeField] private D_ChargeState spinAttackStateData;
    [SerializeField] private D_ChargeState spinAttackStateData_Phase2;

    [SerializeField] private D_RangedAttackState spitStateData;
    [SerializeField] private D_JumpState jumpStateData;
    [SerializeField] private D_JumpState jumpStateData_Phase2;

    [SerializeField] private D_DeadState deadStateData;

    [Header("Other")]
    public float phase2HealthThreshold =80f;
    public ShockwaveSpawner shockwaveSpawner;
    public ProjectileController projectileAttack_1;
    public ProjectileController projectileAttack_2;

    [Header("Spin VFXs")]
    public ParticleSystem spinEffect;

    [Header("Jump Attack VFXs")]
    public ParticleSystem jumpDirtVFX;
    public ParticleSystem jumpImpactVFX;
    public ParticleSystem shockwaveVFX;


    public bool isVisibleOnScreen;

    public override void Start()
    {
        base.Start();

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameManager.instance.player.GetComponent<Collider2D>());

        spitStateAttack_1 = new RattlerEnemy_SpitState(this, stateMachine, "spit", null, spitStateData, projectileAttack_1, this);
        spitStateAttack_2 = new RattlerEnemy_SpitState_Attack2(this, stateMachine, "spit_attack2", null, spitStateData, projectileAttack_2, this);

        jumpStartState = new RattlerEnemy_JumpStartState(this, stateMachine, "jump", null, this);
        jumpState = new RattlerEnemy_JumpState(this, stateMachine, "jump", jumpStateData, this);
        spinStartState = new RattlerEnemy_SpinStartState(this, stateMachine, "spin_start", null, this);
        spinAttackState = new RattlerEnemy_SpinState(this, stateMachine, "spin_loop", spinAttackStateData, this);
        idleState = new RattlerEnemy_IdleState(this, stateMachine, "idle", idleStateData, this);
        deadState = new RattlerEnemy_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(idleState);
    }

    public override void Update()
    {
        base.Update();
        if(Health < phase2HealthThreshold)
        {
            idleState = new RattlerEnemy_IdleState(this, stateMachine, "idle", idleStateData_Phase2, this);
            spinAttackState = new RattlerEnemy_SpinState(this, stateMachine, "spin_loop", spinAttackStateData_Phase2, this);
            jumpState = new RattlerEnemy_JumpState(this, stateMachine, "jump", jumpStateData_Phase2, this);

            GetComponent<SpriteRenderer>().color = Color.red;
        }
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

    public override void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        if (stateMachine.currentState == spinAttackState)
        {
            if (Health < phase2HealthThreshold)
            {
                DamageHop(entityData.damageHopSpeed*100 * dirX);
            }
            else
            {
                DamageHop(entityData.damageHopSpeed * dirX);
            }
        }
    }

    public override void ProcessHit(int amount)
    {
        base.ProcessHit(amount);

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

    public void SpitAttack() {
        projectileAttack_1.RaiseOnFireEvent();
    }
    public void SpitAttack2()
    {
        projectileAttack_2.RaiseOnFireEvent();
    }

    private void OnBecameVisible()
    {
        isVisibleOnScreen = true;
    }

    private void OnBecameInvisible()
    {
        isVisibleOnScreen = false;
    }
}
