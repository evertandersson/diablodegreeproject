using UnityEngine;

namespace Game
{
    public class RangedEnemyAttack : EnemyAttack
    {
        public GameObject projectilePrefab;

        public void SpawnProjectile()
        {
            // Calculate direction to the player
            Vector3 direction = (PlayerManager.Instance.transform.position - enemy.transform.position).normalized;

            // Calculate spawn position with an offset
            Vector3 spawnPosition = enemy.transform.position + offset;

            // Instantiate the projectile and set its rotation
            GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.LookRotation(direction));
        }
    }

}
