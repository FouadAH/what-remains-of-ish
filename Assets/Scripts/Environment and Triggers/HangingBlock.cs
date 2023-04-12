using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingBlock : MonoBehaviour
{
    Rigidbody2D rgb2D;
    Collider2D col2D;

    [Header("Hinges")]
    public GameObject hingesParent;
    public BreakableObject fixedHinge;

    public bool isFalling;

    public LayerMask obstacleLayer;
    public LayerMask damagables;

    [Header("Particles")]
    public ParticleSystem spearHitEffect;
    public ParticleSystem hitEffect;
    public ParticleSystem landEffect;

    [Header("SFX")]
    [FMODUnity.EventRef] public string landImpactSFX;

    public event System.Action OnExplode = delegate { };

    private void Start()
    {
        rgb2D = GetComponent<Rigidbody2D>();
        col2D = GetComponent<Collider2D>();
        fixedHinge.OnBreak += FixedHinge_OnBreak;
        foreach (BreakableObject chain in hingesParent.GetComponentsInChildren<BreakableObject>())
        {
            chain.OnBreak += FixedHinge_OnBreak;
        }
    }

    private void FixedHinge_OnBreak()
    {
        isFalling = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isFalling)
        {
            //Detected ground
            if (IsInLayerMask(collision.gameObject.layer, obstacleLayer))
            {
                isFalling = false;

                Physics2D.IgnoreCollision(col2D, GameManager.instance.player.GetComponent<Collider2D>());

                col2D.isTrigger = false;
                rgb2D.bodyType = RigidbodyType2D.Static;

                ParticleSystem landEffectInstance = Instantiate(landEffect, transform.position, Quaternion.identity);
                landEffectInstance.Play();

                landEffect.Play();
                gameObject.layer = 9; 
                FMODUnity.RuntimeManager.PlayOneShot(landImpactSFX, GetComponent<Transform>().position);
            }
            else if (IsInLayerMask(collision.gameObject.layer, damagables))
            {
                Hurtbox hurtbox = collision.gameObject.GetComponent<Hurtbox>();
                int damageDealt = (collision.gameObject.CompareTag("Player")) ? 1 : 10;

                Vector2 dir = Vector2.zero;

                if (hurtbox != null)
                {
                    Vector2 direction = (hurtbox.transform.position - transform.position).normalized;

                    if (direction.x > 0.1)
                        dir.x = -1;
                    else if (direction.x < -0.1)
                        dir.x = 1;

                    dir.y = direction.y;
                }

                hurtbox?.collisionDamage(damageDealt, dir.x, -dir.y);

            }
        }

    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}

