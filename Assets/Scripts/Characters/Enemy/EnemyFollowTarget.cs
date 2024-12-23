using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnemyFollowTarget : EnemyEvent
    {
        float updateTargetDelay = 0.4f; // Delay for updating the target position
        float animationCheckDelay = 0.2f; // Delay for checking animations

        float targetTimer = 0;
        float animationTimer = 0;

        float distance = 1.5f;

        private bool isAttackAnimationPlaying = false; // Cached result for animation check

        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);
            enemy.Agent.isStopped = false;
            SetNewDestination(enemy.Player.position);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            // Update animation timer
            animationTimer += Time.deltaTime;
            if (animationTimer >= animationCheckDelay)
            {
                isAttackAnimationPlaying = IsAnyAttackAnimationPlaying();
                animationTimer = 0;
            }

            // Skip if an attack animation is playing
            if (isAttackAnimationPlaying)
                return;

            // Update target timer
            targetTimer += Time.deltaTime;

            if (IsCloseToPlayer(distance))
            {
                enemy.SetNewEvent<EnemyAttack>();
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
