using Game;
using UnityEngine;

public class OnAnimationEnd : StateMachineBehaviour
{
    // This is triggered when the animation state finishes
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerManager playerManager = PlayerManager.Instance;

        if (playerManager != null)
        {
            if (playerManager.playerMovement.hasBufferedInput) return;

            animator.SetBool("IsAttacking", false);

            playerManager.ClearAttack(); // Reset the attack state
            playerManager.playerMovement.ResetBufferedInput(); // Reset buffered inputs

            // Process buffered input if any
            if (playerManager.playerMovement.hasBufferedInput)
            {
                playerManager.playerMovement.ProcessBufferedInput(true);
            }

            // Optionally log for debugging
            Debug.Log("Attack animation finished, processed buffered inputs.");
        }
        
    }
}
