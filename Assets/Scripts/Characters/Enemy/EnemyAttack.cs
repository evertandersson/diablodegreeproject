using UnityEngine;

namespace Game
{
    public class EnemyAttack : EnemyEvent
    {
        private int currentAttackIndex;

        public override void OnBegin(bool firstTime)
        {
            if (!IsCloseToPlayer())
            {
                isDone = true;
                return;
            }

            currentAttackIndex = 0;
            enemy.Agent.isStopped = true;
            enemy.Animator.SetTrigger("Attack");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            enemy.HandleRotation(PlayerManager.Instance.transform.position);
            HandleAnimationCombo();
        }

        private void HandleAnimationCombo()
        {
            // Don't attack while in take damage animation
            if (IsAnimationPlaying(enemy.damageAnimName))
                return;

            // If current attack animation is playing
            if (IsAnimationPlaying(enemy.attackAnimNames[currentAttackIndex]))
            {
                if (enemy.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
                {
                    // If close to player, do the next attack in the list
                    if (IsCloseToPlayer())
                    {
                        currentAttackIndex++;
                        if (currentAttackIndex >= enemy.attackAnimNames.Length)
                        {
                            currentAttackIndex = 0;
                        }
                        enemy.Animator.SetTrigger("Attack"); // Trigger next animation
                    }
                    else
                    {
                        isDone = true; // End combo if not close to the player
                        Debug.Log("IsDone");
                    }
                }
            }
        }

        public void DealDamage()
        {
            if (IsCloseToPlayer() && IsTargetedAtPlayer())
            {
                PlayerManager.Instance.TakeDamage(enemy.Damage);
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
        }

        public override bool IsDone()
        {
            return isDone;
        }
    }
}
