using UnityEngine;

namespace Game
{
    public class GolemFollowTarget : EnemyFollowTarget
    {
        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);
        }

        public override void OnUpdate()
        {
            enemy.SetFloatRunSpeed();

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
                return;
            }

            if (IsCloseToPlayer(enemy.golem.distanceToJumpAttack) && IsTargetedAtPlayer())
            {
                enemy.golem.JumpAttack();
                return;
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
