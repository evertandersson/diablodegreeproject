using UnityEngine;

namespace Game
{
    public class GolemBoss : Enemy
    {
        public static bool isGolemKilled = false;

        public float distanceToJumpAttack = 4f;

        [HideInInspector] public int jumpAttackTrigger = Animator.StringToHash("IntroAttack");
        [HideInInspector] public int jumpAttackAnim = Animator.StringToHash("IntroAttack");


        protected override void OnEnable()
        {
            Agent.avoidancePriority = 10;
            TriggerCutscene02.StopCutscene02 += StopCutscene;
        }

        protected override void OnDisable()
        {
            TriggerCutscene02.StartCutscene02 -= StopCutscene;
        }

        public void DisableBoss()
        {
            gameObject.SetActive(false);
            isGolemKilled = true;
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

        protected override void Die()
        {
            base.Die();
            isGolemKilled = true;
            SaveManager.Instance.Save();
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
