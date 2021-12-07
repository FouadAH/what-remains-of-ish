using UnityEngine;

/// <summary>
/// Interface for game objects that are damagable.
/// </summary>
public interface IDamagable //: IHittable
{
    float Health { get; set; }
    int MaxHealth { get; set; }
    void ModifyHealth(int amount);
    int knockbackGiven { get; set; }
    void KnockbackOnDamage(int amount, float dirX, float dirY);
    
}
