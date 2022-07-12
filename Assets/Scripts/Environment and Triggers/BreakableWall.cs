using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour, IHittable
{
    public float Health { get; set; }

    public GameObject wall;
    public GameObject shadow;

    SpriteRenderer wallSprite;
    Collider2D wallCollider;
    Animator shadowAnimator;

    public ParticleSystem wallHitParticles;

    [Header("Wall Hit Animation Values")]
    public float startDuration = 1f;
    public float endDuration = 1f;
    public float moveDistanceX = 5;

    float endValueX;
    float startValueX;

    private void Start()
    {
        Health = 1;

        shadowAnimator = GetComponentInChildren<Animator>();

        wallCollider = wall.GetComponent<Collider2D>();
        wallSprite = wall.GetComponent<SpriteRenderer>();

        startValueX = transform.position.x;
        endValueX = transform.position.x - moveDistanceX;
    }

    public void ProcessHit(int amount, DamageType type)
    {
        Health--;
        wallHitParticles.Play();
        WallHitSequence();

        if (Health == 0)
        {
            GetComponent<Collider2D>().enabled = false;
            shadowAnimator.Play("SecretRoomReveal");
            wallCollider.enabled = false;
            wallSprite.enabled = false;
        }
    }

    void WallHitSequence()
    {
        if (wall != null)
        {
            Sequence mySequence = DOTween.Sequence();
            mySequence.Append(wall.transform.DOMoveX(endValueX, startDuration));
            mySequence.Append(wall.transform.DOMoveX(startValueX, endDuration));
        }
    }


}
