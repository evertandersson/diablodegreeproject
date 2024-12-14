using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class PlayerMovement : MonoBehaviour
    {
        Animator animator;
        PlayerManager playerManager;
        Rigidbody rb;

        private void Start()
        {
            playerManager = PlayerManager.Instance;
            animator = playerManager.Animator;
            rb = GetComponent<Rigidbody>();
        }

        public void SetDestination(Vector3 destinationPosition)
        {
            playerManager.Agent.isStopped = false;
            playerManager.Agent.SetDestination(destinationPosition);
        }

        public void Roll()
        {
            animator.applyRootMotion = true;
            animator.SetTrigger("Roll");

            playerManager.Agent.isStopped = true;

            StartCoroutine(StopRolling());
        }

        private IEnumerator StopRolling()
        {
            // Allow time for the roll animation to start
            yield return new WaitForSeconds(0.1f);

            // Wait until the roll animation finishes
            while (playerManager.IsAnimationPlaying("Roll"))
            {
                yield return null;
            }

            // Roll animation finished, return to idle state
            playerManager.CurrentPlayerState = PlayerManager.State.Idle;
            animator.applyRootMotion = false;

            // Re-enable the NavMeshAgent after the roll
            playerManager.Agent.isStopped = false;
        }

        private void OnAnimatorMove()
        {
            // Check if the player is rolling
            if (!PlayerManager.Instance.IsRolling)
                return;

            rb.isKinematic = false;
            float delta = Time.deltaTime;
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            rb.velocity = velocity;
        }
    }
}
