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
            enemy.Agent.isStopped = false;
            elapsedTime = 0f; // Reset timeout
            SetNewDestination(); // Set the first destination
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (enemy.Agent.pathPending)
                return; // Wait until the NavMeshAgent finishes calculating the path

            if (enemy.Agent.remainingDistance <= enemy.Agent.stoppingDistance && !enemy.Agent.pathPending)
            {
                Debug.Log($"Reached destination: {targetPosition}");
                isDone = true; // Mark roaming as done
            }

            // Timeout logic
            elapsedTime += Time.deltaTime;
            if (elapsedTime > roamingTimeout)
            {
                Debug.Log("Roaming timed out");
                isDone = true;
            }
        }

        private void SetNewDestination()
        {
            for (int i = 0; i < 10; i++) // Try up to 10 random positions
            {
                Vector3 potentialPosition = GetRandomPosition();
                if (IsValidDestination(potentialPosition))
                {
                    targetPosition = potentialPosition;
                    enemy.Agent.SetDestination(targetPosition);
                    elapsedTime = 0f; // Reset timeout
                    Debug.Log($"Setting new destination: {targetPosition}");
                    return;
                }
            }

            isDone = true; // Mark as done if no valid destination is found
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-walkingRange, walkingRange),
                0,
                Random.Range(-walkingRange, walkingRange)
            );

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 20f, NavMesh.AllAreas))
            {
                return hit.position;
            }

            return transform.position; // Default to current position if invalid
        }

        private bool IsValidDestination(Vector3 position)
        {
            NavMeshPath path = new NavMeshPath();
            enemy.Agent.CalculatePath(position, path);
            return path.status == NavMeshPathStatus.PathComplete;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            targetPosition = transform.position;
            enemy.Agent.isStopped = true;
        }

        public override bool IsDone()
        {
            return isDone;
        }
    }
}
