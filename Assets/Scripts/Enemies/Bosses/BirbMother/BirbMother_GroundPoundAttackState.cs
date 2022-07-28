using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbMother_GroundPoundAttackState : ChargeState
{
    private BirbMother enemy;
    Vector2 movementDirection;

    public float currentSpeedX;
    public float currentSpeedY;

    float targetSpeedX;
    float targetSpeedY;

    int bounceTimes = 0;

    public BirbMother_GroundPoundAttackState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData, BirbMother enemy) : base(etity, stateMachine, animBoolName, stateData)
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

        movementDirection = Vector2.down;

        targetSpeedX = stateData.chargeSpeed * movementDirection.x;
        targetSpeedY = stateData.chargeSpeed * movementDirection.y;
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

        if ( bounceTimes >= 1)
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
        RaycastHit2D wallHit = Physics2D.Raycast(enemy.transform.position, movementDirection, 3f, LayerMask.GetMask("Obstacles"));
        if (wallHit)
        {
            if (!isHittingGround)
            {
                bounceTimes++;
                enemy.ShakeScreen();
                GameObject.Instantiate(enemy.impactVFX, wallHit.point, Quaternion.identity);
            }

            isHittingGround = true;
            enemy.projectileAttack.RaiseOnFireEvent();

            RaycastHit2D ray = Physics2D.Raycast(enemy.transform.position, movementDirection, 1f, LayerMask.GetMask("Obstacles"));
            Debug.DrawRay(ray.point, ray.normal, Color.yellow, 10f);

            Vector2 reflectionVector = Vector2.Reflect(movementDirection, ray.normal.normalized);

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
