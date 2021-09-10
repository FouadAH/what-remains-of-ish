using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    [HideInInspector] public bool doBoost = false;
    [HideInInspector] public Vector2 boostDir = Vector2.zero;

    public bool isTeleporting;
    Vector2 boomerangPos;

    private bool teleportLock;
    private bool canTeleport;

    public float teleportCooldownTimer = 0.6f;
    public float boomerangTeleportDamage = 15f;
    public float teleportDelay = 0.08f;
    public float teleporEffectDuration = 0.8f;

    public ParticleSystem teleportRechargeEffect;
    public TrailRenderer teleportTrail;

    BoomerangLauncher boomerangLauncher;
    PlayerMovement playerMovement;
    Boomerang boomerang;
    Collider2D playerCollider;

    private void Start()
    {
        boomerangLauncher = GetComponentInChildren<BoomerangLauncher>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCollider = GetComponent<Collider2D>();
    }

    public void OnTeleportInput(Transform transformToMove, ref Vector2 velocity, Boomerang boomerangRef)
    {
        boomerang = boomerangRef;
        if (boomerang.IsInaccesable())
            return;

        if (!teleportLock)
        {
            canTeleport = true;
        }

        if (canTeleport)
        {
            canTeleport = false;

            StartCoroutine(TeleportLock(teleportCooldownTimer));

            isTeleporting = true;

            boomerangPos = boomerang.transform.position;
            boomerang.StopBoomerang();
            boostDir = (boomerangPos - (Vector2)boomerangLauncher.transform.position).normalized;

            StartCoroutine(TeleportEffect(teleporEffectDuration));
            StartCoroutine(Teleport(transformToMove, teleportDelay));
        }
    }

    public IEnumerator Teleport(Transform transformToMove, float timer)
    {
        yield return new WaitForSeconds(timer);

        RaycastHit2D[] hits = Physics2D.LinecastAll(boomerangLauncher.transform.position, boomerangPos, GameManager.instance.player.GetComponent<Player>().enemyMask);
        foreach (RaycastHit2D hit in hits)
        {
            IDamagable damagable = hit.collider.GetComponent<IDamagable>();
            if (damagable != null)
            {
                damagable.ModifyHealth(10);
            }
        }

        TeleportPlayer(transformToMove);

        if (boomerang != null)
        {
            isTeleporting = false;
            Destroy(boomerang.gameObject);
            boomerangLauncher.canFire = true;
        }

        StartCoroutine(BoomerangDashBoost(0.1f));
    }

    void TeleportPlayer(Transform transformToMove)
    {
        Vector2 teleportPosition = boomerangPos;
        float playerColliderSizeX = playerCollider.bounds.size.x/2;
        float playerColliderSizeY = playerCollider.bounds.size.y/2;

        RaycastHit2D hitRight = Physics2D.Raycast(boomerangPos, Vector2.right, playerColliderSizeX, boomerangLauncher.hittable);
        RaycastHit2D hitLeft = Physics2D.Raycast(boomerangPos, Vector2.left, playerColliderSizeX, boomerangLauncher.hittable);
        RaycastHit2D hitUp = Physics2D.Raycast(boomerangPos, Vector2.up, playerColliderSizeY, boomerangLauncher.hittable);
        RaycastHit2D hitDown = Physics2D.Raycast(boomerangPos, Vector2.down, playerColliderSizeY, boomerangLauncher.hittable);

        if (hitRight)
        {
            float boomerangDistanceToWallRight = Mathf.Abs(boomerangPos.x - hitRight.point.x);
            if (boomerangDistanceToWallRight < playerColliderSizeX)
            {
                teleportPosition.x -= Mathf.Abs(playerColliderSizeX - boomerangDistanceToWallRight);
            }
        }

        if (hitLeft)
        {
            float boomerangDistanceToWallLeft = Mathf.Abs(boomerangPos.x - hitLeft.point.x);
            if (boomerangDistanceToWallLeft < playerColliderSizeX)
            {
                teleportPosition.x += Mathf.Abs(playerColliderSizeX - boomerangDistanceToWallLeft);
            }
        }

        if (hitUp)
        {
            float boomerangDistanceToWallUp = Mathf.Abs(boomerangPos.y - hitUp.point.y);
            if (boomerangDistanceToWallUp < playerColliderSizeY)
            {
                teleportPosition.y -= Mathf.Abs(playerColliderSizeY - boomerangDistanceToWallUp);
            }
        }

        if (hitDown)
        {
            float boomerangDistanceToWallDown = Mathf.Abs(boomerangPos.y - hitDown.point.y);
            if (boomerangDistanceToWallDown < playerColliderSizeY)
            {
                teleportPosition.y += Mathf.Abs(playerColliderSizeY - boomerangDistanceToWallDown);
                //Debug.Log("Distance To Wall Down: " + boomerangDistanceToWallDown + " Offset by: " + Mathf.Abs(playerColliderSizeY - boomerangDistanceToWallDown));
            }
        }

        transformToMove.position = teleportPosition;

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
        yield return new WaitWhile(() => playerMovement.isAirborne);
        teleportRechargeEffect.Play();
        teleportLock = false;
    }

    public IEnumerator BoomerangDashBoost(float timer)
    {
        doBoost = true;
        yield return new WaitForSeconds(timer);
        doBoost = false;
    }

}