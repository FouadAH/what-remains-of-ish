using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vase : BreakableObject
{
    public float coinDropChance;
    public float coinDropAmount;

    public GameObject coinPrefab;

    private void OnDisable()
    {
        float random = Random.value*100;
        if (random <= coinDropChance)
        {
            DestroyedCoinSpawner();
        }
    }

    public void DestroyedCoinSpawner()
    {
        for (int i = 0; i < coinDropAmount; i++)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }
}
