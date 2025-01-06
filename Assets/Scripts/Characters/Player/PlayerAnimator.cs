using Game;
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
        PlayerManager.Instance.SetFloatRunSpeed();

        animator.SetBool(isAttackingHash, isAttacking);
    }
}
