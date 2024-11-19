using UnityEngine;

namespace Game
{
    public class EnemyTakeDamage : EnemyEvent
    {
        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);
            agent.isStopped = true;
            animator.SetTrigger("IsHit");
            Debug.Log("Enemy took damage");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (IsAnimationPlaying(enemy.damageAnimName))
            {
                // When the animation is done, destroy the enemy object
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    isDone = true;
                    Debug.Log("IsDone");
                }
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            agent.isStopped = false;
            Debug.Log("Done with takedamge");
        }

        public override bool IsDone()
        {
            return isDone;
        }

        private bool IsAnimationPlaying(string animationName)
        {
            // Check if the current animation state is the one we are interested in
            return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
        }
    }
}
