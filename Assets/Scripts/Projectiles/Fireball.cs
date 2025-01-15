using UnityEngine;

namespace Game
{
    public class Fireball : MonoBehaviour, IPooledObject
    {
        [SerializeField]
        protected float moveSpeed = 10;

        public string explosionToSpawn;

        protected bool hasHit;

        [SerializeField]
        protected float lifeTime = 5.0f;
        protected float timer;

        public virtual void OnObjectSpawn()
        {
            hasHit = false;
            timer = 0;
        }

        protected virtual void FixedUpdate()
        {
            transform.position += transform.forward * moveSpeed * Time.fixedDeltaTime;

            timer += Time.fixedDeltaTime;
            if (timer >= lifeTime)
            {
                gameObject.SetActive(false);
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (hasHit == false)
            {
                if (!other.gameObject.GetComponent<PlayerManager>()
                    && !other.gameObject.CompareTag("Projectile") &&
                    other.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast"))
                {
                    ObjectPooling.Instance.SpawnFromPool(explosionToSpawn, transform.position, Quaternion.identity);
                    ObjectPooling.Instance.DespawnObject(this.gameObject);
                    hasHit = true;
                }
            }

        }

    }
}