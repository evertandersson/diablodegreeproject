using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnemyFollowTarget : EnemyEvent
    {
        float updateTargetDelay = 0.4f; // Delay for updating the target position
        float targetTimer = 0;

        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);
            if (enemy.Agent.enabled)
            {
                enemy.Agent.isStopped = false;
                SetNewDestination(enemy.Player.position);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            // Update animation timer
            CheckAnimationInterval();

            // Skip if an attack animation is playing
            if (isAttackAnimationPlaying)
                return;

            // Update target timer
            targetTimer += Time.deltaTime;

            if (IsCloseToPlayer(enemy.distanceToAttack))
            {
                enemy.Attack();
            }

            if (targetTimer > updateTargetDelay)
            {
                // Update target position
                SetNewDestination(enemy.Player.position);
                targetTimer = 0;
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
