using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingCoinDrop : Savable, IDamagable
{
    Rigidbody2D rgb2D;
    SpriteRenderer spriteRenderer;
    Collider2D col2D;

    float health = 3;
    int maxHealth = 3;

    [Header("Layer settings")]

    public bool isFalling;

    public LayerMask obstacleLayer;
    public LayerMask damagables;

    [Header("Coin Drop settings")]

    public GameObject coinPrefab;
    public Transform coinSpawnOrigin;
    public int coinAmountOnDestroy = 25;
    public int coinAmountOnHit = 5;

    [System.Serializable]
    public struct Data
    {
        public bool isDestroyed;
    }

    [SerializeField]
    private Data coinDropData;


    [Header("Physics settings")]
    public int knockbackForce;
    public float collitionForce = 2f;
    [SerializeField] private float upForce = 3f;
    [SerializeField] private float sideForce = 3f;

    [Header("Hinges")]
    public GameObject hingesParent;
    public BreakableObject fixedHinge;

    [Header("Particles")]
    public ParticleSystem spearHitEffect;
    public ParticleSystem hitEffect;
    public ParticleSystem breakEffect;

    [Header("SFX")]
    [FMODUnity.EventRef] public string hitSFX;
    [FMODUnity.EventRef] public string breakSFX;

    float IDamagable.Health { get => health; set => health = value; }
    int IDamagable.MaxHealth { get => maxHealth; set => maxHealth = value; }
    int IDamagable.knockbackGiven { get => knockbackForce; set => knockbackForce = value; }

    public event System.Action OnExplode = delegate { };

    public override void Start()
    {
        base.Start();
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



    public void DestroyedCoinSpawner()
    {
        for (int i = 0; i < coinAmountOnDestroy; i++)
        {
            Instantiate(coinPrefab, coinSpawnOrigin.position, Quaternion.identity);
        }
    }

    public void HitCoinSpawner()
    {
        coinAmountOnDestroy -= coinAmountOnHit;
        for (int i = 0; i < coinAmountOnHit; i++)
        {
            Instantiate(coinPrefab, coinSpawnOrigin.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            int directionX = (collision.transform.position.x > transform.position.x) ? -1 : 1;
            rgb2D.AddForce(new Vector2(collitionForce * directionX, 0), ForceMode2D.Impulse);
            FMODUnity.RuntimeManager.PlayOneShot(hitSFX, transform.position);
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

                ParticleSystem breakEffectInstance = Instantiate(breakEffect, transform.position, Quaternion.identity);
                breakEffectInstance.Play();

                breakEffect.Play();
                FMODUnity.RuntimeManager.PlayOneShot(breakSFX, GetComponent<Transform>().position);
                DestroyedCoinSpawner();

                Destroy(gameObject);
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

    void IDamagable.KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        rgb2D.AddForce(new Vector2(knockbackForce * dirX, 0), ForceMode2D.Impulse);
    }

    void IHittable.ProcessHit(int hitAmount, DamageType type)
    {
        health -= 1;
        breakEffect.Play();
        FMODUnity.RuntimeManager.PlayOneShot(hitSFX, transform.position);

        Vector3 randomEulerRotation = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
        Quaternion randomQuaternionRotation = Quaternion.Euler(randomEulerRotation.x, randomEulerRotation.y, randomEulerRotation.z);
        ParticleSystem hitEffectInstance = Instantiate(spearHitEffect, transform.position, randomQuaternionRotation);
        hitEffectInstance.Play();

        HitCoinSpawner();

        if (health <= 0)
        {
            ParticleSystem breakEffectInstance = Instantiate(breakEffect, transform.position, Quaternion.identity);
            breakEffectInstance.Play();
            DestroyedCoinSpawner();
            gameObject.SetActive(false);
            FMODUnity.RuntimeManager.PlayOneShot(breakSFX, GetComponent<Transform>().position);
        }
    }

    public override string SaveData()
    {
        return JsonUtility.ToJson(coinDropData);
    }

    public override void LoadDefaultData()
    {
        coinDropData.isDestroyed = false;
        gameObject.SetActive(true);
    }

    public override void LoadData(string data, string version)
    {
        coinDropData = JsonUtility.FromJson<Data>(data);
        gameObject.SetActive(!coinDropData.isDestroyed);
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

    public void ProcessStunDamage(int amount, float stunDamageMod = 1)
    {
        //throw new System.NotImplementedException();
    }
}
