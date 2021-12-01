using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldEnemy_MeleeAttackState : MeleeAttackState, IHitboxResponder
{
    private ShieldEnemy enemy;

    public ShieldEnemy_MeleeAttackState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttack stateData, ShieldEnemy enemy) : base(etity, stateMachine, animBoolName, attackPosition, stateData)
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

    public override void FinishAttack()
    {
        base.FinishAttack();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isAnimationFinished)
        {
            if (isPlayerInMinAgroRange)
            {
                stateMachine.ChangeState(enemy.playerDetectedState);
            }
            else
            {
                stateMachine.ChangeState(enemy.moveState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocity(0);
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack();
        foreach (var hitbox in enemy.hitboxes)
        {
            hitbox.useResponder(this);
            hitbox.startCheckingCollision();
        }
    }

    void IHitboxResponder.collisionedWith(Collider2D collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if (hurtbox != null)
        {
            Vector2 direction = (hurtbox.transform.position - enemy.transform.position).normalized;
            Vector2 knockBackDirection = (direction.x > 0) ? new Vector2(-1, direction.y) : new Vector2(1, direction.y);
            hurtbox.getHitBy(entity.GetComponent<IAttacker>(), (knockBackDirection.x), (knockBackDirection.y));
        }
    }
}
