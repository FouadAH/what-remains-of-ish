using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomerangDash : MonoBehaviour
{
    [HideInInspector] public bool doBoost = false;
    [HideInInspector] public Vector2 boostDir = Vector2.zero;

    public bool isDashingBoomerang;
    public bool boomerangDash;
    Vector2 boomerangPos;

    private bool teleportLock;
    private bool canTeleport;
    public bool airborne;

    public float teleportCooldownTimer = 0.6f;
    public ParticleSystem teleportRechargeEffect;
    public TrailRenderer teleportTrail;

    public void OnTeleportInput(Transform transformToMove, ref Vector2 velocity, Boomerang boomerang)
    {
        if (!boomerang.IsAccesable())
            return;

        if (!teleportLock)
        {
            canTeleport = true;
        }

        if (canTeleport)
        {
            canTeleport = false;

            StartCoroutine(TeleportLock(teleportCooldownTimer));

            boomerangDash = true;
            isDashingBoomerang = true;
            boomerang.dashActive = true;

            boomerangPos = boomerang.transform.position;
            boomerang.StopBoomerang();

            Transform boomerangLauncher = transformToMove.GetComponentInChildren<BoomerangLauncher>().transform;

            Vector2 dir = (boomerangPos - (Vector2)boomerangLauncher.position).normalized;
            boostDir = dir;

            StartCoroutine(TeleportEffect(0.8f));
            StartCoroutine(Teleport(transformToMove, boomerangLauncher, 0.08f));
          
            if(boomerang != null)
            {
                Destroy(boomerang.gameObject);
                GameManager.instance.boomerangLauncher.GetComponent<BoomerangLauncher>().canFire = true;
            }
        }
    }


    public IEnumerator Teleport(Transform transformToMove, Transform boomerangLauncher, float timer)
    {
        yield return new WaitForSeconds(timer);

        RaycastHit2D[] hits = Physics2D.LinecastAll(boomerangLauncher.position, boomerangPos, GameManager.instance.player.GetComponent<Player>().enemyMask);
        foreach (RaycastHit2D hit in hits)
        {
            IDamagable damagable = hit.collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.ModifyHealth(10);
            }
        }

        transformToMove.position = boomerangPos;
        StartCoroutine(BoomerangDashBoost(0.1f));
    }

    public IEnumerator TeleportEffect( float timer)
    {
        teleportTrail.emitting = true;
        yield return new WaitForSeconds(timer);
        teleportTrail.emitting = false;
    }



    public IEnumerator TeleportLock(float teleportCooldownTimer)
    {
        teleportLock = true;
        yield return new WaitForSeconds(teleportCooldownTimer);
        teleportRechargeEffect.Play();
        yield return new WaitWhile(() => airborne);
        teleportLock = false;
    }

    public IEnumerator BoomerangDashBoost(float timer)
    {
        doBoost = true;
        yield return new WaitForSeconds(timer);
        doBoost = false;
    }

}
