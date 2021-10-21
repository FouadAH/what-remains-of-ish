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
    public void ProcessMelee(IAttacker attacker, IDamagable target, float knockbackDirX, float knockbackDirY)
    {
        ProcessKnockbackOnHit(attacker, knockbackDirX, knockbackDirY);
        KnockbackOnMeleeDamage(attacker, target, -knockbackDirX, -knockbackDirY);
        ProcessAttack(target, attacker.MeleeDamage);
    }

    public void ProcessCollisionDamage(int damageAmount, IDamagable target, float knockbackDirX, float knockbackDirY)
    {
        ProcessKnockbackOnDamage(target, -knockbackDirX, -knockbackDirY);
        ProcessAttack(target, damageAmount);
    }

    /// <summary>
    /// Method for processing ranged attacks 
    /// </summary>
    /// <param name="attacker">Game object that is attacking</param>
    /// <param name="target">Game object being attacked</param>
    public void ProcessRanged(ILauncher attacker, IDamagable target, float knockbackDirX, float knockbackDirY)
    {
        int amount = CalculateAttackAmountRanged(attacker);
        ProcessKnockbackOnDamage(target, -knockbackDirX, -knockbackDirY);
        ProcessAttack(target, amount);
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
    private void ProcessKnockbackOnHit(IAttacker attacker, float knockbackDirX, float knockbackDirY)
    {
        if(knockbackDirY == 1)
        {
            attacker.KnockbackOnHit(25, knockbackDirX, knockbackDirY);
        }
        else
        {
            attacker.KnockbackOnHit(attacker.HitKnockbackAmount, knockbackDirX, knockbackDirY);
        }
    }

    /// <summary>
    /// Helper method that knocks back the target
    /// </summary>
    /// <param name="attacker">Target object</param>
    /// <param name="knockbackDirX">X Knockback direction</param>
    /// <param name="knockbackDirY">Y Knockback direction</param>
    private void ProcessKnockbackOnDamage(IDamagable target, float knockbackDirX, float knockbackDirY)
    {
        target.KnockbackOnDamage(target.knockbackGiven, knockbackDirX, knockbackDirY);
    }

    private void KnockbackOnMeleeDamage(IAttacker attacker, IDamagable target, float knockbackDirX, float knockbackDirY)
    {
        target.KnockbackOnDamage(attacker.HitKnockbackAmount, knockbackDirX, knockbackDirY);
    }

    private void KnockbackOnRangedDamage(ILauncher attacker, IDamagable target, float knockbackDirX, float knockbackDirY)
    {
        //target.KnockbackOnDamage(attacker., knockbackDirX, knockbackDirY);
    }
}
