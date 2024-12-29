using UnityEngine;

namespace Game
{
    public class EnemyProjectile : Fireball
    {
        protected override void OnTriggerEnter(Collider other)
        {
            if (!hasHit)
            {
                // Ignore the owner and other projectiles
                if (other.gameObject.CompareTag("Projectile") || other.gameObject.GetComponent<RangedEnemy>())
                {
                    return; // Ignore collision with owner and unwanted objects
                }

                if (explosion != null)
                {
                    Debug.Log(other);
                    ObjectPooling.Instance.SpawnFromPool("Explosion", transform.position, Quaternion.identity);
                    ObjectPooling.Instance.DespawnObject(this.gameObject);
                    hasHit = true;
                }
            }
        }
    }
}
