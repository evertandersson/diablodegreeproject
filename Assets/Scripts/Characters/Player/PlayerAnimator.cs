using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    private int runSpeedHash = Animator.StringToHash("RunSpeed");
    private int isAttackingHash = Animator.StringToHash("IsAttacking");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    public void HandleAnimations(bool isAttacking)
    {
        float speed = agent.velocity.magnitude / agent.speed;
        float currentSpeed = animator.GetFloat(runSpeedHash);
        float targetSpeed = Mathf.Clamp01(speed);
        float smoothSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5f);
        animator.SetFloat(runSpeedHash, smoothSpeed);

        animator.SetBool(isAttackingHash, isAttacking);
    }
}
