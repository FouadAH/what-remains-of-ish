using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinDrop : MonoBehaviour, IDamagable
{
    public GameObject coinPrefab;
    public int coinAmount = 10;
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
        if (Health == 0)
        {
            CoinSpawner();
            Destroy(gameObject);
        }
    }
    public void CoinSpawner()
    {
        for (int i = 0; i < coinAmount; i++)
        {
            GameObject.Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }
}
