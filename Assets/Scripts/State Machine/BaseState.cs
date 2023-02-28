using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState
{
    public float startTime { get; protected set; }
    protected FiniteStateMachine stateMachine;
    protected string animBoolName;

    public virtual void Enter()
    {
        startTime = Time.time;
        DoChecks();
    }

    public virtual void Exit(){}

    public virtual void LogicUpdate(){}

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void LatePhysicsUpdate(){}

    public virtual void DoChecks(){}
}
