using UnityEngine;

namespace Game
{
    public class EnemyAttack : EnemyEvent
    {
        private int currentAttackIndex;

        public override void OnBegin(bool firstTime)
        {
            currentAttackIndex = 0;
            enemy.Agent.isStopped = true;
            enemy.Animator.SetTrigger("Attack");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            HandleAnimationCombo();
        }

        private void HandleAnimationCombo()
        {
            if (IsAnimationPlaying(enemy.attackAnimNames[currentAttackIndex]))
            {
                if (enemy.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f)
                {
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
