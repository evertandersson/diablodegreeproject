using UnityEngine;

namespace Game
{
    public class EnemyProjectile : Fireball
    {
        private Enemy parentEnemy;

        public void SetEnemy(Enemy enemy)
        {
            parentEnemy = enemy;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (!hasHit)
            {
                // Ignore the owner and other projectiles
                if (other.gameObject.CompareTag("Projectile") || 
                    other.gameObject.GetComponent<RangedEnemy>() ||
                    other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast"))
                {
                    return; // Ignore collision with owner and unwanted objects
                }

                Debug.Log(other);
                GameObject explosion = ObjectPooling.Instance.SpawnFromPool(explosionToSpawn, transform.position, Quaternion.identity);
                EnemyExplosion enemyExplosion = explosion.GetComponent<EnemyExplosion>();
                enemyExplosion.SetEnemy(parentEnemy);

                ObjectPooling.Instance.DespawnObject(this.gameObject);
                hasHit = true;  
                
            }
        }
    }
}
