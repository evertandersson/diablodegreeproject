using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Game 
{ 
    public class EnemyEvent : EventHandler.GameEventBehaviour
    {
        protected Enemy enemy;

        protected bool isDone = false;

        protected Vector3 targetPosition;
        protected float elapsedTime = 0f;

        Vector3 offset = new Vector3(0, 1.2f, 0);

        private void Start()
        {
            enemy = GetComponent<Enemy>();
        }

        public override void OnBegin(bool firstTime)
        {
            isDone = false;
        }

        public override void OnUpdate()
        {
            float speed = enemy.Agent.velocity.magnitude / enemy.Agent.speed;
            enemy.CharacterAnimator.SetFloat("RunSpeed", speed);

            if (enemy.isAggro && enemy.EnemyEventHandler.CurrentEvent is not EnemyTakeDamage)
            {
                enemy.SetNewEvent<EnemyFollowTarget>();
                enemy.isAggro = false;
            }
        }

        public override void OnEnd()
        {
            isDone = false;
        }

        public override bool IsDone()
        {
            return true;
        }

        protected void SetNewDestination(Vector3 pos)
        {
            for (int i = 0; i < 10; i++) // Try up to 10 random positions
            {
                if (IsValidDestination(pos))
                {
                    targetPosition = pos;
                    enemy.Agent.SetDestination(targetPosition);
                    elapsedTime = 0f; // Reset timeout
                    return;
                }
            }

            isDone = true; // Mark as done if no valid destination is found
        }

        protected bool IsValidDestination(Vector3 position)
        {
            NavMeshPath path = new NavMeshPath();
            enemy.Agent.CalculatePath(position, path);
            return path.status == NavMeshPathStatus.PathComplete;
        }

        public bool IsAnimationPlaying(string animationName)
        {
            // Check if the current animation state is the one we are interested in
            return enemy.CharacterAnimator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
        }

        protected bool IsAnyAttackAnimationPlaying()
        {
            foreach (var animationName in enemy.attackAnimNames)
            {
                if (enemy.IsAnimationPlaying(animationName))
                {
                    return true;
                }
            }
            return false; // No animations are playing
        }

        protected bool IsCloseToPlayer(float distance)
        {
            return Vector3.Distance(enemy.transform.position, enemy.Player.position) < distance;
        }

        protected bool IsTargetedAtPlayer()
        {
            RaycastHit hit;

            if (Physics.Raycast(enemy.transform.position + offset, transform.forward, out hit, enemy.detectionMask))
            {
                // Check if the raycast hit the player
                return hit.transform == enemy.Player;
            }

            return false; // Raycast did not hit the player
        }
    }
}