using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLink : MonoBehaviour
{
    public Lamp lamp;

    [Header("Physics settings")]
    [SerializeField] private float upForce = 3f;
    [SerializeField] private float sideForce = 3f;

    Collider2D col2D;
    Rigidbody2D rb2D;
    HingeJoint2D hingeJoint2D;
    SpriteRenderer spriteRenderer;


    void Start()
    {
        lamp.OnExplode += Lamp_OnExplode;
        col2D = GetComponent<Collider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        hingeJoint2D = GetComponent<HingeJoint2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Lamp_OnExplode()
    {
        Physics2D.IgnoreCollision(col2D, GameManager.instance.player.GetComponent<Collider2D>());
        col2D.enabled = true;
        hingeJoint2D.enabled = false;
        float xForce = Random.Range(sideForce, -sideForce);
        float yForce = Random.Range(upForce / 2f, upForce);
        rb2D.AddForce(new Vector2(xForce, yForce), ForceMode2D.Impulse);

    }
}
