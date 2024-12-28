using UnityEngine;

namespace Game
{
    public class EnemyAttack : EnemyEvent
    {
        private int currentAttackIndex;
        float distance = 2.0f;

        public override void OnBegin(bool firstTime)
        {
            if (!IsCloseToPlayer(enemy.distanceToAttack))
            {
                isDone = true;
                return;
            }

            currentAttackIndex = 0;
            enemy.Agent.isStopped = true;
            enemy.CharacterAnimator.SetTrigger(enemy.attackTrigger);
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
            if (IsAnimationPlaying(enemy.damageAnim))
                return;

            // If current attack animation is playing
            if (IsAnimationPlaying(enemy.attackAnims[currentAttackIndex]))
            {
                if (enemy.CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
                {
                    // If close to player, do the next attack in the list
                    if (IsCloseToPlayer(distance))
                    {
                        currentAttackIndex++;
                        if (currentAttackIndex >= enemy.attackAnims.Length)
                        {
                            currentAttackIndex = 0;
                        }
                        enemy.CharacterAnimator.SetTrigger(enemy.attackTrigger); // Trigger next animation
                    }
                    else
                    {
                        isDone = true; // End combo if not close to the player
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
