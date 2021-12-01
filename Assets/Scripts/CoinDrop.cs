using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinDrop : MonoBehaviour, IDamagable
{
    [SerializeField] private string m_ID = Guid.NewGuid().ToString();
    public string ID => m_ID;

    [ContextMenu("Generate new ID")]
    private void RegenerateGUID() => m_ID = Guid.NewGuid().ToString();

    public GameObject coinPrefab;
    public Transform coinSpawnOrigin;
    public int coinAmountOnDestroy = 10;
    public int coinAmountOnHit = 5;

    public float Health { get; set; }
    public int MaxHealth { get; set; }
    public int knockbackGiven { get; set; }

    public int health;
    string key;

    private void Start()
    {
        MaxHealth = health;
        Health = MaxHealth;
        key = "CD_" + ID;
        gameObject.SetActive((PlayerPrefs.GetInt(key, 0) != 1));
    }
    public void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        return;
    }

    public void ModifyHealth(int amount)
    {
        Health--;
        if (Health == 0)
        {
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();

            DestroyedCoinSpawner();
            Destroy(gameObject);
        }
        else
        {
            HitCoinSpawner();
        }
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
        for (int i = 0; i < coinAmountOnHit; i++)
        {
            Instantiate(coinPrefab, coinSpawnOrigin.position, Quaternion.identity);
        }
    }
}
