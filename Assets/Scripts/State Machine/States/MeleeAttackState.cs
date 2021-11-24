﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackState : AttackState
{
    protected D_MeleeAttack stateData;

    protected AttackDetails attackDetails;

    public MeleeAttackState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttack stateData) : base(etity, stateMachine, animBoolName, attackPosition)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        base.Enter();

        attackDetails.damageAmount = stateData.attackDamage;
        attackDetails.knockbackAmount = stateData.knockbackAmount;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void FinishAttack()
    {
        base.FinishAttack();
    }

    public override void AnitcipationStart()
    {
        base.AnitcipationStart();
    }
    public override void AttackStart()
    {
        base.AttackStart();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void LatePhysicsUpdate()
    {
        base.LatePhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
    }

}
