using System.Collections;
using System.Linq.Expressions;
using UnityEngine;

namespace Game
{
    public class EnemyAttack : EnemyEvent
    {
        [SerializeField] protected int currentAttackIndex;

        public override void OnBegin(bool firstTime)
        {
            isDone = false; // Ensure state is active
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

        protected void HandleAnimationCombo()
        {
            // Don't attack while in take damage animation
            if (IsAnimationPlaying(enemy.damageAnim))
                return;

            if (currentAttackIndex >= enemy.attackAnims.Length)
            {
                isDone = true;
                return;
            }

            // If current attack animation is playing
            if (IsAnimationPlaying(enemy.attackAnims[currentAttackIndex]))
            {
                if (enemy.CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
                {
                    // If close to player, do the next attack in the list
                    if (IsCloseToPlayer(enemy.distanceToAttack + 0.5f))
                    {
                        currentAttackIndex++;
                        if (currentAttackIndex >= enemy.attackAnims.Length)
                        {
                            isDone = true;
                            return;
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
            enemy.CharacterAnimator.SetTrigger(enemy.endAttackTrigger);
        }

        public override bool IsDone()
        {
            return isDone;
        }
    }

}
