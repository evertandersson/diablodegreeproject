using UnityEngine;

namespace Game
{
    public class GolemBoss : Enemy
    {
        public float distanceToJumpAttack = 4f;

        [HideInInspector] public int jumpAttackTrigger = Animator.StringToHash("IntroAttack");
        [HideInInspector] public int jumpAttackAnim = Animator.StringToHash("IntroAttack");

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

        public void JumpAttack()
        {
            SetNewEvent<JumpAttack>();
        }
    }
}
