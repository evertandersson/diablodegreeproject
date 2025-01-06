using UnityEngine;

namespace Game
{
    public class GolemBoss : Enemy
    {
        public override void TakeDamage(int damage)
        {
            if (!IsDead)
            {
                health -= damage;
                healthBar.SetHealth(health);
                StartCoroutine(FlashRoutine());
                bloodSplashEffect.Play();

                if (health <= 0)
                {
                    Die();
                }
            }
        }
    }
}
