using Game;
using UnityEngine;

public static class StatsCalculator
{
    public static int CalculateAbilityDamage(AttackTypeSO attackType)
    {
        int damage = Mathf.RoundToInt(PlayerManager.Instance.Damage * attackType.damageMultiplier);
        return damage;
    }

    public static void CalculateDamage(int parentDamage, Character target)
    {
        float damage = parentDamage - target.Defense;
        damage = Mathf.Clamp(damage, 0, Mathf.Infinity);
        target.TakeDamage(Mathf.RoundToInt(damage));
    }

    public static void CalculateAttackSpeed()
    {

    }
}
