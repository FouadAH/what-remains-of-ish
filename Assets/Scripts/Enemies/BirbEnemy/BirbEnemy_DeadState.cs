using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbEnemy_DeadState : DeadState
{
    private BirbEnemy enemy;

    public BirbEnemy_DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData, BirbEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocity(0);
    }
}
