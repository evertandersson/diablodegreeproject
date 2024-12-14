using Game;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;
    PlayerManager playerManager;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        playerManager = PlayerManager.Instance;
    }

    public void HandleAnimations(bool isAttacking)
    {
        float speed = agent.velocity.magnitude / agent.speed;

        animator.SetFloat("RunSpeed", Mathf.Clamp01(speed));
        animator.SetBool("IsAttacking", isAttacking);
    }
}
