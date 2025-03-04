using UnityEngine;

namespace Game
{
    public class GolemBoss : Enemy
    {
        public static bool isGolemKilled = false;

        public float distanceToJumpAttack = 4f;

        [HideInInspector] public int jumpAttackTrigger = Animator.StringToHash("IntroAttack");
        [HideInInspector] public int jumpAttackAnim = Animator.StringToHash("IntroAttack");

        // DEBUG ONLY
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                isGolemKilled = true;
            }
        }

        protected override void OnEnable()
        {
            Agent.avoidancePriority = 10;
            Popup.Pause += Pause;
            Popup.UnPause += UnPause;
            TriggerCutscene02.StopCutscene02 += StopCutscene;
        }

        protected override void OnDisable()
        {
            Popup.Pause -= Pause;
            Popup.UnPause -= UnPause;
            TriggerCutscene02.StartCutscene02 -= StopCutscene;
        }

        public void DisableBoss()
        {
            gameObject.SetActive(false);
            isGolemKilled = true;
        }

        public override void TakeDamage(int damage, bool isCriticalHit = false)
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
