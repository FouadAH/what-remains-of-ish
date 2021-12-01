using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdbeeEnemy_AttackState : MeleeAttackState, IHitboxResponder
{
    private BirdbeeEnemy enemy;
    bool isAnticipation;
    bool isAttacking;
    Vector2 anticipationStartPositon;
    Vector2 attackEndPosition;

    public BirdbeeEnemy_AttackState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, Transform attackPosition, D_MeleeAttack stateData, BirdbeeEnemy enemy) : base(etity, stateMachine, animBoolName, attackPosition, stateData)
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
        enemy.aIPath.canMove = false;
        enemy.aIPath.canSearch = false;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.aIPath.canMove = true;
        enemy.aIPath.canSearch = true;
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
            stateMachine.ChangeState(enemy.playerDetectedState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public override void TriggerAttack()
    {
        base.TriggerAttack(); 
    }

    public override void AnitcipationStart()
    {
        base.AnitcipationStart();
        isAnticipation = true;
    }

    public override void AttackStart()
    {
        base.AttackStart();
        isAnticipation = false;
        isAttacking = true;
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
