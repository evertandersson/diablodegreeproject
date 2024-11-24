using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnemyRoaming : EnemyEvent
    {
        private float walkingRange = 20f;
        private float roamingTimeout = 5f; // Max time to attempt roaming

        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);
            enemy.Agent.isStopped = false;
            elapsedTime = 0f; // Reset timeout
            SetNewDestination(GetRandomPosition()); // Set the first destination
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (IsAnyAttackAnimationPlaying())
                return;

            //If enemy can see player, remove this event and add FollowTarget event
            if (enemy.CanSeePlayer())
            {
                isDone = true;
                enemy.EnemyEventHandler.RemoveEvent(this);
                enemy.SetNewEvent<EnemyFollowTarget>();
            }

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
