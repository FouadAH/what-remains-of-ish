using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbMother_ChargeToPlayerState : ChargeState
{
    private BirbMother enemy;
    Vector2 playerPos;
    Vector2 directionToPlayer;
    public Vector2 movementDirection;

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

    int bounceTimes = 0;
    bool firstTake = true;

    public BirbMother_ChargeToPlayerState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData, BirbMother enemy) : base(etity, stateMachine, animBoolName, stateData)
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
        elapsedTime = 0;

        firstTake = true;
        hitGroundFirstTake = true;
        shouldDeccelerate = false;
        hasDetectedGround = false;

        isHittingGround = false;
        bounceTimes = 0;

        playerPos = GameManager.instance.playerCurrentPosition.position;
        directionToPlayer = (playerPos - (Vector2)enemy.transform.position).normalized;
        directionToPlayer = new Vector2(directionToPlayer.x, -1).normalized;
        movementDirection = directionToPlayer;

        accelerationTimeX = accelerationTimeChargeX;
        accelerationTimeY = accelerationTimeChargeY;

        targetSpeedX = stateData.chargeSpeed * directionToPlayer.x;
        targetSpeedY = stateData.chargeSpeed * directionToPlayer.y;

        enemy.impactVFX.transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LatePhysicsUpdate()
    {
        base.LatePhysicsUpdate();
    }

    float timeAfterBounce;
    float halfTimeAfterBounce;

    bool shouldDeccelerate;

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (bounceTimes >= 1)
        {
            if (firstTake)
            {
                firstTake = false;
                halfTimeAfterBounce = Time.time + 0.05f;
                timeAfterBounce = Time.time + stateData.chargeTime;
            }

            if(Time.time >= halfTimeAfterBounce)
            {
                shouldDeccelerate = true;
            }

            if (Time.time >= timeAfterBounce)
            {
                stateMachine.ChangeState(enemy.flyState);
            }
        }

        int directionX = (movementDirection.x > 0) ? 1 : -1;
        if (entity.facingDirection != directionX)
        {
            entity.Flip();
        }
    }

    bool isHittingGround = false;
    bool hasDetectedGround = false;
    bool hitGroundFirstTake = true;
    float elapsedTime = 0;
    float totalTime = 0.2f;

    public override void PhysicsUpdate()
    {
        RaycastHit2D wallHit = Physics2D.Raycast(enemy.transform.position, movementDirection, 1f, LayerMask.GetMask("Obstacles"));
        RaycastHit2D groundHit = Physics2D.Raycast(enemy.transform.position, Vector2.down, 4f, LayerMask.GetMask("Obstacles"));

        if (groundHit && !wallHit && !hasDetectedGround)
        {
            currentSpeedY = 0;
            targetSpeedY = 0;
            targetSpeedX = Mathf.Sign(directionToPlayer.x) * stateData.chargeSpeed;

            movementDirection = new Vector2(Mathf.Sign(directionToPlayer.x), 0);
            entity.SetVelocityY(0);

            hasDetectedGround = true;
        }

        if (wallHit)
        {
            if (!isHittingGround)
            {
                currentSpeedX = 0;
                currentSpeedY = 0;

                entity.SetVelocityX(0);
                entity.SetVelocityY(0);

                bounceTimes++;
                enemy.enemyAudio.PlayEvent(enemy.wallImpactSFX);
                enemy.ShakeScreen();
                GameObject.Instantiate(enemy.impactVFX, wallHit.point, Quaternion.Euler(0, 0, 90));

                if (enemy.hasEnteredPhase2)
                {
                    enemy.projectileAttack.RaiseOnFireEvent();
                }
            }

            isHittingGround = true;

            Debug.DrawRay(wallHit.point, wallHit.normal, Color.yellow, 10f);

            Vector2 reflectionVector = Vector2.Reflect(movementDirection, wallHit.normal.normalized);

            targetSpeedX = reflectionVector.x * stateData.chargeSpeed;
            targetSpeedY = reflectionVector.y * stateData.chargeSpeed;

            movementDirection = reflectionVector;

           
            enemy.spriteObj.localScale = new Vector2(1.7f, 2.5f);
        }
        else
        {
            isHittingGround = false;
        }

        if (shouldDeccelerate)
        {
            accelerationTimeX = deccelerationTimeX;
            accelerationTimeY = deccelerationTimeY;

            targetSpeedX = 0;
            targetSpeedY = 0;
        }

        currentSpeedX = Mathf.SmoothDamp(currentSpeedX, targetSpeedX * enemy.flyCurveX.Evaluate(elapsedTime/totalTime), ref currentVelocityX, accelerationTimeX);
        currentSpeedY = Mathf.SmoothDamp(currentSpeedY, targetSpeedY, ref currentVelocityY, accelerationTimeY);

        entity.SetVelocityX(currentSpeedX);
        entity.SetVelocityY(currentSpeedY);

        elapsedTime += Time.deltaTime;
    }
}
