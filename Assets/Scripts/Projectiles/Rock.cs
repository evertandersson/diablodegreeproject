using UnityEngine;

namespace Game
{
    public class Rock : MonoBehaviour
    {
        [SerializeField] private ParticleSystem rockParticle;

        private EarthShatter earthShatter;

        private void Awake()
        {
            earthShatter = GetComponentInParent<EarthShatter>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (!earthShatter.enemiesHit.Contains(other.GetComponent<Enemy>()))
            {
                Enemy enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                {
                    earthShatter.enemiesHit.Add(enemy);
                    enemy.TakeDamage(PlayerManager.Instance.Damage);
                }
            }
        }
    }
}
