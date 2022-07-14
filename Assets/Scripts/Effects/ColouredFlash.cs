using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColouredFlash : MonoBehaviour
{
    [Tooltip("Material to switch to during the flash.")]
    [SerializeField] private Material flashMaterial;

    [Tooltip("Duration of the flash.")]
    [SerializeField] private float duration;

    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    private Color originalColor = Color.white;

    private Coroutine flashRoutine;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        originalMaterial = spriteRenderer.material;
        originalColor = spriteRenderer.color;
        flashMaterial = new Material(flashMaterial);
    }

    public void Flash(Color color)
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashRoutine(color));
    }

    public void ResetMaterial()
    {
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        if (originalMaterial != null)
        {
            spriteRenderer.material = originalMaterial;
        }

        if (originalColor != null)
        {
            spriteRenderer.color = originalColor;
        }

        flashRoutine = null;
    }

    private IEnumerator FlashRoutine(Color color)
    {
        spriteRenderer.material = flashMaterial;
        flashMaterial.color = color;
        spriteRenderer.color = color;
        yield return new WaitForSeconds(duration);
        spriteRenderer.material = originalMaterial;
        spriteRenderer.color = originalColor;
        flashRoutine = null;
    }

    public IEnumerator FlashMultiple(Color color, float iFrameTime)
    {
        float temp = iFrameTime + Time.time;
        while (Time.time < temp)
        {
            spriteRenderer.material = flashMaterial;
            flashMaterial.color = color;
            yield return new WaitForSeconds(duration);
            spriteRenderer.material = originalMaterial;
            yield return new WaitForSeconds(duration);
        }
    }
}
