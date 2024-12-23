using UnityEngine;

namespace Game
{
    public class EnemyTakeDamage : EnemyEvent
    {
        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);

            enemy.Agent.isStopped = true;
            enemy.CharacterAnimator.SetTrigger("IsHit");
            enemy.isAggro = true;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (IsAnimationPlaying(enemy.damageAnimName))
            {
                // When the animation is done, destroy the enemy object
                if (enemy.CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    isDone = true;
                    Debug.Log("IsDone");
                }
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            enemy.Agent.isStopped = false;
            Debug.Log("Done with takedamge");
        }

        public override bool IsDone()
        {
            return isDone;
        }
    }
}
