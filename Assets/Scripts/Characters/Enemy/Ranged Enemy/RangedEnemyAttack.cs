using UnityEngine;

namespace Game
{
    public class RangedEnemyAttack : EnemyAttack
    {
        public GameObject projectilePrefab;

        protected override void HandleAnimationCombo()
        {
            if (IsAnimationPlaying(enemy.damageAnim))
                return;

            if (!enemy.IsAnimationPlayingStrict(enemy.attackAnims[0]))
            {
                if (IsCloseToPlayer(enemy.distanceToAttack + 0.5f) && IsTargetedAtPlayer())
                {
                    enemy.CharacterAnimator.SetTrigger(enemy.attackTrigger); // Trigger next animation
                }
                else
                {
                    isDone = true; // End combo if not close to the player
                }
            }
        }

        public void SpawnProjectile()
        {
            // Calculate direction to the player
            Vector3 direction = (PlayerManager.Instance.transform.position - enemy.transform.position).normalized;

            // Calculate spawn position with an offset
            Vector3 spawnPosition = enemy.transform.position + offset;

            // Instantiate the projectile and set its rotation
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
            EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
            enemyProjectile.SetEnemy(enemy);
        }
    }

}


