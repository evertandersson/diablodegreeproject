using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    private enum State
    {
        Idle,
        FollowTarget,
        Attack,
        Dead
    }

    [SerializeField]
    private State state;

    private NavMeshAgent agent;

    private Animator animator;

    [SerializeField]
    private Vector3 targetPosition;

    private float walkingRange = 100;

    protected override void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        maxHealth = 20;
        base.Start();

        targetPosition = transform.position;
        state = State.Idle;
    }

    private void Update()
    {
        float speed = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("RunSpeed", speed);

        switch (state)
        {
            case State.Idle:
                // If close to the current target position, get a new one
                if (Vector3.Distance(transform.position, targetPosition) < 1f)
                {
                    targetPosition = GetRandomPosition();
                    agent.SetDestination(targetPosition);
                    Debug.Log($"Setting new destination: {targetPosition}");
                }
                break;

            case State.FollowTarget:
                // Logic for following a target
                break;

            case State.Attack:
                // Logic for attacking
                break;

            case State.Dead:
                // Logic for death
                break;
        }
    }

    protected Vector3 GetRandomPosition()
    {
        Vector3 randomPosition = new Vector3(Random.Range(-walkingRange, walkingRange), 0, Random.Range(-walkingRange, walkingRange));
        NavMeshHit hit;

        // Ensure the position is valid on the NavMesh
        if (NavMesh.SamplePosition(randomPosition, out hit, 20f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        // Default to current position if no valid position is found
        return transform.position;
    }


    public override void Attack(int attackIndex)
    {
        throw new System.NotImplementedException();
    }

    protected override void Die()
    {
        throw new System.NotImplementedException();
    }
}
