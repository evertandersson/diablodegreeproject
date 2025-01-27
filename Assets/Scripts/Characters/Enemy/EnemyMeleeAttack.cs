using UnityEngine;

namespace Game
{
    public class EnemyMeleeAttack : EnemyAttack
    {
        public void DealDamage()
        {
            if (IsCloseToPlayer(enemy.distanceToAttack + 0.5f) && IsTargetedAtPlayer())
            {
                StatsCalculator.CalculateDamage(enemy.Damage, PlayerManager.Instance);
            }
        }
    }
}
