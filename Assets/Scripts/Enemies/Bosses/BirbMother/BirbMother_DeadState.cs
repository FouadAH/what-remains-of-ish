using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbMother_DeadState : DeadState
{
    private BirbMother enemy;
    float deathTime = 0.2f;
    public bool timeStopIsActive = false;

    public BirbMother_DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData, BirbMother enemy) : base(etity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void DoChecks()
    {
        base.DoChecks();
    }

    public override void Enter()
    {
        startTime = Time.time;

        entity.SetVelocity(0);
        enemy.GetComponent<EnemyAudio>().StopPlayingEvent();

        entity.damageBox.gameObject.SetActive(false);

        if (stateData.deathBloodParticle != null)
        {
            GameObject.Instantiate(stateData.deathBloodParticle, entity.aliveGO.transform.position,
                stateData.deathBloodParticle.transform.rotation);
        }

        GameObject.Instantiate(enemy.deathEffectsParent, entity.aliveGO.transform.position,
              stateData.deathBloodParticle.transform.rotation);

        CoinSpawner();
        entity.damageBox.enabled = false;
        entity.gameObject.SetActive(false);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        entity.SetVelocity(0);
    }
}
