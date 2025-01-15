using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnemyRoaming : EnemyEvent
    {
        private float walkingRange = 20f;
        private float roamingTimeout = 5f; // Max time to attempt roaming
        private float elapsedRoamingTime = 0f; // Timer for roaming timeout

        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);
            enemy.Agent.isStopped = false;
            elapsedRoamingTime = 0f; // Reset timeout
            isAttackAnimationPlaying = false; // Reset animation state

            SetNewDestination(GetRandomPosition()); // Set the first destination
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            // Update animation timer
            CheckAnimationInterval();

            // Skip if an attack animation is playing
            if (isAttackAnimationPlaying)
                return;

            // If the enemy can see the player, switch to FollowTarget event
            if (enemy.CanSeeTarget(PlayerManager.Instance.transform, offset))
            {
                isDone = true;
                enemy.EnemyEventHandler.RemoveEvent(this);
                enemy.SetNewEvent<EnemyFollowTarget>();
            }

            // Wait for the NavMeshAgent to calculate the path
            if (enemy.Agent.pathPending)
                return;

            // Check if the enemy has reached the destination
            if (enemy.Agent.remainingDistance <= enemy.Agent.stoppingDistance && !enemy.Agent.pathPending)
            {
                isDone = true; // Mark roaming as done
            }

            // Timeout logic
            elapsedRoamingTime += Time.deltaTime;
            if (elapsedRoamingTime > roamingTimeout)
            {
                isDone = true;
            }
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
