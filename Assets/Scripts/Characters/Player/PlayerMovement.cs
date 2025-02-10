using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class PlayerMovement : MonoBehaviour
    {
        PlayerManager playerManager;
        Rigidbody rb;

        public float rollTimer = 0;
        public float idleTimer;
        private Vector3 rollDirection; // To store the roll direction
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
        }

        public void HandleSwitchToIdle()
        {
            idleTimer += Time.deltaTime;

            if (playerManager.CurrentPlayerState == PlayerManager.State.Rolling && idleTimer > 1)
            {
                playerManager.CurrentPlayerState = PlayerManager.State.Idle;
                RollEnd();
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
            idleTimer = 0;

            rollDirection = GetRollDirection();

            // Disable NavMeshAgent and enable root motion for rolling
            if (playerManager.Agent.enabled) playerManager.Agent.isStopped = true;

            // Lock rotation to prevent unwanted turning and face the roll direction
            playerManager.transform.rotation = Quaternion.LookRotation(rollDirection);

            // Trigger the roll animation
            playerManager.CharacterAnimator.SetTrigger(rollTrigger);
        }

        public void RollUpdate()
        {
            playerManager.Agent.Warp(transform.position);

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
            return;

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

            transform.position = transform.position + playerManager.CharacterAnimator.deltaPosition;

            playerManager.transform.rotation = Quaternion.LookRotation(rollDirection);
        }
    }
}
