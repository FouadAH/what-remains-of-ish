using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour, IDamagable
{
    Rigidbody2D rgb2D;
    SpriteRenderer spriteRenderer;
    Collider2D col2D;

    public float collitionForce = 2f;

    public float health;
    public int maxHealth;

    public GameObject explosionPrefab;

    [Header("Physics settings")]
    public int knockbackForce;
    [SerializeField] private float upForce = 3f;
    [SerializeField] private float sideForce = 3f;

    [Header("Hinges")]
    public GameObject hingesParent;
    public BreakableObject fixedHinge;

    public bool isFalling;

    public LayerMask obstacleLayer;
    public LayerMask damagables;

    [Header("Particles")]
    public ParticleSystem spearHitEffect;
    public ParticleSystem hitEffect;
    public ParticleSystem breakEffect;

    float IDamagable.Health { get => health; set => health = value; }
    int IDamagable.MaxHealth { get => maxHealth; set => maxHealth = value; }
    int IDamagable.knockbackGiven { get => knockbackForce; set => knockbackForce = value; }

    public event System.Action OnExplode = delegate { };

    private void Start()
    {
        rgb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if(collision.GetComponent<Player>())
        {
            int directionX = (collision.transform.position.x > transform.position.x) ? -1 : 1;
            rgb2D.AddForce(new Vector2(collitionForce * directionX, 0), ForceMode2D.Impulse);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Lamp Hit", GetComponent<Transform>().position);
        }

        if (isFalling) 
        {
            if (IsInLayerMask(collision.gameObject.layer, obstacleLayer))
            {
                isFalling = false;
                OnExplode();

                Physics2D.IgnoreCollision(col2D, GameManager.instance.player.GetComponent<Collider2D>());

                col2D.isTrigger = false;
                rgb2D.gravityScale = 1;
                rgb2D.mass = 2;

                Instantiate(explosionPrefab, new Vector2(transform.position.x, transform.position.y -2), Quaternion.identity);

                ParticleSystem breakEffectInstance = Instantiate(breakEffect, transform.position, Quaternion.identity);
                breakEffectInstance.Play();

                breakEffect.Play();
                Destroy(gameObject);
                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactive Objects/Lamp Break", GetComponent<Transform>().position);
            }
            else if(IsInLayerMask(collision.gameObject.layer, damagables))
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

    void IDamagable.KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        rgb2D.AddForce(new Vector2(knockbackForce * dirX, 0), ForceMode2D.Impulse);
    }

    void IHittable.ProcessHit(int hitAmount)
    {
        health -= hitAmount;
        breakEffect.Play();
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Lamp Hit", GetComponent<Transform>().position);

        Vector3 randomEulerRotation = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
        Quaternion randomQuaternionRotation = Quaternion.Euler(randomEulerRotation.x, randomEulerRotation.y, randomEulerRotation.z);
        ParticleSystem hitEffectInstance = Instantiate(spearHitEffect, transform.position, randomQuaternionRotation);
        hitEffectInstance.Play();

        if (health <= 0)
        {
            ParticleSystem breakEffectInstance = Instantiate(breakEffect, transform.position, Quaternion.identity);
            breakEffectInstance.Play();

            gameObject.SetActive(false);
        }
    }
    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }


    private void OnDisable()
    {
        //breakEffect.Play();
    }

    public void ProcessStunDamage(int amount, float stunDamageMod = 1)
    {
        //throw new System.NotImplementedException();
    }
}
