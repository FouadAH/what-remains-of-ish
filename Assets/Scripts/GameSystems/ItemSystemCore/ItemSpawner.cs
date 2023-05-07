using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemSpawner : MonoBehaviour
{
    public GameObject item;
    public Transform targetPosition;
    public float targetScaleMod = 1.5f;
    public float scaleTime = 0.8f;

    public ParticleSystem spawnEffect;

    [ContextMenu("Spawn Item")]
    public void SpawnItem()
    {
        if(targetPosition == null)
        {
            targetPosition = transform;
        }

        item.transform.position = targetPosition.position;
        item.transform.localScale = Vector3.zero;
        item.SetActive(true);
        item.transform.DOScale(Vector3.one * targetScaleMod, scaleTime).SetEase(Ease.InBounce);
        spawnEffect.Play();
    }
}
