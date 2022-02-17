using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbEnemy_FlyState : FlyState
{
    private BirbEnemy enemy;

    public BirbEnemy_FlyState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_FlyState stateData, BirbEnemy enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
        direction = new Vector2(0.5f, 0.5f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    Vector2 direction;
    bool hasCollided;
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        RaycastHit2D hit2D = Physics2D.Raycast(enemy.transform.position, direction, entity.entityData.wallCheckDistance, entity.entityData.whatIsGround);
        
        if (hit2D.collider)
        {
            Vector2 inDirection = direction;
            Vector2 normal = hit2D.normal;
            direction = Vector2.Reflect(inDirection, normal).normalized;

            if (Mathf.Sign(direction.x) != enemy.facingDirection)
            {
                enemy.Flip();
            }

        }

        Debug.DrawLine(enemy.transform.position, enemy.transform.position + (Vector3)(direction * entity.entityData.wallCheckDistance), Color.cyan);

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocityX(direction.x * stateData.flySpeed);
        entity.SetVelocityY(direction.y * stateData.flySpeed);
    }

    

}
