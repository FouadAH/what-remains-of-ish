using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RattlerEnemy_JumpState : JumpState
{
    RattlerEnemy enemy;
    bool hasJumped;
    bool jumpTowards;

    public RattlerEnemy_JumpState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_JumpState stateData, RattlerEnemy enemy, bool jumpTowards) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
        this.jumpTowards = jumpTowards;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        hasJumped = false;

        if (jumpTowards)
            JumpAttack();
        else
            JumpInPlaceAttack();
    }

    public override void Exit()
    {
        base.Exit();

        enemy.jumpDirtVFX.Play();
        enemy.jumpImpactVFX.Play();
        enemy.shockwaveVFX.Play();

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
        float directionToPlayer = enemy.playerRuntimeDataSO.playerPosition.x - enemy.transform.position.x;
        enemy.rb.AddForce(new Vector2(directionToPlayer* stateData.speedModifier, stateData.jumpHeight), ForceMode2D.Impulse);
    }

    void JumpInPlaceAttack()
    {
        enemy.rb.AddForce(new Vector2(0, stateData.jumpHeight), ForceMode2D.Impulse);
    }
}
