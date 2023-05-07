using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell : MonoBehaviour, IHittable
{
    public bool isActive;
    public float activeTime = 5f;
    public event Action OnTriggered = delegate { };
    public event Action OnDeactivated = delegate { };

    SpriteRenderer spriteRenderer;
    Color active = Color.green;
    Color unactive = Color.white;

    Rigidbody2D rgb2D;
    Collider2D col2D;

    public float collitionForce = 2f;

    [Header("Physics settings")]
    public int knockbackForce;
    [SerializeField] private float upForce = 3f;
    [SerializeField] private float sideForce = 3f;

    [Header("Particles")]
    public ParticleSystem hitEffect;

    public event System.Action OnExplode = delegate { };

    private void Start()
    {
        rgb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        col2D = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            int directionX = (collision.transform.position.x > transform.position.x) ? -1 : 1;
            rgb2D.AddForce(new Vector2(collitionForce * directionX, 0), ForceMode2D.Impulse);
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactive Objects/Lamp Hit", GetComponent<Transform>().position);
        }

    }
    void IHittable.ProcessHit(int hitAmount, DamageType type)
    {
        SetActive();

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactive Objects/Lamp Hit", GetComponent<Transform>().position);

        rgb2D.AddForce(new Vector2(knockbackForce, 0), ForceMode2D.Impulse);

    }
    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

    public void SetActive()
    {
        if (!isActive)
        {
            StartCoroutine(ActiveTimer());
        }
    }

    IEnumerator ActiveTimer()
    {
        spriteRenderer.color = active;
        isActive= true;

        OnTriggered?.Invoke();

        yield return new WaitForSeconds(activeTime);

        spriteRenderer.color = unactive;
        isActive = false;

        OnDeactivated?.Invoke();
    }
}
