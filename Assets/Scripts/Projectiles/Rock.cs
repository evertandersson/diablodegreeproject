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
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null)
            {
                if (!earthShatter.enemiesHit.ContainsKey(enemy))
                {
                    enemy.TakeDamage(PlayerManager.Instance.Damage);
                    earthShatter.enemiesHit.Add(enemy, 0);
                }
            }
        }
    }
}
