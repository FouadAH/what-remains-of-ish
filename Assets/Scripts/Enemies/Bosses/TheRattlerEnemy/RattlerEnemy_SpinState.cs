using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RattlerEnemy_SpinState : ChargeState
{
    private RattlerEnemy enemy;
    int directionX;

    public RattlerEnemy_SpinState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_ChargeState stateData, RattlerEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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
        enemy.spinEffect.Play();
        enemy.GetComponent<EnemyAudio>().PlayEventOnce(enemy.spinLoop);
    }

    public override void Exit()
    {
        base.Exit();
        enemy.transform.rotation = Quaternion.identity;
        enemy.spinEffect.Stop();
        enemy.GetComponent<EnemyAudio>().StopPlayingEvent();
    }

    public override void LatePhysicsUpdate()
    {
        base.LatePhysicsUpdate();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();


        if (isChargeTimeOver)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        Vector2 playerPos = enemy.playerRuntimeDataSO.playerPosition;
        directionX = (entity.transform.position.x < playerPos.x) ? 1 : -1;
        entity.SetVelocity(stateData.chargeSpeed * directionX);

        Quaternion currentRotation = enemy.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, 10 * -directionX);
        float speed = 0.1f;

        enemy.transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, Time.time * speed);

        //if (enemy.CheckWall() && !isCollidingWithWall)
        //{
        //    isCollidingWithWall = true;
        //    float wallBounceForce = (enemy.CheckWallFront()) ? -100 : 100;
        //    entity.SetVelocity(wallBounceForce);
        //}
        //else
        //{
        //    isCollidingWithWall = false;
        //    entity.SetVelocity(stateData.chargeSpeed * directionX);
        //}
    }
}
