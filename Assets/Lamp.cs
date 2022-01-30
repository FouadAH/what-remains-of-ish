using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour, IDamagable
{
    Rigidbody2D rgb2D;
    SpriteRenderer spriteRenderer;

    public float collitionForce = 2f;

    public float health;
    public int maxHealth;
    public int knockbackForce;

    [Header("Particles")]
    public ParticleSystem spearHitEffect;
    public ParticleSystem hitEffect;
    public ParticleSystem breakEffect;

    float IDamagable.Health { get => health; set => health = value; }
    int IDamagable.MaxHealth { get => maxHealth; set => maxHealth = value; }
    int IDamagable.knockbackGiven { get => knockbackForce; set => knockbackForce = value; }

    private void Start()
    {
        rgb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
       if(collision.GetComponent<Player>())
        {
            int directionX = (collision.transform.position.x > transform.position.x) ? -1 : 1;
            rgb2D.AddForce(new Vector2(collitionForce * directionX, 0), ForceMode2D.Impulse);
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

        Vector3 randomEulerRotation = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
        Quaternion randomQuaternionRotation = Quaternion.Euler(randomEulerRotation.x, randomEulerRotation.y, randomEulerRotation.z);
        ParticleSystem hitEffectInstance = Instantiate(spearHitEffect, transform.position, randomQuaternionRotation);
        hitEffectInstance.Play();

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        //breakEffect.Play();
    }
}
