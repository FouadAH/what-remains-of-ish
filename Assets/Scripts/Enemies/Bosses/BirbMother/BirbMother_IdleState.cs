using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbMother_IdleState : IdleState
{
    private BirbMother enemy;
    private Vector2 Ntarget;
    private Vector2 targetPointCenter;
    private float flySpeed = 2f;
    Coroutine pathRoutine;

    public float currentSpeedX;
    public float currentSpeedY;

    float currentVelocityX;
    float currentVelocityY;

    float accelerationTimeX = 0.08f;
    float accelerationTimeY = 0.08f;

    float accelerationTimeChargeX = 0.08f;
    float accelerationTimeChargeY = 0.08f;

    float deccelerationTimeX = 0.2f;
    float deccelerationTimeY = 0.2f;

    public float targetSpeedX;
    public float targetSpeedY;

    bool playerInCloseDistance;
    float playerCloseDistance = 8f;


    public BirbMother_IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, BirbMother enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        float playerDistance = Mathf.Abs(enemy.transform.position.x - enemy.playerRuntimeDataSO.playerPosition.x);
        playerInCloseDistance = playerDistance <= playerCloseDistance;
    }

    public override void Enter()
    {
        base.Enter();
        playerInCloseDistance = false;
        targetPointCenter = enemy.transform.position;
        entity.SetVelocityX(0);
        entity.SetVelocityY(0);

        pathRoutine = enemy.StartCoroutine(UpdatePath());
    }

    public override void Exit()
    {
        base.Exit();
        enemy.StopCoroutine(pathRoutine);
    }

    public override void LatePhysicsUpdate()
    {
        base.LatePhysicsUpdate();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        float random = Random.Range(0, 100);


        int directionX = (enemy.transform.position.x < enemy.playerRuntimeDataSO.playerPosition.x) ? 1 : -1;
        if (entity.facingDirection != directionX)
        {
            entity.Flip();
        }

        if (!enemy.bossFightHasStarted)
        {
            startTime = Time.time;
            return;
        }

        if (isIdleTimeOver)
        {
            if (playerInCloseDistance)
            {
                if (random <= 25)
                {
                    stateMachine.ChangeState(enemy.groundPoundAttackState);
                }
                else if (random > 25)
                {
                    stateMachine.ChangeState(enemy.wallBounceAttackState);
                }
            }
            else
            {
                if (random <= 35)
                {
                    stateMachine.ChangeState(enemy.groundPoundAttackState);
                }
                else if (random > 35)
                {
                    stateMachine.ChangeState(enemy.chargeToPlayerState);
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Vector2 directionToTarget = ((Vector2)enemy.TargetPoint.transform.position - (Vector2)entity.transform.position).normalized;

        targetSpeedX = directionToTarget.x * flySpeed;
        targetSpeedY = directionToTarget.y * flySpeed;

        currentSpeedX = Mathf.SmoothDamp(currentSpeedX, targetSpeedX, ref currentVelocityX, accelerationTimeX);
        currentSpeedY = Mathf.SmoothDamp(currentSpeedY, targetSpeedY, ref currentVelocityY, accelerationTimeY);

        entity.SetVelocityX(currentSpeedX);
        entity.SetVelocityY(currentSpeedY);
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            UpdateTarget();
        }
    }

    private void UpdateTarget()
    {
        Ntarget = (Random.insideUnitCircle * 2) + targetPointCenter;
        enemy.TargetPoint.transform.position = new Vector3(Ntarget.x, Ntarget.y);
    }
}
