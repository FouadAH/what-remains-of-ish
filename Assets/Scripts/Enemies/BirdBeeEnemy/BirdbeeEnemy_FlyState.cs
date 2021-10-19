using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdbeeEnemy_FlyState : FlyState
{
    private BirdbeeEnemy enemy;
    private Vector2 Ntarget;
    private Vector2 center;

    Coroutine pathRoutine;

    public BirdbeeEnemy_FlyState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_FlyState stateData, BirdbeeEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        enemy.aIPath.maxSpeed = stateData.flySpeed;
        pathRoutine = enemy.StartCoroutine(UpdatePath());
        center = enemy.startPosition;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.StopCoroutine(pathRoutine);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (entity.CheckPlayerInMinAggroRadius())
        {
            enemy.StopCoroutine(pathRoutine);
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    IEnumerator UpdatePath()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / stateData.updateRate);
            UpdateTarget();
        }
    }

    private void UpdateTarget()
    {
        enemy.destinationSetter.target = enemy.TargetPoint.transform;
        Ntarget = (UnityEngine.Random.insideUnitCircle * stateData.radius) + center;
        enemy.TargetPoint.transform.position = new Vector3(Ntarget.x, Ntarget.y);
        
        int directionX = (enemy.transform.position.x < enemy.TargetPoint.transform.position.x) ? 1 : -1;
        if (entity.facingDirection != directionX)
        {
            entity.Flip();
        }
    }
}
