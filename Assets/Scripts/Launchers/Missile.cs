using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public LayerMask hittable;
    public GameObject bulletEffect;
    public event Action<Collider2D> OnRangedHit = delegate { };

    [SerializeField] Transform Target;
    [SerializeField] float MoveSpeed = 350f;
    [SerializeField] float rotateSpeed = 2000f;
    Rigidbody2D rb;

    public void Start()
    {
        Target = GameManager.instance.player.transform;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    public void Update()
    {
        rb.velocity = transform.up * MoveSpeed;
        float AngleRad = Mathf.Atan2(Target.transform.position.y - transform.position.y, Target.transform.position.x - transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        Quaternion q = Quaternion.AngleAxis(AngleDeg - 90, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotateSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject.layer, hittable))
        {
            OnRangedHit.Invoke(collision);
            Instantiate(bulletEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}