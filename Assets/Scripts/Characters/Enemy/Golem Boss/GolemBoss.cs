using UnityEngine;

namespace Game
{
    public class GolemBoss : Enemy
    {
        public float distanceToJumpAttack = 4f;

        [HideInInspector] public int jumpAttackTrigger = Animator.StringToHash("IntroAttack");
        [HideInInspector] public int jumpAttackAnim = Animator.StringToHash("IntroAttack");


        protected override void OnEnable()
        {
            TriggerCutscene02.StopCutscene02 += StopCutscene;
        }

        protected override void OnDisable()
        {
            TriggerCutscene02.StartCutscene02 -= StopCutscene;
        }

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

        private void StopCutscene()
        {
            standStill = false;
            isAggro = true;
        }

        public void JumpAttack()
        {
            SetNewEvent<JumpAttack>();
        }

        public void RangedAttack()
        {
            SetNewEvent<GolemRangedAttack>();
        }
    }
}
