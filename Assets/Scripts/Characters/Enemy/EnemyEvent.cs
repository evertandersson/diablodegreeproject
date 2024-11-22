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
            Debug.Log("Enemy Event Started: " + enemy.EnemyEventHandler.CurrentEvent);
        }

        public override void OnUpdate()
        {
            float speed = enemy.Agent.velocity.magnitude / enemy.Agent.speed;
            enemy.Animator.SetFloat("RunSpeed", speed);
        }

        public override void OnEnd()
        {
            isDone = false;
            Debug.Log("Enemy Event Ended");
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
                    Debug.Log($"Setting new destination: {targetPosition}");
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
    }
}