using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class PlayerMovement : MonoBehaviour
    {
        PlayerManager playerManager;
        Rigidbody rb;

        private float rollTimer = 0;
        private Vector3 rollDirection;  // To store the roll direction

        private void Start()
        {
            playerManager = PlayerManager.Instance;
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        }

        public void SetDestination(Vector3 destinationPosition)
        {
            if (!playerManager.Agent.enabled) return;

            playerManager.Agent.isStopped = false;
            playerManager.Agent.SetDestination(destinationPosition);
        }

        public void StartRolling()
        {
            // Use the mouseInputPosition from the MouseInput script
            Vector3 mouseWorldPosition = playerManager.mouseInput.mouseInputPosition;

            // Calculate the direction from the player to the mouse position
            rollDirection = (mouseWorldPosition - transform.position).normalized;
            rollDirection.y = 0; // Ensure movement is constrained to the XZ plane

            // Disable NavMeshAgent and enable root motion for rolling
            //playerManager.Agent.velocity = Vector3.zero; // Clear any lingering velocity
            playerManager.Agent.isStopped = true;
            playerManager.Agent.enabled = false;
            playerManager.Animator.applyRootMotion = true;

            // Enable Rigidbody physics for collisions
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Lock rotation to prevent unwanted turning and face the roll direction
            playerManager.transform.rotation = Quaternion.LookRotation(rollDirection);

            // Trigger the roll animation
            playerManager.Animator.SetTrigger("Roll");

            // Update state
            playerManager.CurrentPlayerState = PlayerManager.State.Rolling;
        }

        public void HandleEndRolling()
        {
            rollTimer += Time.deltaTime;
            if (rollTimer > 0.2f)
            {
                if (!playerManager.IsAnimationPlaying("Roll"))
                {
                    // Re-enable NavMeshAgent and disable root motion after the roll
                    playerManager.Agent.enabled = true;
                    playerManager.Agent.isStopped = false;
                    playerManager.Animator.applyRootMotion = false;

                    // Disable Rigidbody physics after the roll
                    Rigidbody rb = GetComponent<Rigidbody>();
                    rb.isKinematic = true;

                    rollTimer = 0;
                    playerManager.CurrentPlayerState = PlayerManager.State.Idle;
                }
            }
        }

        private void OnAnimatorMove()
        {
            if (playerManager.CurrentPlayerState != PlayerManager.State.Rolling)
                return;

            // Apply root motion position
            transform.position += playerManager.Animator.deltaPosition;

            // Force rotation to stay aligned with rollDirection
            playerManager.transform.rotation = Quaternion.LookRotation(rollDirection);

        }

    }
}
