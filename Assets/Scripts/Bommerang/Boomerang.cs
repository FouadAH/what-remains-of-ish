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

    BoomerangLauncher boomerangLauncher;

    GameManager gm;
    Rigidbody2D rb;

    public bool dashActive;

    private void Start()
    {
        gm = GameManager.instance;
        boomerangLauncher = gm.boomerangLauncher.GetComponent<BoomerangLauncher>();
        rb = GetComponent<Rigidbody2D>();
        startPostion = transform.position;
        StartCoroutine(BommerangBehaviour());
    }

    private void Update()
    {
        boomerangSprite.transform.Rotate(Vector3.forward * (boomerangLauncher.rotateSpeed * Time.deltaTime));
    }

    private IEnumerator BommerangBehaviour()
    {
        while(!back)
        {
            rb.velocity = transform.up * boomerangLauncher.MoveSpeed;
            back = Vector2.Distance(startPostion, transform.position) > boomerangLauncher.distance;
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
        yield return new WaitForSecondsRealtime(boomerangLauncher.boomerangHoverTime);
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
                gm.boomerangLauncher.GetComponent<BoomerangLauncher>().canFire = true;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacles") || collision.gameObject.layer == LayerMask.NameToLayer("Lever"))
        {
            Instantiate(boomerangLauncher.hitEffect, transform.position, Quaternion.identity);

            instantCallback = true;
            back = true;
        }

        if (IsInLayerMask(collision.gameObject.layer, boomerangLauncher.damagable))
        {
            Instantiate(boomerangLauncher.hitEffect, transform.position, Quaternion.identity);
            Vector2 hitPos = transform.position;
            OnRangedHit.Invoke(collision, hitPos);
        }

        if (collision.gameObject.GetComponent<PlayerDash>() && dashActive)
        {
            collision.gameObject.GetComponent<BoomerangDash>().isDashingBoomerang = false;
            GameManager.instance.boomerangLauncher.GetComponent<BoomerangLauncher>().canFire = true;
            Destroy(gameObject);
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
