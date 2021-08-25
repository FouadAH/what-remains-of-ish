using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : MonoBehaviour, IDamagable
{
    public float Health { get; set; }
    public int MaxHealth { get; set; }
    public int knockbackGiven { get; set; }

    private void Start()
    {
        MaxHealth = 3;
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
            Destroy(gameObject);
        }
    }
}
