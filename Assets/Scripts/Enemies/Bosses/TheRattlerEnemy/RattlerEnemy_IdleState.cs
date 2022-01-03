using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RattlerEnemy_IdleState : IdleState
{
    private RattlerEnemy enemy;
    public RattlerEnemy_IdleState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, RattlerEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
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
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LatePhysicsUpdate()
    {
        base.LatePhysicsUpdate();
    }

    int spitStateConsecutive;
    int spinStateConsecutive;
    int jumpStateConsecutive;

    float spinAttackChance;
    float spinAttackChanceWeight;

    float spitAttackChance;
    float spitAttackChanceWeight;

    float jumpAttackChance;
    float jumpAttackChanceWeight;

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        float random = Random.Range(0, 100);

        if (isIdleTimeOver)
        {
            if (!enemy.isVisibleOnScreen)
            {
                if (random <= 50)
                    stateMachine.ChangeState(enemy.jumpState);
                else if (random > 50)
                    stateMachine.ChangeState(enemy.spinAttackState);
            }
            else
            {
                if (stateMachine.previousState == enemy.spitState)
                {
                    if(random <= 20)
                    {
                        stateMachine.ChangeState(enemy.spitState);
                    }
                    if (random > 20 && random <= 60)
                    {
                        stateMachine.ChangeState(enemy.jumpState);
                    }
                    else if (random > 60)
                    {
                        stateMachine.ChangeState(enemy.spinAttackState);
                    }
                }
                else if (stateMachine.previousState == enemy.jumpState)
                {
                    if (random <= 20)
                    {
                        stateMachine.ChangeState(enemy.jumpState);
                    }
                    if (random > 20 && random <= 60)
                    {
                        stateMachine.ChangeState(enemy.spitState);
                    }
                    else if (random > 60)
                    {
                        stateMachine.ChangeState(enemy.spinAttackState);
                    }
                }
                else if (stateMachine.previousState == enemy.spinAttackState)
                {
                    if (random <= 20)
                    {
                        stateMachine.ChangeState(enemy.spinAttackState);
                    }
                    if (random > 20 && random <= 60)
                    {
                        stateMachine.ChangeState(enemy.jumpState);
                    }
                    else if (random > 60)
                    {
                        stateMachine.ChangeState(enemy.spitState);
                    }
                }
                else
                {
                    if (random <= 33)
                    {
                        stateMachine.ChangeState(enemy.jumpState);
                    }
                    else if (random > 33 && random <= 66)
                    {
                        stateMachine.ChangeState(enemy.spinAttackState);
                    }
                    else if (random >= 100)
                    {
                        stateMachine.ChangeState(enemy.spitState);
                    }
                }
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
