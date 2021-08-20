using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    public Transform boomerangSprite;
    public event Action<Collider2D, Vector2> OnRangedHit = delegate { };

    Vector3 startPostion;
    private bool back;
    private bool instantCallback;
    public Transform wallDetection;
    

    BoomerangLauncher boomerangLauncher;

    GameManager gm;
    Rigidbody2D rb;

    public bool dashActive;

    private void Start()
    {
        gm = GameManager.instance;
        boomerangLauncher = gm.player.GetComponent<Player>().boomerangLauncher;
        rb = GetComponent<Rigidbody2D>();
        startPostion = transform.position;
        StartCoroutine(BommerangBehaviour());
    }

    private void Update()
    {
        boomerangSprite.transform.Rotate(Vector3.forward * (boomerangLauncher.rotateSpeed * Time.deltaTime));
    }

    private void FixedUpdate()
    {
        BounceOffWall();
    }

    private Vector2 velocityXSmoothing;
    public float accelerationTimeGrounded = 0.05f;

    Vector2 targetVelocity;
    bool isReflecting = false;
    void SetVelocity()
    {
        if(!isReflecting)
            targetVelocity = transform.up * boomerangLauncher.MoveSpeed;

        rb.velocity = Vector2.SmoothDamp(rb.velocity, targetVelocity, ref velocityXSmoothing, accelerationTimeGrounded);
    }

    public float boomerangAirTime = 2f;
    public float boomerangAirTimeBonus = 0f;

    Coroutine timer;
    IEnumerator BoomerangTimer()
    {
        yield return new WaitForEndOfFrame();
        back = false;

        yield return new WaitForSeconds(boomerangAirTime);
        yield return new WaitForSeconds(boomerangAirTimeBonus);

        yield return new WaitForEndOfFrame();
        back = true;

        timer = null;
        yield return null;
    }

    private IEnumerator BommerangBehaviour()
    {
        boomerangAirTimeBonus = 0;
        while (!back)
        {
            yield return new WaitForEndOfFrame();
            //rb.velocity = transform.up * boomerangLauncher.MoveSpeed;
            SetVelocity();
            if(timer == null)
                timer = StartCoroutine(BoomerangTimer());
            //back = Vector2.Distance(startPostion, transform.position) > boomerangLauncher.distance;
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
                //Debug.Log("Destroyed boomerang");
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
        RaycastHit2D hit = Physics2D.Raycast((Vector2)wallDetection.position + new Vector2(0.3f, 0.3f), transform.up, 1f, obstacles); 

        if(hit.collider != null && !back)
        {

            Vector2 inDirection = GetComponent<Rigidbody2D>().velocity;
            Vector2 normal = hit.normal;
            Vector2 reflectionVelocity = Vector2.Reflect(inDirection, normal).normalized;

            rb.velocity = Vector2.zero;
            Instantiate(boomerangLauncher.hitEffect, hit.point, Quaternion.identity);
            boomerangAirTimeBonus += 0.15f;
            //isReflecting = true;

            float angle = Mathf.Atan2(reflectionVelocity.y, reflectionVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }

    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        //ContactPoint2D[] contactPoint2D = null;
        //collider.GetContacts(contactPoint2D);/

        //if (collider.gameObject.layer == LayerMask.NameToLayer("Obstacles") || collider.gameObject.layer == LayerMask.NameToLayer("Lever"))
        //{
        //    Instantiate(boomerangLauncher.hitEffect, transform.position, Quaternion.identity);

        //    instantCallback = true;
        //    back = true;
        //}
        if (collider.gameObject.layer == LayerMask.NameToLayer("Lever"))
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

        if (collider.gameObject.GetComponent<PlayerDash>() && dashActive)
        {
            collider.gameObject.GetComponent<BoomerangDash>().isDashingBoomerang = false;
            boomerangLauncher.canFire = true;
            Destroy(gameObject);
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

    public LayerMask obstacles;
    public bool IsAccesable()
    {
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, 1, obstacles);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, Vector2.down, 1, obstacles);

        return (hitUp.collider == null) && (hitDown.collider == null);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down);
        Gizmos.DrawLine(wallDetection.position, transform.position + transform.up * 1f);
    }
}
