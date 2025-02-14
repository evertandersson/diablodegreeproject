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
        private Vector3 offset = new Vector3(0, 1.2f, 0);

        // Buffer variables:
        private Vector3 bufferedDestination;
        public bool hasBufferedMovement;
        private int bufferedAttackIndex;
        public bool hasBufferedAttack;
        public bool hasBufferedRoll;
        [SerializeField] private LayerMask groundLayer;

        public bool hasBufferedInput => hasBufferedAttack || hasBufferedRoll || hasBufferedMovement;

        // Animation names:
        public int rollTrigger = Animator.StringToHash("Roll");

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
            if (hasBufferedAttack)
            {
                playerManager.CurrentPlayerState = PlayerManager.State.Idle;
                RollEnd();
                PlayerManager.Instance.Attack(bufferedAttackIndex);
                return;
            }
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
                RollEnd();
                SetDestination(bufferedDestination);
                return;
            }
        }

        public void ResetBufferedInput(bool resetTimers = true)
        {
            hasBufferedMovement = false;
            hasBufferedAttack = false;
            hasBufferedRoll = false;
            if (resetTimers)
            {
                rollTimer = 0;
                playerManager.attackTimer = 0;
            }
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

            rollDirection = GetRollDirection();

            if (playerManager.Agent.enabled) playerManager.Agent.isStopped = true;

            playerManager.transform.rotation = Quaternion.LookRotation(rollDirection);
            playerManager.CharacterAnimator.SetTrigger(rollTrigger);
        }

        public void RollUpdate()
        {
            playerManager.Agent.Warp(transform.position);

            rollTimer += Time.deltaTime;

            if (rollTimer > 0.4f)
            {
                if (hasBufferedRoll || hasBufferedMovement)
                {
                    ProcessBufferedInput();
                    return;
                }

                if (hasBufferedAttack && rollTimer > 0.5f)
                {
                    ProcessBufferedInput();
                    return;
                }

                if (!playerManager.IsAnimationPlayingStrict(rollTrigger))
                {
                    RollEnd();
                    playerManager.CurrentPlayerState = PlayerManager.State.Idle;
                    ResetBufferedInput();
                }
            }
        }



        private void RollEnd()
        {
            // Re-enable NavMeshAgent and disable root motion after the roll
            playerManager.Agent.isStopped = false;
            rollTimer = 0;
        }

        private void OnAnimatorMove()
        {
            if (playerManager.CurrentPlayerState != PlayerManager.State.Rolling) return;

            // Apply animator movement (root motion)
            Vector3 movement = playerManager.CharacterAnimator.deltaPosition;

            // Add additional forward force to ensure movement
            movement += rollDirection * playerManager.rollSpeed * Time.deltaTime;

            transform.position += movement;

            // Ensure rotation follows roll direction
            playerManager.transform.rotation = Quaternion.LookRotation(rollDirection);
        }

    }
}
