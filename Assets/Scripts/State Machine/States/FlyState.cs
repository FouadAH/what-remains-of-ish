using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyState : State
{
    protected D_FlyState stateData;

    protected bool isDetectingWall;
    protected bool isDetectingLedge;
    protected bool isPlayerInMinAgroRange;

    public FlyState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_FlyState stateData) : base(etity, stateMachine, animBoolName)
    {
        this.stateData = stateData;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isDetectingLedge = entity.CheckLedge();
        isDetectingWall = entity.CheckWall();
        isPlayerInMinAgroRange = entity.CheckPlayerInMinAgroRange();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

}