using UnityEngine;

/// <summary>
/// Interface for game objects that are damagable.
/// </summary>
public interface IDamagable : IHittable
{
    float Health { get; set; }
    int MaxHealth { get; set; }
    int knockbackGiven { get; set; }
    void KnockbackOnDamage(int amount, float dirX, float dirY);
    
}
