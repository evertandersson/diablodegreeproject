using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class PlayerMovement : MonoBehaviour
    {
        PlayerManager playerManager;
        Rigidbody rb;

        public float rollTimer = 0;
        private Vector3 rollDirection; // To store the roll direction
        private float initialYPosition; // To store the initial Y position
        private Vector3 offset = new Vector3(0, 1.2f, 0);

        // Buffer variables:
        private Vector3 bufferedDestination;
        public bool hasBufferedMovement;
        private int bufferedAttackIndex;
        private bool hasBufferedAttack;
        private bool hasBufferedRoll;
        [SerializeField] private LayerMask groundLayer;

        // Animation names:
        private int rollTrigger = Animator.StringToHash("Roll");

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
            Physics.IgnoreLayerCollision(playerLayer, playerLayer, true);
        }

        public bool ReadyForAnotherInput(float timer, float targetTime)
        {
            return timer > targetTime && PlayerManager.Instance.isInteracting;
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

        public void ProcessBufferedInput(bool previousActionIsAttack = false)
        {
            if (hasBufferedRoll)
            {
                PlayerManager.Instance.CurrentPlayerState = PlayerManager.State.Rolling;
                RollStart();
                ResetBufferedInput();
                return;
            }

            if (hasBufferedMovement && !playerManager.IsAttacking)
            {
                playerManager.CurrentPlayerState = PlayerManager.State.Idle;
                if (!previousActionIsAttack) RollEnd();
                SetDestination(bufferedDestination);
                ResetBufferedInput();
                return;
            }

            if (hasBufferedAttack)
            {
                playerManager.CurrentPlayerState = PlayerManager.State.Idle;
                if (!previousActionIsAttack) RollEnd();
                PlayerManager.Instance.Attack(bufferedAttackIndex);
                ResetBufferedInput();
                return;
            }

            playerManager.CurrentPlayerState = PlayerManager.State.Idle;
        }

        private void FixedUpdate()
        {
            if (playerManager.CurrentPlayerState == PlayerManager.State.Rolling && IsLookingTowardsWall())
            {
                playerManager.CurrentPlayerState = PlayerManager.State.Idle;
                RollEnd();
            }
        }

        private bool IsLookingTowardsWall()
        {
            RaycastHit hit;
            float checkDistance = 4f;

            if (Physics.Raycast(transform.position, rollDirection, out hit, checkDistance))
            {
                string layerName = LayerMask.LayerToName(hit.transform.gameObject.layer);
                return (layerName == "Wall" || layerName == "Obstacle") && hit.distance < 0.5f;
            }

            return false;
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

        private Vector3 GetRollDirection()
        {
            // Use the mouseInputPosition from the MouseInput script
            Vector3 mouseWorldPosition = playerManager.mouseInput.mouseInputPosition;

            if (mouseWorldPosition == transform.position) mouseWorldPosition = transform.forward;

            // Calculate the direction from the player to the mouse position
            Vector3 direction = (mouseWorldPosition - transform.position).normalized;
            direction.y = 0;

            return direction;
        }

        public void RollStart()
        {
            playerManager.ClearAttack();
            rollTimer = 0;

            // Store the initial Y position
            initialYPosition = transform.position.y;

            rollDirection = GetRollDirection();

            // Disable NavMeshAgent and enable root motion for rolling
            if (playerManager.Agent.enabled)
            {
                playerManager.Agent.isStopped = true;
                playerManager.Agent.enabled = false;
            }

            // Lock rotation to prevent unwanted turning and face the roll direction
            playerManager.transform.rotation = Quaternion.LookRotation(rollDirection);

            // Trigger the roll animation
            playerManager.CharacterAnimator.SetTrigger(rollTrigger);
        }

        public void RollUpdate()
        {
            if (playerManager.IsAnimationPlaying(rollTrigger))
            {
                rollTimer = playerManager.CharacterAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                if (rollTimer >= 0.6f)
                {
                    if (hasBufferedRoll || hasBufferedMovement)
                    {
                        ProcessBufferedInput();
                        return;
                    }

                    if (hasBufferedAttack && rollTimer >= 0.7f)
                    {
                        ProcessBufferedInput();
                        return;
                    }
                }
                return;
            }

            ProcessBufferedInput();
        }

        private void RollEnd()
        {
            // Re-enable NavMeshAgent and disable root motion after the roll
            playerManager.Agent.enabled = true;
            playerManager.Agent.isStopped = false;

            // Update the NavMeshAgent's position to prevent snapping
            Vector3 currentPosition = transform.position;
            playerManager.Agent.Warp(currentPosition);

            rollTimer = 0;
        }

        private void OnAnimatorMove()
        {
            if (playerManager.CurrentPlayerState != PlayerManager.State.Rolling) return;

            Vector3 newPosition = transform.position + playerManager.CharacterAnimator.deltaPosition;

            // Get ground height difference
            float groundAdjustment = DistanceToGround();

            // Adjust Y position based on ground height
            float targetY = newPosition.y - groundAdjustment;

            newPosition.y = Mathf.Lerp(transform.position.y, targetY, 0.5f);

            transform.position = newPosition;

            // Maintain roll direction
            playerManager.transform.rotation = Quaternion.LookRotation(rollDirection);
        }



        public float DistanceToGround()
        {
            RaycastHit hit;
            float raycastLength = 2f;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

            if (Physics.Raycast(rayOrigin, Vector3.down, out hit, raycastLength, groundLayer))
            {
                return transform.position.y - hit.point.y;
            }

            return 0f;
        }
    }
}
