using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public Transform boomerangSprite;
    public event Action<Collider2D, Vector2> OnRangedHit = delegate { };

    [Header("Wall Detection Settings")]

    public Transform wallDetectionObjects;

    public float wallDetectionDistance = 1.4f;

    [Header("Bounce Settings")]

    public float collitionAirTimeBonus = 0.2f;
    public float bounceAirTimeBonus = 0.8f;

    public float collitionSpeedBonus = 5f;
    public float bounceSpeedBonus = 10f;

    [Header("Speed Settings")]

    public float maxSpeedBonus = 20f;
    public float maxAirTime = 2.5f;

    float speedBonus = 0;

    private Vector2 velocityXSmoothing;
    Vector2 targetVelocity;

    BoomerangLauncher boomerangLauncher;
    GameManager gm;
    Player player;
    Rigidbody2D rgd2D;
    Collider2D col2D;

    bool isReflecting = false;
    bool back;
    bool instantCallback;

    Coroutine timer;
    Transform[] wallDetectionObjs;
    SpriteRenderer spriteRenderer;
    PlayerMovement playerMovement;

    public AnimationCurve boomerangSpeed;

    [Header("VFX")]

    public ParticleSystem freezeBurst;
    public GameObject explosionPrefab;
    public GameObject freezePrefab;

    [Header("SFX")]

    [FMODUnity.EventRef] public string wallHitSFX;
    [FMODUnity.EventRef] public string enemyHitSFX;
    [FMODUnity.EventRef] public string criticalHitSFX;
    [FMODUnity.EventRef] public string freezeSFX;
    [FMODUnity.EventRef] public string explosionSFX;


    Rumbler rumbler;

    int bounceCount;
    int enemiesDamaged;

    float startTime;
    float boomerangDuration;

    bool isComingBack;
    bool hasExploded;
    bool hasFrozen;
    bool isStopped ;
    bool isInCollider;

    private void Awake()
    {
        player = FindObjectOfType<Player>();

        col2D = GetComponent<Collider2D>();
        rgd2D = GetComponent<Rigidbody2D>();

        Physics2D.IgnoreCollision(col2D, player.GetComponent<Collider2D>());

        playerMovement = player.GetComponent<PlayerMovement>();
        boomerangLauncher = player.GetComponent<Player>().boomerangLauncher;

        wallDetectionObjs = wallDetectionObjects.GetComponentsInChildren<Transform>();

        speedBonus = 0;
        startTime = Time.time;
        rumbler = playerMovement.gameObject.GetComponent<Rumbler>();
    }

    private void Update()
    {
        //spriteRenderer.color = IsAccesable() ? Color.white : Color.red;
        boomerangSprite.transform.Rotate(Vector3.forward * (boomerangLauncher.rotateSpeed * Time.deltaTime));

        if (isComingBack)
            return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (GameManager.instance.playerData.hasBoomerangExplosion)
                InitiateExplosion();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (GameManager.instance.playerData.hasBoomerangFreeze)
                BoomerangFreeze();
        }
    }

    private void FixedUpdate()
    {
        if (isComingBack)
            return;

        BounceOffWall();

        while (!back)
        {
            if (timer == null)
            {
                timer = StartCoroutine(BoomerangCountdown());
            }
            return;
        }

        Callback(); 

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    isComingBack = true;
        //    CallbackImediate();
        //}
    }

    void InitiateExplosion()
    {
        if (!hasExploded)
        {
            hasExploded = true;
            FMODUnity.RuntimeManager.PlayOneShotAttached(explosionSFX, gameObject);
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            HaltBoomerang();
        }
    }

    public void BoomerangFreeze()
    {
        if (!hasFrozen)
        {
            hasFrozen = true;
            FMODUnity.RuntimeManager.PlayOneShotAttached(freezeSFX, gameObject);
            Instantiate(freezePrefab, transform.position, Quaternion.identity);
            HaltBoomerang();
        }
    }

    public void Launch()
    {
        rgd2D.AddForce(transform.up * boomerangLauncher.throwForce, ForceMode2D.Impulse);
    }

    private IEnumerator BoomerangCountdown()
    {
        back = false;
        boomerangDuration = boomerangLauncher.boomerangAirTime;

        float normalizedTime = 0;
        while (normalizedTime < boomerangDuration)
        {
            normalizedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        rgd2D.isKinematic = false;
        rgd2D.angularDrag = boomerangLauncher.angularDrag;
        rgd2D.drag = boomerangLauncher.linearDrag;
        rgd2D.gravityScale = boomerangLauncher.gravityScale;
        col2D.isTrigger = true;

        yield return new WaitForSeconds(boomerangLauncher.boomerangHoverTime);
        back = true;
        timer = null;
    }

    private void Callback()
    {
        if (!isStopped)
        {
            rgd2D.isKinematic = true;
            rgd2D.velocity = Vector2.zero;
            StartCoroutine(Recall());
        }
    }

    private void CallbackImediate()
    {
        GetComponent<Collider2D>().isTrigger = true;

        StopAllCoroutines();
        rgd2D.isKinematic = true;
        rgd2D.velocity = Vector2.zero;

        instantCallback = false;
        StartCoroutine(Recall());
    }

    IEnumerator Recall()
    {
        back = true;
        col2D.isTrigger = true;

        while (true)
        {
            float AngleRad = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x);
            float AngleDeg = (180 / Mathf.PI) * AngleRad;

            Quaternion q = Quaternion.AngleAxis(AngleDeg - 90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * boomerangLauncher.rotateSpeed);
            rgd2D.velocity = transform.up * boomerangLauncher.returnMoveSpeed;

            if (Vector2.Distance(transform.position, player.transform.position) <= 0.8f)
            {
                DestroyBoomerang();
            }

            yield return null;
        }
    }

    public void StopBoomerang()
    {
        if (!isStopped)
        {
            isStopped = true;
            StopAllCoroutines();
            rgd2D.isKinematic = true;
            rgd2D.velocity = Vector2.zero;
        }
    }

    public void DestroyBoomerang()
    {
        boomerangLauncher.canFire = true;
        Destroy(gameObject);
    }

    public void HaltBoomerang()
    {
        rgd2D.isKinematic = false;
        rgd2D.angularDrag = boomerangLauncher.angularDrag;
        rgd2D.drag = boomerangLauncher.linearDrag;
        rgd2D.gravityScale = boomerangLauncher.gravityScale;
        col2D.isTrigger = true;
    }

    void BounceOffWall()
    {
        foreach(Transform wallDetectionObject in wallDetectionObjs)
        {
            RaycastHit2D hit = Physics2D.Raycast(wallDetectionObject.position, transform.up, wallDetectionDistance, boomerangLauncher.hittable);
            Debug.DrawRay(wallDetectionObject.position, transform.up, Color.yellow);

            if (hit.collider != null && !back)
            {
                bounceCount++;
                if (IsInLayerMask(hit.collider.gameObject.layer, boomerangLauncher.damagable))
                {
                    if (enemiesDamaged < 2)
                    {
                        Instantiate(boomerangLauncher.hitEffect, transform.position, Quaternion.identity);
                        FMODUnity.RuntimeManager.PlayOneShotAttached(enemyHitSFX, gameObject);

                        enemiesDamaged++;
                        Vector2 hitPos = transform.position;
                        OnRangedHit.Invoke(hit.collider, hitPos);
                    }
                    else
                    {
                        HaltBoomerang();
                    }
                }

                Vector2 inDirection = rgd2D.velocity;
                Vector2 normal = hit.normal;
                Vector2 reflectionVelocity = Vector2.Reflect(inDirection, normal).normalized;

                rgd2D.velocity = Vector2.zero;
                Instantiate(boomerangLauncher.hitEffect, hit.point, Quaternion.identity);
                FMODUnity.RuntimeManager.PlayOneShotAttached(wallHitSFX, gameObject);

                if (hit.collider.gameObject.tag.ToString().Equals("Bounce"))
                {
                    speedBonus = Mathf.Min(speedBonus + bounceSpeedBonus, maxSpeedBonus);
                    boomerangDuration = Mathf.Min(boomerangDuration + bounceAirTimeBonus, maxAirTime);
                }
                else
                {
                    speedBonus = Mathf.Min(speedBonus + collitionSpeedBonus, maxSpeedBonus);
                    boomerangDuration = Mathf.Min(boomerangDuration + collitionAirTimeBonus, maxAirTime);
                }

                float angle = Mathf.Atan2(reflectionVelocity.y, reflectionVelocity.x) * Mathf.Rad2Deg - 90;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                float bounceForce = ((boomerangLauncher.bounceForce / ( bounceCount * boomerangLauncher.falloutStrenght)) );
                bounceForce = Mathf.Clamp(bounceForce, 0, boomerangLauncher.bounceForce);
                rgd2D.AddForce(transform.up * bounceForce, ForceMode2D.Impulse);
                //rgd2D.AddForce(transform.up * boomerangLauncher.bounceForce, ForceMode2D.Impulse);

                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsInLayerMask(collider.gameObject.layer, boomerangLauncher.interactable))
        {
            Instantiate(boomerangLauncher.hitEffect, transform.position, Quaternion.identity);
            FMODUnity.RuntimeManager.PlayOneShotAttached(wallHitSFX, gameObject);

            instantCallback = true;
            back = true;
        }

        if (IsInLayerMask(collider.gameObject.layer, boomerangLauncher.damagable))
        {
            Instantiate(boomerangLauncher.hitEffect, transform.position, Quaternion.identity);
            FMODUnity.RuntimeManager.PlayOneShotAttached(enemyHitSFX, gameObject);

            enemiesDamaged++;
            Vector2 hitPos = transform.position;
            OnRangedHit.Invoke(collider, hitPos);
        }
    }

    public float wallCheckersDistance = 1.5f;

    public bool IsInaccesable()
    {
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, wallCheckersDistance, boomerangLauncher.hittable);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, wallCheckersDistance, boomerangLauncher.hittable);

        RaycastHit2D hitR = Physics2D.Raycast(transform.position, Vector2.right, wallCheckersDistance, boomerangLauncher.hittable);
        RaycastHit2D hitL = Physics2D.Raycast(transform.position, Vector2.left, wallCheckersDistance, boomerangLauncher.hittable);

        var collider = Physics2D.OverlapPoint(transform.position, boomerangLauncher.hittable);
        bool isNearWall = ((hitUp.collider != null) && (hitDown.collider != null)) || ((hitR.collider != null) && (hitL.collider != null));
        return isNearWall || collider != null;
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * wallCheckersDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * wallCheckersDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckersDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckersDistance);

        Gizmos.color = Color.yellow;

        foreach (Transform item in wallDetectionObjs)
        {
            Gizmos.DrawLine(item.position, item.position + transform.up * wallDetectionDistance);
        }
    }

    private void OnDestroy()
    {
        boomerangLauncher.canFire = true;
    }
}
