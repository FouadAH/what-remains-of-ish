using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbMother : Entity
{
    public IdleState idleState { get; private set; }
    public FlyState flyState { get; private set; }
    public ChargeState groundPoundAttackState { get; private set; }
    public ChargeState wallBounceAttackState { get; private set; }
    public ChargeState chargeToPlayerState { get; private set; }
    public ShootState roarAttackState { get; private set; }
    public DeadState deadState { get; private set; }

    [Header("States")]
    [SerializeField] private D_IdleState idleStateData;
    [SerializeField] private D_FlyState flyStateData;
    [SerializeField] private D_ChargeState downSpinAttackStateData;
    [SerializeField] private D_ChargeState chargeToPlayerStateData;
    [SerializeField] private D_ChargeState wallBounceAttackStateData;
    [SerializeField] private D_RangedAttackState roarAttackStateData;
    [SerializeField] private D_DeadState deadStateData;

    [Header("Other")]
    public float phase2HealthThreshold = 80f;

    public ProjectileController projectileAttack;
    public GameObject TargetPoint;

    public Transform[] flyToPoints;

    public AnimationCurve flyCurveX, flyCurveY;
    public AnimationCurve playerChaseCurveX;

    public Transform spriteObj;
    float spriteScaleXSmoothing;
    float spriteScaleYSmoothing;

    [Header("Impact VFXs")]
    public ParticleSystem impactVFX;
    public GameObject deathEffectsParent;
    public Cinemachine.CinemachineImpulseSource impactInpulseSource;

    public override void Start()
    {
        base.Start();

        spriteObj = GetComponentInChildren<SpriteRenderer>().transform;
        
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), GameManager.instance.player.GetComponent<Collider2D>());

        //roarAttackState = new BirbMother_RoarState(this, stateMachine, "spit", null, roarAttackStateData, projectileAttack, this);
        idleState = new BirbMother_IdleState(this, stateMachine, "idle", idleStateData, this);
        groundPoundAttackState = new BirbMother_GroundPoundAttackState(this, stateMachine, "spin_loop", downSpinAttackStateData, this);
        chargeToPlayerState = new BirbMother_ChargeToPlayerState(this, stateMachine, "charge", chargeToPlayerStateData, this);
        wallBounceAttackState = new BirbMother_WallBounceAttackState(this, stateMachine, "charge", wallBounceAttackStateData, this);
        flyState = new BirbMother_FlyToTarget(this, stateMachine, "fly", flyStateData, this);
        deadState = new BirbMother_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(idleState);
    }

    public bool hasEnteredPhase2 = false;

    public void ShakeScreen()
    {
        impactInpulseSource.GenerateImpulse();
    }

    void SpriteUpdate()
    {
        float spriteScaleX = Mathf.SmoothDamp(spriteObj.localScale.x, 2, ref spriteScaleXSmoothing, 0.15f);
        float spriteScaleY = Mathf.SmoothDamp(spriteObj.localScale.y, 2, ref spriteScaleYSmoothing, 0.15f);

        spriteObj.localScale = new Vector2(spriteScaleX, spriteScaleY);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        SpriteUpdate();
    }

    public override void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        if (stateMachine.currentState == wallBounceAttackState)
        {
            Vector2 directionFromPlayer = (transform.position - GameManager.instance.playerCurrentPosition.position).normalized;

            BirbMother_WallBounceAttackState chargeAttackState = (BirbMother_WallBounceAttackState)stateMachine.currentState;

            chargeAttackState.targetSpeedX = directionFromPlayer.x * wallBounceAttackStateData.chargeSpeed;
            chargeAttackState.targetSpeedY = directionFromPlayer.y * wallBounceAttackStateData.chargeSpeed;

            chargeAttackState.movementDirection = directionFromPlayer;
        }
    }

    public override void ProcessHit(int amount, DamageType type)
    {
        base.ProcessHit(amount, type);

        if(Health <= phase2HealthThreshold)
        {
            hasEnteredPhase2 = true;
        }

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

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.cyan;

        //if(Application.isPlaying)
            //Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + rb.velocity.x, transform.position.y + rb.velocity.y, 0));
    }
}
