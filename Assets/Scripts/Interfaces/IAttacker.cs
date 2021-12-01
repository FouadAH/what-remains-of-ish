/// <summary>
/// Interface for game objects that have stats (i.e the player, and npcs), extends the damagable interface
/// </summary>
public interface IAttacker : IDamagable
{
    int MeleeDamage { get; set; }
    int HitKnockbackAmount { get; set; }
    void KnockbackOnHit(int amount, float dirX, float dirY);
}
