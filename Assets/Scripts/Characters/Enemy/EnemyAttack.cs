using UnityEngine;

namespace Game
{
    public class EnemyAttack : EnemyEvent
    {
        private int currentAttackIndex;
        float distance = 2.0f;

        public override void OnBegin(bool firstTime)
        {
            if (!IsCloseToPlayer(distance))
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
            if (enemy.IsAnimationPlaying(enemy.damageAnimName))
                return;

            // If current attack animation is playing
            if (enemy.IsAnimationPlaying(enemy.attackAnimNames[currentAttackIndex]))
            {
                if (enemy.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
                {
                    // If close to player, do the next attack in the list
                    if (IsCloseToPlayer(distance))
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
            if (IsCloseToPlayer(distance) && IsTargetedAtPlayer())
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
