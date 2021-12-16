using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public Transform boomerangSprite;
    public event Action<Collider2D, Vector2> OnRangedHit = delegate { };

    public Transform wallDetectionObjects;

    public float wallDetectionDistance = 1.4f;

    public float collitionAirTimeBonus = 0.2f;
    public float bounceAirTimeBonus = 0.8f;

    public float collitionSpeedBonus = 5f;
    public float bounceSpeedBonus = 10f;

    public float maxSpeedBonus = 20f;
    public float maxAirTime = 2.5f;

    float speedBonus = 0;

    private Vector2 velocityXSmoothing;
    Vector2 targetVelocity;

    BoomerangLauncher boomerangLauncher;
    GameManager gm;
    Rigidbody2D rb;

    [HideInInspector] 
    bool isReflecting = false;
    bool back;
    bool instantCallback;

    Coroutine timer;
    Transform[] wallDetectionObjs;
    Rigidbody2D rgb2D;
    SpriteRenderer spriteRenderer;
    PlayerMovement playerMovement;

    private void Awake()
    {
        gm = GameManager.instance;
        playerMovement = gm.player.GetComponent<PlayerMovement>();
        boomerangLauncher = gm.player.GetComponent<Player>().boomerangLauncher;
        rb = GetComponent<Rigidbody2D>();
        //StartCoroutine(BommerangBehaviour());
        wallDetectionObjs = wallDetectionObjects.GetComponentsInChildren<Transform>();
        rgb2D = GetComponent<Rigidbody2D>();

        speedBonus = 0;
        //spriteRenderer = boomerangSprite.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        //spriteRenderer.color = IsAccesable() ? Color.white : Color.red;
        boomerangSprite.transform.Rotate(Vector3.forward * (boomerangLauncher.rotateSpeed * Time.deltaTime));
    }

    bool isComingBack = false;
    private void FixedUpdate()
    {

        if (isComingBack)
            return;

        BounceOffWall();
        while (!back)
        {
            SetVelocity();
            if (timer == null)
                timer = StartCoroutine(BoomerangCountdown());

            return;
        }

        rb.velocity = Vector2.zero;

        if (instantCallback)
        {
            isComingBack = true;
            CallbackImediate();
        }
        else
        {
            isComingBack = true;
            StartCoroutine(Callback());
        }

        
    }

    bool firstTake = true;
    void SetVelocity()
    {
        if (!isReflecting)
        {
            float targetSpeedX = boomerangLauncher.MoveSpeed + speedBonus + Mathf.Abs(playerMovement.Velocity.x);
            float targetSpeedY = boomerangLauncher.MoveSpeed;

            targetVelocity = transform.up * new Vector2(targetSpeedX, targetSpeedY);
        }

        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity , ref velocityXSmoothing, boomerangLauncher.accelerationTime);
    }

    float boomerangDuration;
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

        rb.velocity = Vector2.zero;
        back = true;
        timer = null;
    }

    private IEnumerator Callback()
    {
        rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(boomerangLauncher.boomerangHoverTime);
        StartCoroutine(Recall());
    }

    private void CallbackImediate()
    {
        rb.velocity = Vector2.zero;
        instantCallback = false;
        StartCoroutine(Recall());
    }

    IEnumerator Recall()
    {
        while (true)
        {
            float AngleRad = Mathf.Atan2(gm.player.transform.position.y - transform.position.y, gm.player.transform.position.x - transform.position.x);
            float AngleDeg = (180 / Mathf.PI) * AngleRad;

            Quaternion q = Quaternion.AngleAxis(AngleDeg - 90, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * boomerangLauncher.rotateSpeed);
            rb.velocity = transform.up * boomerangLauncher.MoveSpeed;
            if (Vector2.Distance(transform.position, gm.player.transform.position) <= 0.8f)
            {
                boomerangLauncher.canFire = true;
                Destroy(gameObject);
            }
            yield return null;
        }
    }

    public void StopBoomerang()
    {
        StopAllCoroutines();
        rb.velocity = Vector2.zero;
    }

    void BounceOffWall()
    {
        foreach(Transform wallDetectionObject in wallDetectionObjs)
        {
            RaycastHit2D hit = Physics2D.Raycast(wallDetectionObject.position, transform.up, wallDetectionDistance, boomerangLauncher.hittable);
            Debug.DrawRay(wallDetectionObject.position, transform.up, Color.yellow);

            if (hit.collider != null && !back)
            {
                if (IsInLayerMask(hit.collider.gameObject.layer, boomerangLauncher.damagable)) 
                {
                    Instantiate(boomerangLauncher.hitEffect, transform.position, Quaternion.identity);
                    Vector2 hitPos = transform.position;
                    OnRangedHit.Invoke(hit.collider, hitPos);
                }

                Vector2 inDirection = rgb2D.velocity;
                Vector2 normal = hit.normal;
                Vector2 reflectionVelocity = Vector2.Reflect(inDirection, normal).normalized;

                rb.velocity = Vector2.zero;
                Instantiate(boomerangLauncher.hitEffect, hit.point, Quaternion.identity);
                
                if(hit.collider.gameObject.tag.ToString().Equals("Bounce"))
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
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsInLayerMask(collider.gameObject.layer, boomerangLauncher.interactable))
        {
            Instantiate(boomerangLauncher.hitEffect, transform.position, Quaternion.identity);
            instantCallback = true;
            back = true;
        }

        if (IsInLayerMask(collider.gameObject.layer, boomerangLauncher.damagable))
        {
            Instantiate(boomerangLauncher.hitEffect, transform.position, Quaternion.identity);
            Vector2 hitPos = transform.position;
            OnRangedHit.Invoke(collider, hitPos);
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
    public float wallCheckersDistance = 1.5f;
    public bool IsInaccesable()
    {
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, wallCheckersDistance, boomerangLauncher.hittable);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, wallCheckersDistance, boomerangLauncher.hittable);

        RaycastHit2D hitR = Physics2D.Raycast(transform.position, Vector2.right, wallCheckersDistance, boomerangLauncher.hittable);
        RaycastHit2D hitL = Physics2D.Raycast(transform.position, Vector2.left, wallCheckersDistance, boomerangLauncher.hittable);

        return ((hitUp.collider != null) && (hitDown.collider != null)) || ((hitR.collider != null) && (hitL.collider != null));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * wallCheckersDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * wallCheckersDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.left * wallCheckersDistance);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * wallCheckersDistance);

        foreach (Transform item in wallDetectionObjs)
        {
            Gizmos.DrawLine(item.position, item.position + transform.up * wallDetectionDistance);
        }
    }
}
