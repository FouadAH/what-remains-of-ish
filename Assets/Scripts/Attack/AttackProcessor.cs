using UnityEngine;

/// <summary>
/// Class responsible for processing attacks
/// </summary>
public class AttackProcessor
{
    /// <summary>
    /// Method for processing melee attacks
    /// </summary>
    /// <param name="attacker">Game object that is attacking</param>
    /// <param name="target">Game object being attacked</param>
    /// <param name="knockbackDirX">X Knockback direction</param>
    /// <param name="knockbackDirY">Y Knockback direction</param>
    public void ProcessMelee(IBaseStats attacker, IDamagable target, int knockbackDirX, int knockbackDirY)
    {
        int amount = CalculateAttackAmountMelee(attacker);
        ProcessAttack(target, amount);
        ProcessKnockbackOnHit(attacker, knockbackDirX, knockbackDirY);
        ProcessKnockbackOnDamage(target, -knockbackDirX,  -knockbackDirY);
    }

    /// <summary>
    /// Method for processing ranged attacks 
    /// </summary>
    /// <param name="attacker">Game object that is attacking</param>
    /// <param name="target">Game object being attacked</param>
    public void ProcessRanged(ILauncher attacker, IDamagable target)
    {
        int amount = CalculateAttackAmountRanged(attacker);
        ProcessAttack(target, amount);
    }

    /// <summary>
    /// Helper method that determines melee damage amount
    /// </summary>
    /// <param name="attacker">Attacking game object</param>
    /// <returns></returns>
    private int CalculateAttackAmountMelee(IBaseStats attacker)
    {
        return (int)(Random.Range(attacker.MinMeleeDamage, attacker.MaxMeleeDamage + 1) * attacker.MeleeAttackMod);
    }

    /// <summary>
    /// Helper method that determines ranged damage amount
    /// </summary>
    /// <param name="attacker">Attacking game object</param>
    /// <returns></returns>
    private int CalculateAttackAmountRanged(ILauncher attacker)
    {
        return (int)(Random.Range(attacker.MinRangeDamage, attacker.MaxRangeDamage + 1) * attacker.RangedAttackMod);
    }

    /// <summary>
    /// Helper method that damages the target object
    /// </summary>
    /// <param name="target">Damagable target object</param>
    /// <param name="amount">Damage amount</param>
    private void ProcessAttack(IDamagable target, int amount)
    {
        target.ModifyHealth(amount);
    }

    /// <summary>
    /// Helper method that knocks back the attacker
    /// </summary>
    /// <param name="attacker">Attacker object</param>
    /// <param name="knockbackDirX">X Knockback direction</param>
    /// <param name="knockbackDirY">Y Knockback direction</param>
    private void ProcessKnockbackOnHit(IBaseStats attacker, int knockbackDirX, int knockbackDirY)
    {
        attacker.KnockbackOnHit(attacker.HitKnockbackAmount, knockbackDirX, knockbackDirY);
    }

    /// <summary>
    /// Helper method that knocks back the target
    /// </summary>
    /// <param name="attacker">Target object</param>
    /// <param name="knockbackDirX">X Knockback direction</param>
    /// <param name="knockbackDirY">Y Knockback direction</param>
    private void ProcessKnockbackOnDamage(IDamagable target, int knockbackDirX, int knockbackDirY)
    {
        target.KnockbackOnDamage(target.knockbackGiven, knockbackDirX, knockbackDirY);
    }
}
