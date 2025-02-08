using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerInput : MonoBehaviour
    {
        PlayerMovement playerMovement;

        [SerializeField]
        private CanvasGroup inventoryCanvasGroup;

        PlayerInputSystem playerInputSystem;

        InputAction move;
        InputAction roll;
        InputAction[] attacks;  // Array to store all attack actions
        InputAction openInventory;
        InputAction openSkillTree;
        InputAction highlightItems;

        bool isMoving = false;
        bool isClickInteraction = false;
        float distanceToWall;

        public LayerMask layerMask;

        public static event Action HighlightItems;
        public static event Action HideItems;

        private void Awake()
        {
            playerInputSystem = new PlayerInputSystem();
            playerMovement = GetComponent<PlayerMovement>();
        }

        private void OnEnable()
        {
            // Initialize the move action
            move = playerInputSystem.Player.Move;
            move.Enable();
            move.performed += StartMoving;
            move.canceled += StopMoving;

            roll = playerInputSystem.Player.Roll;
            roll.Enable();
            roll.performed += Roll;

            // Array for all attacks
            attacks = new InputAction[] {
                playerInputSystem.Player.Attack1,
                playerInputSystem.Player.Attack2,
                playerInputSystem.Player.Attack3,
                playerInputSystem.Player.Attack4,
                playerInputSystem.Player.Attack5
            };

            // Initialize the open inventory action
            openInventory = playerInputSystem.UI.OpenInventory;
            openInventory.Enable();
            openInventory.performed += OpenInventory;

            openSkillTree = playerInputSystem.UI.OpenSkillTree;
            openSkillTree.Enable();
            openSkillTree.performed += OpenSkillTree;

            // Initialize the highlight items action
            highlightItems = playerInputSystem.Player.HighlightItems;
            highlightItems.Enable();
            highlightItems.performed += ShowHighlightItems;
            highlightItems.canceled += HideHighlightItems;

            // Subscribe to each attack action
            for (int i = 0; i < attacks.Length; i++)
            {
                attacks[i].Enable();
                int index = i;
                attacks[i].performed += ctx => Attack(ctx, index);
            }
        }


        private void OnDisable()
        {
            move.Disable();
            roll.Disable();
            openInventory.Disable();
            openSkillTree.Disable();
            highlightItems.Disable();
            for (int i = 0; i < attacks.Length; i++)
            {
                attacks[i].Disable();
            }
        }

        private float GetCurrentTimer()
        {
            if (PlayerManager.Instance.IsAttacking)
                return PlayerManager.Instance.attackTimer;
            else if (PlayerManager.Instance.IsRolling)
                return playerMovement.rollTimer;
            else
                return 0;
        }

        private float GetWaitForNextBufferedInputTimer()
        {
            if (PlayerManager.Instance.IsAttacking)
            {
                AttackTypeSO attack = PlayerManager.Instance.CurrentAction as AttackTypeSO;
                float bufferedInput = StatsCalculator.CalculateAttackSpeed(attack.bufferedInputDelay);
                return bufferedInput;
            }
            else if (PlayerManager.Instance.IsRolling)
                return 0.2f;
            else
                return 0;
        }

        #region Movement

        private void StartMoving(InputAction.CallbackContext context)
        {
            if (CanNotDoAction()) return;

            isMoving = true;
            isClickInteraction = true;
        }

        private void StopMoving(InputAction.CallbackContext context) 
        {
            isMoving = false;
            isClickInteraction = false;
        }

        public bool IsMoving()
        {
            return isMoving;
        }

        public void Move()
        {
            if (isClickInteraction)
            {
                HandleInteraction();
                isClickInteraction = false;
                return;
            }

            if (playerMovement.ReadyForAnotherInput(GetCurrentTimer(), GetWaitForNextBufferedInputTimer()))
            {
                Vector3 targetPosition = PlayerManager.Instance.mouseInput.mouseInputPosition;

                if (NavMesh.SamplePosition(targetPosition, out NavMeshHit navHit, 1.0f, NavMesh.AllAreas))
                {
                    playerMovement.BufferInput(navHit.position);
                }
                return;
            }

            if (PlayerManager.Instance.CurrentPlayerState != PlayerManager.State.Inventory && !PlayerManager.Instance.isInteracting)
            {
                NavMeshAgent navMeshAgent = PlayerManager.Instance.Agent;
                if (navMeshAgent == null || !navMeshAgent.enabled)
                    return;

                Vector3 targetPosition = PlayerManager.Instance.mouseInput.mouseInputPosition;
                float distance = Vector3.Distance(PlayerManager.Instance.transform.position, targetPosition);
                Vector3 direction = (targetPosition - transform.position).normalized;
                direction.y = 0;

                // Define the angle for the offset
                float angleOffset = 45f;

                // Calculate new directions rotated 45 degrees relative to the mouse direction
                Vector3 leftDirection = Quaternion.AngleAxis(-angleOffset, Vector3.up) * direction;
                Vector3 rightDirection = Quaternion.AngleAxis(angleOffset, Vector3.up) * direction;

                // Perform raycasts at 45-degree angles in front of the mouse direction
                RaycastHit leftHit, rightHit;
                bool leftBlocked = Physics.Raycast(transform.position, leftDirection, out leftHit, 2.4f, layerMask);
                bool rightBlocked = Physics.Raycast(transform.position, rightDirection, out rightHit, 2.4f, layerMask);

                // Debug visualization
                Debug.DrawRay(transform.position, leftDirection * 2.4f, leftBlocked ? Color.red : Color.green);
                Debug.DrawRay(transform.position, rightDirection * 2.4f, rightBlocked ? Color.red : Color.green);

                float leftDistance = leftBlocked ? leftHit.distance : distance;
                float rightDistance = rightBlocked ? rightHit.distance : distance;

                float maxWallCheckDistance = 2.4f;

                // Default values for offsets
                float leftOffsetDistance = 1.2f;
                float rightOffsetDistance = 1.2f;

                // Adjust only the closest side if necessary
                if (leftDistance < maxWallCheckDistance || rightDistance < maxWallCheckDistance)
                {
                    float leftNormalized = Mathf.Clamp01(leftDistance / maxWallCheckDistance);
                    leftOffsetDistance = Mathf.Lerp(-0.8f, 1.2f, leftNormalized);
                    float rightNormalized = Mathf.Clamp01(rightDistance / maxWallCheckDistance);
                    rightOffsetDistance = Mathf.Lerp(-0.8f, 1.2f, rightNormalized);
                }


                // Adjust perpendicular offsets based on distance to walls
                Vector3 leftOffset = new Vector3(-direction.z, 0f, direction.x) * leftOffsetDistance;
                Vector3 rightOffset = new Vector3(direction.z, 0f, -direction.x) * rightOffsetDistance;

                // Move the raycast positions slightly behind the player
                Vector3 backwardOffset = -direction; // Adjust this value to move further back

                // Maintain a consistent height for both origins
                float fixedHeight = transform.position.y + 1.2f;

                // Base positions for raycast origins
                Vector3 baseLeftOrigin = transform.position + leftOffset + backwardOffset;
                Vector3 baseRightOrigin = transform.position + rightOffset + backwardOffset;

                // Apply height adjustment to prevent stuttering
                Vector3 leftOrigin = new Vector3(baseLeftOrigin.x, fixedHeight, baseLeftOrigin.z);
                Vector3 rightOrigin = new Vector3(baseRightOrigin.x, fixedHeight, baseRightOrigin.z);

                // Perform SphereCasts from the new positions
                bool leftWallBlocked = Physics.SphereCast(leftOrigin, 0.2f, direction, out RaycastHit leftHit2, distance, layerMask);
                bool rightWallBlocked = Physics.SphereCast(rightOrigin, 0.2f, direction, out RaycastHit rightHit2, distance, layerMask);

                // Draw debug rays to visualize
                Debug.DrawRay(leftOrigin, direction * (leftWallBlocked ? leftHit2.distance : distance), leftWallBlocked ? Color.red : Color.green);
                Debug.DrawRay(rightOrigin, direction * (rightWallBlocked ? rightHit2.distance : distance), rightWallBlocked ? Color.red : Color.green);


                Vector3 finalDestination = targetPosition;

                if (leftWallBlocked && rightWallBlocked)
                {
                    if (Physics.Raycast(transform.position, direction, out RaycastHit hit2, distance, layerMask))
                    {
                        finalDestination = hit2.point;
                    }
                }

                if (NavMesh.SamplePosition(finalDestination, out NavMeshHit validHit2, 1.0f, NavMesh.AllAreas))
                {
                    NavMeshPath path = new NavMeshPath();
                    if (navMeshAgent.CalculatePath(validHit2.position, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        playerMovement.SetDestination(validHit2.position);
                    }
                }
            }
        }


        void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (PlayerManager.Instance == null || PlayerManager.Instance.Agent == null)
                return;

            NavMeshAgent agent = PlayerManager.Instance.Agent;

            // Ensure there's a path to draw
            if (!agent.hasPath || agent.path.corners.Length < 2)
                return;

            Gizmos.color = Color.green; // Path color
            Vector3[] pathCorners = agent.path.corners;

            // Draw lines between path corners
            for (int i = 0; i < pathCorners.Length - 1; i++)
            {
                Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
            }
        }


        private void HandleInteraction()
        {
            if (PlayerManager.Instance.mouseInput.hit.transform != null)
            {
                var interactable = PlayerManager.Instance.mouseInput.hit.transform.GetComponent<Interactable>();

                if (interactable != null)
                {
                    // Click detected on an interactable
                    playerMovement.SetDestination(interactable.GetCenterPoint());
                    PlayerManager.Instance.SetCurrentObject(interactable);
                }
            }
        }

        #endregion 

        private void Roll(InputAction.CallbackContext context)
        {
            if (CanNotDoAction()) return;

            // Start rolling only if the player is not already rolling
            if (playerMovement.ReadyForAnotherInput(GetCurrentTimer(), GetWaitForNextBufferedInputTimer()))
            {
                playerMovement.BufferRoll();
            }
            else if (!PlayerManager.Instance.isInteracting)
            {
                PlayerManager.Instance.CurrentPlayerState = PlayerManager.State.Rolling;
            }
        }

        private void Attack(InputAction.CallbackContext context, int attackIndex)
        {
            if (CanNotDoAction()) return;

            // Allow attacking during idle or buffered for after rolling
            if (playerMovement.ReadyForAnotherInput(GetCurrentTimer(), GetWaitForNextBufferedInputTimer()))
            {
                playerMovement.BufferAttack(attackIndex);
                return;
            }
            else if (!PlayerManager.Instance.isInteracting)
            {
                PlayerManager.Instance.Attack(attackIndex);
            }
        }

        private void OpenInventory(InputAction.CallbackContext context)
        {
            if (CanNotOpenMenu()) return;
            TogglePopup(ref PlayerManager.Instance.inventory, PlayerManager.State.Inventory);
        }

        private void OpenSkillTree(InputAction.CallbackContext context)
        {
            if (CanNotOpenMenu()) return;
            TogglePopup(ref PlayerManager.Instance.skillTree, PlayerManager.State.Inventory);
        }

        private bool CanNotDoAction()
        {
            return PlayerManager.Instance.CurrentPlayerState == PlayerManager.State.Dead ||
                PlayerManager.Instance.CurrentPlayerState == PlayerManager.State.Inventory;
        }

        private bool CanNotOpenMenu()
        {
            return PlayerManager.Instance.CurrentPlayerState == PlayerManager.State.Dead ||
                PlayerManager.Instance.CurrentPlayerState == PlayerManager.State.Rolling;
        }

        private void TogglePopup<T>(ref T popupInstance, PlayerManager.State activeState) where T : Popup
        {
            if (EventHandler.Main.CurrentEvent is not T)
            {
                // Open the popup
                popupInstance = Popup.Create<T>();
                PlayerManager.Instance.CurrentPlayerState = activeState;
            }
            else
            {
                // Close the popup
                if (popupInstance != null)
                {
                    popupInstance.OnCancel();
                }
            }
        }

        private void ShowHighlightItems(InputAction.CallbackContext context)
        {
            HighlightItems?.Invoke();
        }
        private void HideHighlightItems(InputAction.CallbackContext context)
        {
            HideItems?.Invoke();
        }
    }
}
