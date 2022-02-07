using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BirdbeeEnemyDummy : Entity
{
    [HideInInspector]public AIDestinationSetter destinationSetter;
    [HideInInspector] public AIPath aIPath;

    [Header("Fly Settings")]
    public GameObject TargetPoint;
    [HideInInspector]public Vector2 startPosition;
    public int knockbackFrames = 5;

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
        destinationSetter = GetComponent<AIDestinationSetter>();
        aIPath = GetComponent<AIPath>();

        flyState = new BirdbeeEnemyDummy_FlyState(this, stateMachine, "move", flyStateData, this);
        deadState = new BirdbeeEnemyDummy_DeadState(this, stateMachine, "dead", deadStateData, this);

        stateMachine.Initialize(flyState);
        GetComponent<EnemyAudio>().PlayEventOnce(birdbeeFlyLoop);
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public override void DamageHop(float velocity){}

    public override void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        Vector3 knockbackForce = new Vector3(entityData.damageHopSpeed * dirX, entityData.damageHopSpeed * dirY, 0);
        StartCoroutine(KnockbackTimer(knockbackForce));
    }

    int frames;

    IEnumerator KnockbackTimer(Vector3 knockbackForce)
    {
        frames = 0;
        aIPath.canMove = false;
        aIPath.canSearch = false;
        yield return new WaitForEndOfFrame();

        rb.velocity = knockbackForce;
        knockbackTimeElapsed = 0;
        while (knockbackTimeElapsed < knockbackTime)
        {
            knockbackTimeElapsed += Time.deltaTime;
            rb.velocity = Vector2.SmoothDamp(rb.velocity, Vector2.zero, ref velocitySmoothing, smoothTime);
            if (CheckGround())
            {
                rb.velocity = Vector2.zero;
            }

            yield return null;
        }
        aIPath.canMove = true;
        aIPath.canSearch = true;
        
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
    }
}
