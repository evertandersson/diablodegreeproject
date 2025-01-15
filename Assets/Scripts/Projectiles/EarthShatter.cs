using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EarthShatter : Fireball
    {
        public List<Enemy> enemiesHit = new List<Enemy>();
        [SerializeField] private ParticleSystem particle;

        private void Awake()
        {
            particle = GetComponent<ParticleSystem>();
        }

        public override void OnObjectSpawn()
        {
            base.OnObjectSpawn();
            enemiesHit.Clear();
        }

        protected override void FixedUpdate()
        {
            // Do nothing
        }

        protected override void OnTriggerEnter(Collider other)
        {
            // Do nothing
        }



        private void Update()
        {
            if (particle != null)
            {
                if (!particle.IsAlive(true))
                {
                    ObjectPooling.Instance.DespawnObject(this.gameObject);
                }
            }
        }
    }

}
