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
            if (stateMachine.previousState == enemy.spitStateAttack_1)
            {
                if(random <= 10)
                {
                    stateMachine.ChangeState(enemy.spitStateAttack_1);
                }
                if (random > 10 && random <= 60)
                {
                    stateMachine.ChangeState(enemy.spinStartState);
                }
                else if (random > 60)
                {
                    stateMachine.ChangeState(enemy.jumpTowardseStartState);
                }
            }
            else if (stateMachine.previousState == enemy.jumpTowardsState || stateMachine.previousState == enemy.jumpInPlaceState )
            {
                if (enemy.hasEnteredPhase2 && stateMachine.previousState != enemy.jumpInPlaceState)
                {
                    stateMachine.ChangeState(enemy.jumpInPlaceStartState);
                }
                else
                {
                    if (random <= 10)
                    {
                        stateMachine.ChangeState(enemy.jumpTowardseStartState);
                    }
                    if (random > 10 && random <= 60)
                    {
                        stateMachine.ChangeState(enemy.spitStateAttack_1);
                    }
                    else if (random > 60)
                    {
                        stateMachine.ChangeState(enemy.spinStartState);
                    }
                }
            }
            else if (stateMachine.previousState == enemy.spinAttackState)
            {
                if (random <= 10)
                {
                    stateMachine.ChangeState(enemy.spinStartState);
                }
                if (random > 10 && random <= 60)
                {
                    stateMachine.ChangeState(enemy.jumpTowardseStartState);
                }
                else if (random > 60)
                {
                    stateMachine.ChangeState(enemy.spitStateAttack_1);
                }
            }
            else
            {
                if (random <= 33)
                {
                    stateMachine.ChangeState(enemy.jumpTowardseStartState);
                }
                else if (random > 33 && random <= 66)
                {
                    stateMachine.ChangeState(enemy.spinStartState);
                }
                else if (random >= 100)
                {
                    stateMachine.ChangeState(enemy.spitStateAttack_1);
                }
            }
            
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
