using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State
{
    protected D_DeadState stateData;
    public event Action EnemyDeath = delegate { };

    public DeadState(Entity etity, FiniteStateMachine stateMachine, string animBoolName, D_DeadState stateData) : base(etity, stateMachine, animBoolName)
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
        entity.damageBox.gameObject.SetActive(false);
        if (stateData.deathBloodParticle != null)
        {
            GameObject.Instantiate(stateData.deathBloodParticle, entity.aliveGO.transform.position, 
                stateData.deathBloodParticle.transform.rotation);
        }
        //if (stateData.refillParticles != null)
        //{
        //    ParticleSystem refillParticlesInstance = GameObject.Instantiate(stateData.refillParticles, 
        //        entity.transform.position, Quaternion.identity);

        //    refillParticlesInstance.Play();
        //}
        entity.StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        entity.SetVelocity(0);
        entity.anim.SetLayerWeight(0, 0f);
        entity.anim.SetLayerWeight(1, 0f);
        entity.anim.SetLayerWeight(2, 1f);
        entity.anim.SetBool("dead", true);

        //if (stateData.refillParticles != null)
        //{
        //    ParticleSystem refillParticlesInstance = GameObject.Instantiate(stateData.refillParticles, 
        //        entity.transform.position, Quaternion.identity);

        //    refillParticlesInstance.emission.SetBurst(0, new ParticleSystem.Burst(0f, stateData.flaskRefillAmount/2));
        //    refillParticlesInstance.Play();
        //}

        CoinSpawner();
        entity.damageBox.enabled = false;
        entity.gameObject.SetActive(false);
        yield return null;
    }

    public void CoinSpawner()
    {
        for (int i = 0; i < stateData.coinDrop; i++)
        {
            GameObject.Instantiate(stateData.coinPrefab, entity.transform.position, Quaternion.identity);
        }
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
