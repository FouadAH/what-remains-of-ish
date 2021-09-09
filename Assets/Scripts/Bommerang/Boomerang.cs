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

    private void Start()
    {
        gm = GameManager.instance;
        boomerangLauncher = gm.player.GetComponent<Player>().boomerangLauncher;
        rb = GetComponent<Rigidbody2D>();
        //StartCoroutine(BommerangBehaviour());
        wallDetectionObjs = wallDetectionObjects.GetComponentsInChildren<Transform>();
        rgb2D = GetComponent<Rigidbody2D>();
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
        boomerangLauncher.boomerangAirTimeBonus = 0;
        while (!back)
        {
            SetVelocity();
            if (timer == null)
                timer = StartCoroutine(BoomerangTimer());

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

    void SetVelocity()
    {
        if(!isReflecting)
            targetVelocity = transform.up * boomerangLauncher.MoveSpeed;

        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocityXSmoothing, boomerangLauncher.accelerationTime);
    }

    IEnumerator BoomerangTimer()
    {
        yield return new WaitForEndOfFrame();
        back = false;

        yield return new WaitForSeconds(boomerangLauncher.boomerangAirTime);
        yield return new WaitForSeconds(boomerangLauncher.boomerangAirTimeBonus);

        yield return new WaitForEndOfFrame();
        back = true;

        timer = null;
        yield return null;
    }

    private IEnumerator BommerangBehaviour()
    {
        boomerangLauncher.boomerangAirTimeBonus = 0;
        while (!back)
        {
            yield return new WaitForEndOfFrame();
            SetVelocity();

            if(timer == null)
                timer = StartCoroutine(BoomerangTimer());

            yield return null;
        }

        rb.velocity = Vector2.zero;

        if (instantCallback)
        {
            CallbackImediate();
        }
        else
        {
            StartCoroutine(Callback());
        }
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
                boomerangLauncher.boomerangAirTimeBonus += 0.15f;

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
    
    public bool IsInaccesable()
    {
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, 1, boomerangLauncher.hittable);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 1, boomerangLauncher.hittable);

        RaycastHit2D hitR = Physics2D.Raycast(transform.position, Vector2.right, 1, boomerangLauncher.hittable);
        RaycastHit2D hitL = Physics2D.Raycast(transform.position, Vector2.left, 1, boomerangLauncher.hittable);

        return ((hitUp.collider != null) && (hitDown.collider != null)) || ((hitR.collider != null) && (hitL.collider != null));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down);

        foreach (Transform item in wallDetectionObjs)
        {
            Gizmos.DrawLine(item.position, item.position + transform.up * wallDetectionDistance);

        }
    }
}
