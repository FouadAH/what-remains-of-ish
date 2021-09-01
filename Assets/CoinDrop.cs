using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinDrop : MonoBehaviour, IDamagable
{
    public GameObject coinPrefab;
    public int coinAmountOnDestroy = 10;
    public int coinAmountOnHit = 5;

    public float Health { get; set; }
    public int MaxHealth { get; set; }
    public int knockbackGiven { get; set; }

    public int health;

    private void Start()
    {
        MaxHealth = health;
        Health = MaxHealth;
    }
    public void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        return;
    }

    public void ModifyHealth(int amount)
    {
        Health--;
        HitCoinSpawner();
        if (Health == 0)
        {
            DestroyedCoinSpawner();
            Destroy(gameObject);
        }
    }
    public void DestroyedCoinSpawner()
    {
        for (int i = 0; i < coinAmountOnDestroy; i++)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }

    public void HitCoinSpawner()
    {
        for (int i = 0; i < coinAmountOnHit; i++)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }
}
