using UnityEngine;

namespace Game
{
    public class GolemFollowTarget : EnemyFollowTarget
    {
        private float attackCooldownTimer = 0f;
        private float attackCooldownDuration = 0.5f;

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

            // Cooldown timer for attacks
            attackCooldownTimer -= Time.deltaTime;

            if (attackCooldownTimer > 0)
                return;

            // Update target timer
            targetTimer += Time.deltaTime;


            if (IsCloseToPlayer(enemy.distanceToAttack) && !IsAnyAttackAnimationPlaying())
            {
                enemy.Attack();
                return;
            }

            if (IsCloseToPlayer(enemy.golem.distanceToJumpAttack) && IsTargetedAtPlayer())
            {
                int attackToPerform = Random.Range(0, 2);
                if (attackToPerform == 0 || attackToPerform == 2)
                {
                    enemy.golem.RangedAttack();
                }
                else
                {
                    enemy.golem.JumpAttack();
                }
                attackCooldownTimer = attackCooldownDuration;
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
