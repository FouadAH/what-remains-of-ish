using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingRock : MonoBehaviour, IHittable
{
    public GameObject groundCheck;
    public float groundCheckRadius = 0.3f;

    public float castLength = 5f;

    public LayerMask playerMask;
    public LayerMask groundMask;

    public float waitTime = 0.2f;
    public float fallSpeed = 10f;
    public float smoothTime = 10f;

    public ParticleSystem destroyEffect;
    public ParticleSystem fallEffect;


    protected ColouredFlash colouredFlash;

    Vector2 velocityRef;
    bool isFalling;
    bool playerDetected;
    Rigidbody2D rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        colouredFlash = GetComponent<ColouredFlash>();
    }

    private void FixedUpdate()
    {
        RaycastHit2D playerHit = Physics2D.Raycast(transform.position, Vector2.down, castLength, playerMask);
        if (playerHit && !playerDetected)
        {
            playerDetected = true;
            StartCoroutine(FallRoutine());
        }

        if (!isFalling)
        {
            return;
        }

        rb.velocity = Vector2.SmoothDamp(rb.velocity, new Vector2(0, -fallSpeed), ref velocityRef, smoothTime);

        if (CheckGround())
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity).Play();
            gameObject.SetActive(false);
        }

    }

    public virtual bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, groundMask);
    }

    IEnumerator FallRoutine()
    {
        fallEffect.Play();
        yield return new WaitForSeconds(waitTime);
        isFalling = true;
    }

    void IHittable.ProcessHit(int hitAmount, DamageType type)
    {
        if (!isFalling)
            StartCoroutine(FallRoutine());

        if (colouredFlash != null)
            colouredFlash.Flash(Color.white);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - castLength));
        Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadius);

    }
}
