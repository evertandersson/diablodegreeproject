using System.Threading;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnemyFollowTarget : EnemyEvent
    {
        protected float updateTargetDelay = 0.4f; // Delay for updating the target position
        protected float targetTimer = 0;

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

            if (!enemy.Agent.enabled)
                return;

            if (enemy.IsAnimationPlaying(enemy.damageAnim))
            {
                if (Vector3.Distance(enemy.Agent.destination, transform.position) > 0.5f)
                    SetNewDestination(transform.position);
                return;
            }

            // Update animation timer
            CheckAnimationInterval();

            if (enemy.Agent.isStopped != IsAnyAttackAnimationPlaying())
                enemy.Agent.isStopped = IsAnyAttackAnimationPlaying();

            // Skip if an attack animation is playing
            if (isAttackAnimationPlaying)
                return;


            // Update target timer
            targetTimer += Time.deltaTime;

            if (IsCloseToPlayer(enemy.distanceToAttack) && !IsAnyAttackAnimationPlaying())
            {
                if (enemy is RangedEnemy)
                {
                    if (IsTargetedAtPlayer())
                    {
                        enemy.Attack();
                        return;
                    }
                }
                else
                {
                    enemy.Attack();
                    return;
                }
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
