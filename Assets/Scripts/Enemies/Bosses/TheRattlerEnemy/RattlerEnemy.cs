using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RattlerEnemy : Entity
{
    public IdleState idleState { get; private set; }
    public ChargeState spinAttackState { get; private set; }
    public JumpState jumpState { get; private set; }
    public ShootState spitState { get; private set; }
    public DeadState deadState { get; private set; }

    [Header("States")]
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_ChargeState spinAttackStateData;
    [SerializeField] private D_RangedAttackState spitStateData;
    [SerializeField] private D_JumpState jumpStateData;
    [SerializeField] private D_DeadState deadStateData;

    [Header("Other")]
    public ShockwaveSpawner shockwaveSpawner;
    public ProjectileController projectileController;

    public bool isVisibleOnScreen;

    public override void Start()
    {
        base.Start();

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameManager.instance.player.GetComponent<Collider2D>());

        spitState = new RattlerEnemy_SpitState(this, stateMachine, "spit", null, spitStateData, projectileController, this);
        jumpState = new RattlerEnemy_JumpState(this, stateMachine, "move", jumpStateData, this);
        spinAttackState = new RattlerEnemy_SpinState(this, stateMachine, "spin", spinAttackStateData, this);
        idleState = new RattlerEnemy_IdleState(this, stateMachine, "idle", idleStateData, this);
        deadState = new RattlerEnemy_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(idleState);
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
            DamageHop(entityData.damageHopSpeed * dirX);
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

    private void OnBecameVisible()
    {
        isVisibleOnScreen = true;
    }

    private void OnBecameInvisible()
    {
        isVisibleOnScreen = false;
    }
}
