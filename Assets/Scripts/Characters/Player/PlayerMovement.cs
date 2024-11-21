using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    NavMeshAgent agent;

    float rotationSpeed = 10.0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void StandStill(Vector3 destinationPosition)
    {
        destinationPosition.y = transform.position.y;

        Vector3 direction = destinationPosition - transform.position;

        if (direction.sqrMagnitude > 0.01f)  // Check if the direction is not too small (to avoid jittering)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        agent.isStopped = true;
    }

    public void SetDestination(Vector3 destinationPosition)
    {
        agent.isStopped = false;
        agent.SetDestination(destinationPosition);
    }


}
