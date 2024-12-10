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
            enemy.Animator.SetFloat("RunSpeed", speed);
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
                    //Debug.Log($"Setting new destination: {targetPosition}");
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

        protected bool IsAnyAttackAnimationPlaying()
        {
            foreach (var animationName in enemy.attackAnimNames)
            {
                if (IsAnimationPlaying(animationName))
                {
                    return true;
                }
            }
            return false; // No animations are playing
        }

        protected bool IsAnimationPlaying(string animationName)
        {
            // Check if the current animation state is the one we are interested in
            return enemy.Animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
        }

        protected bool IsCloseToPlayer(float distance)
        {
            return Vector3.Distance(enemy.transform.position, enemy.Player.position) < distance;
        }

        protected bool IsTargetedAtPlayer()
        {
            RaycastHit hit;

            if (Physics.Raycast(enemy.transform.position, transform.forward, out hit))
            {
                // Check if the raycast hit the player
                return hit.transform == enemy.Player;
            }

            return false; // Raycast did not hit the player
        }
    }
}