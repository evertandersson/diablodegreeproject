using UnityEngine;

namespace Game
{
    public class EnemyTakeDamage : EnemyEvent
    {
        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);

            enemy.Agent.isStopped = true;
            enemy.Animator.SetTrigger("IsHit");
            Debug.Log("Enemy took damage");

            // Disable movement and animation root motion
            enemy.Agent.enabled = false;
            enemy.RB.isKinematic = false;
            enemy.RB.useGravity = true;
            enemy.Animator.applyRootMotion = false; // Disable root motion

            // Freeze rotation to prevent spinning
            enemy.RB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (IsAnimationPlaying(enemy.damageAnimName))
            {
                // When the animation is done, set the event as done
                if (enemy.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    isDone = true;
                    Debug.Log("IsDone");
                }
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();

            // Re-enable Rigidbody and Animator behavior
            enemy.RB.isKinematic = true;
            enemy.RB.useGravity = false;
            enemy.RB.constraints = RigidbodyConstraints.FreezeRotation; // Allow rotation again if needed
            enemy.Animator.applyRootMotion = true;
            enemy.Agent.enabled = true;
            enemy.Agent.isStopped = false;
            Debug.Log("Done with takedamage");
        }


        public override bool IsDone()
        {
            return isDone;
        }
    }
}
