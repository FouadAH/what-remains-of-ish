using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbMother_WallBounceAttackState : ChargeState
{
    private BirbMother enemy;
    Vector2 playerPos;
    Vector2 directionToPlayer;
    public Vector2 movementDirection;

    public float currentSpeedX;
    public float currentSpeedY;

    public float targetSpeedX;
    public float targetSpeedY;

    int bounceTimes = 0;

    public BirbMother_WallBounceAttackState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData, BirbMother enemy) : base(etity, stateMachine, animBoolName, stateData)
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
        isHittingGround = false;
        bounceTimes = 0;

        playerPos = GameManager.instance.playerCurrentPosition.position;
        directionToPlayer =(playerPos - (Vector2)enemy.transform.position).normalized;
        movementDirection = directionToPlayer;

        //targetSpeedX = stateData.chargeSpeed * directionToPlayer.x;
        //targetSpeedY = stateData.chargeSpeed * directionToPlayer.y;

        targetSpeedX = stateData.chargeSpeed * 0.1f * Mathf.Sign(directionToPlayer.x);
        targetSpeedY = stateData.chargeSpeed * -1;

        movementDirection = new Vector2(0.1f * Mathf.Sign(directionToPlayer.x), -1);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LatePhysicsUpdate()
    {
        base.LatePhysicsUpdate();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isChargeTimeOver || bounceTimes >= 5)
        {
            stateMachine.ChangeState(enemy.flyState);
        }

        int directionX = (movementDirection.x > 0) ? 1 : -1;
        if (entity.facingDirection != directionX)
        {
            entity.Flip();
        }
    }

    bool isHittingGround = false;
    public override void PhysicsUpdate()
    {
        RaycastHit2D wallHit = Physics2D.Raycast(enemy.transform.position, movementDirection, 2f, LayerMask.GetMask("Obstacles"));
        Debug.DrawRay(enemy.transform.position, movementDirection * 2, Color.black, 0.1f);

        if (wallHit)
        {
            if (!isHittingGround)
            {
                currentSpeedX = 0;
                currentSpeedY = 0;

                entity.SetVelocityX(0);
                entity.SetVelocityY(0);

                bounceTimes++;
                GameObject.Instantiate(enemy.impactVFX, wallHit.point, Quaternion.identity);
                enemy.ShakeScreen();
            }

            isHittingGround = true;

            Debug.DrawRay(wallHit.point, wallHit.normal, Color.yellow, 10f);

            Vector2 reflectionVector = Vector2.Reflect(movementDirection, wallHit.normal.normalized);

            targetSpeedX = reflectionVector.x * stateData.chargeSpeed;
            targetSpeedY = reflectionVector.y * stateData.chargeSpeed;

            movementDirection = reflectionVector;
            enemy.spriteObj.localScale = new Vector2(2.5f, 1.7f);
        }
        else
        {
            isHittingGround = false;
        }

        currentSpeedX = Mathf.Lerp(currentSpeedX, targetSpeedX, 0.5f);
        currentSpeedY = Mathf.Lerp(currentSpeedY, targetSpeedY, 0.5f);

        entity.SetVelocityX(currentSpeedX);
        entity.SetVelocityY(currentSpeedY);

        //Debug.Log(currentSpeedY);
    }
}
