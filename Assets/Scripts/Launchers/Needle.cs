using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needle : MonoBehaviour
{
    public LayerMask hittable;
    public LayerMask obstacles;

    public GameObject bulletEffect;
    public Rigidbody2D rb2D;
    public float fadeDuration;
    public event Action<Collider2D, Vector2> OnRangedHit = delegate { };

    bool canDamage = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(IsInLayerMask(collision.gameObject.layer, hittable))
        {
            if (canDamage)
            {
                Vector2 hitPos = transform.position;
                OnRangedHit.Invoke(collision, hitPos);
            }

            Instantiate(bulletEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if(IsInLayerMask(collision.gameObject.layer, obstacles))
        {
            canDamage = false;
            Instantiate(bulletEffect, transform.position, Quaternion.identity);
            rb2D.velocity = Vector2.zero;
            StartCoroutine(Fade());
        }
    }

    private IEnumerator Fade()
    {
        SpriteRenderer rend = transform.GetComponent<SpriteRenderer>();
        Color initialColor = rend.material.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            rend.material.color = Color.Lerp(initialColor, targetColor, elapsedTime / fadeDuration);
            yield return null;
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }
}
