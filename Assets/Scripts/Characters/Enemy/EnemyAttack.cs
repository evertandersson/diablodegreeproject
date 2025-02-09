using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class EnemyAttack : EnemyEvent
    {
        protected int currentAttackIndex;

        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);
            if (!IsCloseToPlayer(enemy.distanceToAttack + 0.5f))
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

        protected virtual void HandleAnimationCombo()
        {
            // Don't attack while in take damage animation
            if (enemy.IsAnimationPlayingStrict(enemy.damageAnim))
                return;

            // If current attack animation is playing
            if (IsAnimationPlaying(enemy.attackAnims[currentAttackIndex]))
            {
                if (enemy.CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
                {
                    // If close to player, do the next attack in the list
                    if (IsCloseToPlayer(enemy.distanceToAttack + 0.5f))
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
