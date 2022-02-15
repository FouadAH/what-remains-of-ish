using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public ParticleSystem explosionParticles;

    private void Start()
    {
        explosionParticles.Play();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.attachedRigidbody.AddExplosionForce(50f, transform.position, 12f);
    }

    public void SeldDestruct()
    {
        Destroy(gameObject);
    }
}
