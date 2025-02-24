using UnityEngine;

namespace Game
{
    public class Fireball : MonoBehaviour, IPooledObject, IPausable
    {
        [SerializeField]
        protected float moveSpeed = 10;

        public string explosionToSpawn;

        private ParticleSystem ParticleSystem;

        protected bool hasHit;

        [SerializeField]
        protected float lifeTime = 5.0f;
        protected float timer;

        private void OnEnable()
        {
            Popup.Pause += Pause;
            Popup.UnPause += UnPause;
        }
        private void OnDisable()
        {
            Popup.Pause -= Pause;
            Popup.UnPause -= UnPause;
        }

        private void Awake()
        {
            ParticleSystem = GetComponentInChildren<ParticleSystem>();
        }

        public virtual void OnObjectSpawn()
        {
            hasHit = false;
            timer = 0;
        }

        protected virtual void FixedUpdate()
        {
            if (GameManager.IsPaused) return;

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

        public void Pause()
        {
            if (ParticleSystem != null)
                ParticleSystem.playbackSpeed = 0;
        }

        public void UnPause()
        {
            if (ParticleSystem != null)
                ParticleSystem.playbackSpeed = 1;
        }
    }
}