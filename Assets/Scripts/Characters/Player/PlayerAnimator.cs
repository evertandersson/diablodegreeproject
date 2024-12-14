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
        float currentSpeed = animator.GetFloat("RunSpeed");
        float targetSpeed = Mathf.Clamp01(speed);
        float smoothSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5f);
        animator.SetFloat("RunSpeed", smoothSpeed);

        animator.SetBool("IsAttacking", isAttacking);
    }
}
