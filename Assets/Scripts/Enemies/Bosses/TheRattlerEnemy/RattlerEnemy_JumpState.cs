using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RattlerEnemy_JumpState : JumpState
{
    RattlerEnemy enemy;
    Transform playerPosition;
    bool hasJumped;
    bool isCollidingWithWall;

    public RattlerEnemy_JumpState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_JumpState stateData, RattlerEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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
        hasJumped = false;
        JumpAttack();
    }

    public override void Exit()
    {
        base.Exit();
        enemy.shockwaveSpawner.SpawnShockwave(1);
        enemy.shockwaveSpawner.SpawnShockwave(-1);
    }

    public override void LatePhysicsUpdate()
    {
        base.LatePhysicsUpdate();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (!enemy.CheckGround())
        {
            hasJumped = true;
        }

        if (enemy.CheckGround() && hasJumped)
        {
            stateMachine.ChangeState(enemy.idleState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    void JumpAttack()
    {
        playerPosition = GameManager.instance.player.transform;
        float directionToPlayer = playerPosition.position.x - enemy.transform.position.x;
        enemy.rb.AddForce(new Vector2(directionToPlayer, stateData.jumpHeight), ForceMode2D.Impulse);
    }
}
