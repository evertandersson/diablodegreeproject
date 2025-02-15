using Game;
using UnityEngine;

public class StartRollTimer : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerManager.Instance.playerMovement.rollTimer = 0;    
    }
}
