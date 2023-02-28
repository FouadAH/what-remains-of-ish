using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : BaseState
{
    protected Entity entity;

    public State(Entity etity, FiniteStateMachine stateMachine, string animBoolName)
    {
        this.entity = etity;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public override void Enter()
    {
        base.Enter();
        entity.anim.SetBool(animBoolName, true);
    }
}
