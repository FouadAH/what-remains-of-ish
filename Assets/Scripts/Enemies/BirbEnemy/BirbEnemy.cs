using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbEnemy : Entity
{
    [Header("Fly Settings")]
    [HideInInspector] public Vector2 startPosition;
    public FlyState flyState { get; private set; }
    public DeadState deadState { get; private set; }

    [Header("States")]
    [SerializeField] private D_FlyState flyStateData;
    [SerializeField] private D_DeadState deadStateData;

    [FMODUnity.EventRef] public string birdbeeFlyLoop;

    public override void Start()
    {
        base.Start();
        startPosition = transform.position;

        flyState = new BirbEnemy_FlyState(this, stateMachine, "move", flyStateData, this);
        deadState = new BirbEnemy_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(flyState);
        GetComponent<EnemyAudio>().PlayEventOnce(birdbeeFlyLoop);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        DamageHop(entityData.damageHopSpeed * dirX);
        //Vector3 knockbackForce = new Vector3(entityData.damageHopSpeed * dirX, entityData.damageHopSpeed * dirY, 0);
        //rb.AddForce(knockbackForce, ForceMode2D.Impulse);
    }

    public override void ProcessHit(int amount)
    {
        base.ProcessHit(amount);
        if (isDead && stateMachine.currentState != deadState)
        {
            GetComponent<EnemyAudio>().StopPlayingEvent();
            stateMachine.ChangeState(deadState);
        }
    }

    public override void LoadDefaultData()
    {
        base.LoadDefaultData();
    }


    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(startPosition, flyStateData.radius);

        Gizmos.DrawLine(transform.position, transform.position + (Vector3)(Vector2.right * facingDirection * entityData.wallCheckDistance));
    }
}
