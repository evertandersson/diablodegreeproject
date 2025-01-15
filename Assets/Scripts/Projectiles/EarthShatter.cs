using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EarthShatter : Fireball
    {
        public Dictionary<Enemy, int> enemiesHit = new Dictionary<Enemy, int>();

        [SerializeField] private ParticleSystem particle;

        private float damageTimer = 0.3f;
        private float currentTimer = 0f;

        private void Awake()
        {
            particle = GetComponent<ParticleSystem>();
        }

        public override void OnObjectSpawn()
        {
            base.OnObjectSpawn();
            enemiesHit.Clear();
            currentTimer = 0;
        }

        private void Update()
        {
            if (particle != null)
            {
                currentTimer += Time.deltaTime;

                // Deal damage to enemies at intervals
                if (currentTimer > damageTimer)
                {
                    // Create a temporary list of keys to iterate over
                    var enemiesToProcess = new List<Enemy>(enemiesHit.Keys);

                    foreach (var enemy in enemiesToProcess)
                    {
                        if (enemy != null && enemiesHit[enemy] < 2)
                        {
                            enemy.TakeDamage(PlayerManager.Instance.Damage);
                            enemiesHit[enemy] += 1;
                        }
                    }
                    currentTimer = 0; // Reset timer after processing all enemies
                }

                // Despawn object when particle system is no longer alive
                if (!particle.IsAlive(true))
                {
                    enemiesHit.Clear();
                    ObjectPooling.Instance.DespawnObject(gameObject);
                }
            }
        }

    }
}
