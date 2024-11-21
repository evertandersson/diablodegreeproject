using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    public void HandleAnimations(bool isAttacking)
    {
        float speed = agent.velocity.magnitude / agent.speed;

        animator.SetFloat("RunSpeed", Mathf.Clamp01(speed));
        animator.SetBool("IsAttacking", isAttacking);
    }
}
