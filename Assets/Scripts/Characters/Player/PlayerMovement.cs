using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class PlayerMovement : MonoBehaviour
    {
        PlayerManager playerManager;
        Rigidbody rb;

        private float rollTimer = 0;
        private Vector3 rollDirection; // To store the roll direction
        private float initialYPosition; // To store the initial Y position

        // Buffer variables:
        private Vector3 bufferedDestination;
        public bool hasBufferedMovement;
        private int bufferedAttackIndex;
        private bool hasBufferedAttack;
        private bool hasBufferedRoll;

        private void Start()
        {
            playerManager = PlayerManager.Instance;
            rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;

            // Get the layer indices for Player and Enemy
            int playerLayer = LayerMask.NameToLayer("Player");
            int enemyLayer = LayerMask.NameToLayer("Enemy");

            // Ignore collisions between Player and Enemy layers
            Physics.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        }

        public void BufferInput(Vector3 destination)
        {
            bufferedDestination = destination;
            hasBufferedMovement = true;
        }

        public void BufferAttack(int attackIndex)
        {
            bufferedAttackIndex = attackIndex;
            hasBufferedAttack = true;
        }

        public void BufferRoll()
        {
            hasBufferedRoll = true;
        }

        public void ProcessBufferedInput()
        {
            if (hasBufferedMovement && !playerManager.IsAttacking)
            {
                RollEnd();
                SetDestination(bufferedDestination);
                ResetBufferedInput();
            }

            if (hasBufferedAttack)
            {
                RollEnd();
                PlayerManager.Instance.Attack(bufferedAttackIndex);
                ResetBufferedInput();
            }

            if (hasBufferedRoll)
            {
                PlayerManager.Instance.CurrentPlayerState = PlayerManager.State.Rolling;
                RollStart();
                ResetBufferedInput();
            }
        }

        private void ResetBufferedInput()
        {
            hasBufferedMovement = false;
            hasBufferedAttack = false;
            hasBufferedRoll = false;
        }

        public void SetDestination(Vector3 destinationPosition)
        {
            if (!playerManager.Agent.enabled || playerManager.IsAttacking) return;

            playerManager.Agent.isStopped = false;
            playerManager.Agent.SetDestination(destinationPosition);
        }

        public void RollStart()
        {
            playerManager.ClearAttack();
            rollTimer = 0;
            
            // Store the initial Y position
            initialYPosition = transform.position.y;

            // Use the mouseInputPosition from the MouseInput script
            Vector3 mouseWorldPosition = playerManager.mouseInput.mouseInputPosition;

            if (mouseWorldPosition == transform.position) mouseWorldPosition = transform.forward;

            // Calculate the direction from the player to the mouse position
            rollDirection = (mouseWorldPosition - transform.position).normalized;
            rollDirection.y = 0; // Ensure movement is constrained to the XZ plane

            // Disable NavMeshAgent and enable root motion for rolling
            if (playerManager.Agent.enabled)
            {
                playerManager.Agent.isStopped = true;
                playerManager.Agent.enabled = false;
            }

            // Enable Rigidbody physics for collisions
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            // Lock rotation to prevent unwanted turning and face the roll direction
            playerManager.transform.rotation = Quaternion.LookRotation(rollDirection);

            // Trigger the roll animation
            playerManager.CharacterAnimator.SetTrigger("Roll");
        }

        public void RollUpdate()
        {
            rollTimer += Time.deltaTime;

            if (rollTimer > 0.4f)
            {
                // If has buffered roll, perform another roll and return
                if (hasBufferedRoll)
                {
                    ProcessBufferedInput();
                    return;
                }

                // If has other buffered movement, set state to idle then perform it
                if (hasBufferedMovement)
                {
                    playerManager.CurrentPlayerState = PlayerManager.State.Idle;
                    ProcessBufferedInput();
                    return;
                }

                // Same for buffered attack, but perform when timer is 0.5 instead
                if (hasBufferedAttack && rollTimer > 0.5f)
                {
                    playerManager.CurrentPlayerState = PlayerManager.State.Idle;
                    ProcessBufferedInput();
                    return;
                }

                if (!playerManager.IsAnimationPlaying("Roll"))
                {
                    RollEnd();
                    playerManager.CurrentPlayerState = PlayerManager.State.Idle;
                }
            }
        }

        private void RollEnd()
        {
            // Re-enable NavMeshAgent and disable root motion after the roll
            playerManager.Agent.enabled = true;
            playerManager.Agent.isStopped = false;

            // Update the NavMeshAgent's position to prevent snapping
            Vector3 currentPosition = transform.position;
            currentPosition.y = initialYPosition; // Maintain the original Y position
            playerManager.Agent.Warp(currentPosition);

            // Disable Rigidbody physics after the roll
            rb.isKinematic = true;

            rollTimer = 0;
        }

        private void OnAnimatorMove()
        {
            if (playerManager.CurrentPlayerState != PlayerManager.State.Rolling) return;
            
            // Apply root motion position, but lock the Y position
            Vector3 newPosition = transform.position + playerManager.CharacterAnimator.deltaPosition;
            newPosition.y = initialYPosition; // Maintain the original Y position
            transform.position = newPosition;

            // Force rotation to stay aligned with rollDirection
            playerManager.transform.rotation = Quaternion.LookRotation(rollDirection);
        }
    }
}
