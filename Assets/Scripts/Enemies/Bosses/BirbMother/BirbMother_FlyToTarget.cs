using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbMother_FlyToTarget : FlyState
{
    BirbMother enemy;
    private Vector2 Ntarget;
    private Vector2 targetPointCenter;
    private float flySpeedX;
    private float flySpeedY;

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

    Coroutine pathRoutine;
    float elapsedTime = 0;

    public BirbMother_FlyToTarget(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_FlyState stateData, BirbMother enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        Transform closestFlyPoint = GetClosestPoint(enemy.transform, enemy.flyToPoints);
        targetPointCenter = new Vector2(closestFlyPoint.position.x, closestFlyPoint.position.y);

        elapsedTime = 0;

        flySpeedX = 0;
        flySpeedY = 0;

        entity.SetVelocityX(flySpeedX);
        entity.SetVelocityY(flySpeedY);

        UpdateTarget();

        pathRoutine = enemy.StartCoroutine(UpdatePath());
    }

    Transform GetClosestPoint(Transform entity, Transform[] points)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = entity.position;
        foreach (Transform potentialTarget in points)
        {
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
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

        int directionX = (enemy.transform.position.x < enemy.playerRuntimeDataSO.playerPosition.x) ? 1 : -1;
        if (entity.facingDirection != directionX)
        {
            entity.Flip();
        }

        if (Vector2.SqrMagnitude(Ntarget - (Vector2)entity.transform.position) <= 0.5f)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        Vector2 directionToTarget = (Ntarget - (Vector2)entity.transform.position).normalized;

        targetSpeedX = directionToTarget.x * stateData.flySpeed * enemy.flyCurveX.Evaluate(elapsedTime);
        targetSpeedY = directionToTarget.y * stateData.flySpeed;

        currentSpeedX = Mathf.SmoothDamp(currentSpeedX, targetSpeedX, ref currentVelocityX, accelerationTimeX);
        currentSpeedY = Mathf.SmoothDamp(currentSpeedY, targetSpeedY, ref currentVelocityY, accelerationTimeY);

        entity.SetVelocityX(currentSpeedX);
        entity.SetVelocityY(currentSpeedY);

        int directionX = (directionToTarget.x > 0) ? 1 : -1;
        if (entity.facingDirection != directionX)
        {
            entity.Flip();
        }

        elapsedTime += Time.deltaTime;

    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / stateData.updateRate);
            UpdateTarget();
        }
    }

    private void UpdateTarget()
    {
        Ntarget = (Random.insideUnitCircle * stateData.radius) + targetPointCenter;
        enemy.TargetPoint.transform.position = new Vector3(Ntarget.x, Ntarget.y);
    }
}
