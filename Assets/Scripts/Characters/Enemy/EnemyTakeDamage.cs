using UnityEngine;

namespace Game
{
    public class EnemyTakeDamage : EnemyEvent
    {
        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);

            enemy.CharacterAnimator.SetTrigger(enemy.isHitTrigger);
            enemy.isAggro = true;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            enemy.Agent.isStopped = true;

            if (IsAnimationPlaying(enemy.damageAnim))
            {
                if (enemy.CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    isDone = true;
                }
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            enemy.Agent.isStopped = false;
        }

        public override bool IsDone()
        {
            return isDone;
        }
    }
}
