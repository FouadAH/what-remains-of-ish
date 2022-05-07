using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinDrop : Savable, IDamagable
{
    public GameObject coinPrefab;
    public Transform coinSpawnOrigin;
    public int coinAmountOnDestroy = 10;
    public int coinAmountOnHit = 5;

    public float Health { get; set; }
    public int MaxHealth { get; set; }
    public int knockbackGiven { get; set; }

    public int health;

    [System.Serializable]
    public struct Data
    {
        public bool isDestroyed;
    }

    [SerializeField]
    private Data coinDropData;

    public override void Start()
    {
        base.Start();
        MaxHealth = health;
        Health = MaxHealth;
    }
    private void Update()
    {
        OnDamage();
    }

    public void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        return;
    }

    public void ProcessHit(int amount, DamageType type)
    {
        if (!invinsible)
        {
            iFrames = iFrameTime;
            invinsible = true;
            Debug.Log("process hit");
            Health--;
            if (Health == 0)
            {
                coinDropData.isDestroyed = true;
                DestroyedCoinSpawner();
                gameObject.SetActive(false);
            }
            else
            {
                HitCoinSpawner();
            }
        }
    }


    float iFrames = 0f;
    public float iFrameTime = 3f;
    bool invinsible = false;

    public void OnDamage()
    {
        if (invinsible)
        {
            if (iFrames > 0)
            {
                iFrames -= Time.deltaTime;

            }
            else
            {
                invinsible = false;
            }
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

    public override string SaveData()
    {
        return JsonUtility.ToJson(coinDropData);
    }

    public override void LoadDefaultData()
    {
        coinDropData.isDestroyed = false;
        gameObject.SetActive(true);
    }

    public override void LoadData(string data, string version)
    {
        coinDropData = JsonUtility.FromJson<Data>(data);
        gameObject.SetActive(!coinDropData.isDestroyed);
    }

    public void ProcessStunDamage(int amount, float stunDamageMod = 1)
    {
        //throw new NotImplementedException();
    }
}
