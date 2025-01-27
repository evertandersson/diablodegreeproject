using UnityEngine;

public static class StatsCalculator
{
    public static void CalculateDamage(int parentDamage, Character target)
    {
        float damage = parentDamage - target.Defense;
        damage = Mathf.Clamp(damage, 0, Mathf.Infinity);
        target.TakeDamage(Mathf.RoundToInt(damage));
    }
}
