using Game;
using UnityEngine;

public class AnimationSpeedManager : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("AttackSpeed", PlayerManager.Instance.AttackSpeed);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetFloat("AttackSpeed", 1f);
    }

}
