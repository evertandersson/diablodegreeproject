using UnityEngine;

namespace Game
{
    public class RangedEnemyFollowTarget : EnemyFollowTarget
{
        public override void OnUpdate()
        {
            enemy.SetFloatRunSpeed();

            if (enemy.isAggro && enemy.EnemyEventHandler.CurrentEvent is not EnemyTakeDamage)
            {
                enemy.SetNewEvent<EnemyFollowTarget>();
                enemy.isAggro = false;
            }

            if (!enemy.Agent.enabled)
                return;

            if (enemy.IsAnimationPlayingStrict(enemy.damageAnim))
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
                if (IsTargetedAtPlayer())
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
    }

}
