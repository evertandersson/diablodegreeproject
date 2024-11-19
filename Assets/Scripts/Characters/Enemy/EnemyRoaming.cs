using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnemyRoaming : EnemyEvent
    {
        private Vector3 targetPosition;
        private float walkingRange = 20f;
        private float roamingTimeout = 5f; // Max time to attempt roaming
        private float elapsedTime = 0f;

        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);
            targetPosition = transform.position; // Initialize target position
            SetNewDestination(); // Set the first destination
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            // Check if the enemy has reached the current target position
            if (agent.pathPending)
                return; // Wait until the NavMeshAgent finishes calculating the path

            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                Debug.Log($"Reached destination: {targetPosition}");
                isDone = true; // End roaming after reaching the destination
                return;
            }

            // Timeout logic
            elapsedTime += Time.deltaTime;
            if (elapsedTime > roamingTimeout)
            {
                isDone = true;
            }
        }

        private void SetNewDestination()
        {
            // Try up to 10 random positions
            for (int i = 0; i < 10; i++) 
            {
                Vector3 potentialPosition = GetRandomPosition();
                if (IsValidDestination(potentialPosition))
                {
                    targetPosition = potentialPosition;
                    agent.SetDestination(targetPosition);
                    elapsedTime = 0f; // Reset timeout
                    Debug.Log($"Setting new destination: {targetPosition}");
                    return;
                }
            }

            isDone = true;
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-walkingRange, walkingRange),
                0,
                Random.Range(-walkingRange, walkingRange)
            );

            NavMeshHit hit;
            // Ensure the position is valid on the NavMesh
            if (NavMesh.SamplePosition(randomPosition, out hit, 20f, NavMesh.AllAreas))
            {
                return hit.position;
            }

            // Default to current position if no valid position is found
            return transform.position;
        }

        private bool IsValidDestination(Vector3 position)
        {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(position, path);
            return path.status == NavMeshPathStatus.PathComplete; // Valid if the path is complete
        }

        public override bool IsDone()
        {
            return isDone;
        }
    }
}
