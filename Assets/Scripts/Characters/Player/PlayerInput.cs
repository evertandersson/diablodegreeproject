using UnityEngine;
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
            move.performed += Move;

            roll = playerInputSystem.Player.Roll;
            roll.Enable();
            roll.performed += Roll;

            // Array for all attacks
            attacks = new InputAction[] {
                playerInputSystem.Player.Attack1,
                playerInputSystem.Player.Attack2
            };

            // Initialize the open inventory action
            openInventory = playerInputSystem.UI.OpenInventory;
            openInventory.Enable();
            openInventory.performed += OpenInventory;

            openSkillTree = playerInputSystem.UI.OpenSkillTree;
            openSkillTree.Enable();
            openSkillTree.performed += OpenSkillTree;

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
                return attack.bufferedInputDelay;
            }
            else if (PlayerManager.Instance.IsRolling)
                return 0.2f;
            else
                return 0;
        }

        private void Move(InputAction.CallbackContext context)
        {
            if (PlayerManager.Instance.CurrentPlayerState == PlayerManager.State.Dead) return;

            // Handle movement input
            if (playerMovement.ReadyForAnotherInput(GetCurrentTimer(), GetWaitForNextBufferedInputTimer()))
            {
                // Buffer the movement input during a roll
                playerMovement.BufferInput(PlayerManager.Instance.mouseInput.mouseInputPosition);
                return;
            }

            if (PlayerManager.Instance.CurrentPlayerState != PlayerManager.State.Inventory && !PlayerManager.Instance.isInteracting)
            {
                if (PlayerManager.Instance.mouseInput.hit.transform != null)
                {
                    // Try to get the Interactable interface from the hit object
                    var interactable = PlayerManager.Instance.mouseInput.hit.transform.GetComponent<Interactable>();

                    if (interactable != null)
                    {
                        playerMovement.SetDestination(interactable.GetCenterPoint());

                        // Set the current object in the PlayerManager
                        PlayerManager.Instance.SetCurrentObject(interactable);
                    }
                    else
                    {
                        // Move to the clicked position if not interactable
                        playerMovement.SetDestination(PlayerManager.Instance.mouseInput.mouseInputPosition);
                    }
                }
                else
                {
                    // Default movement to the mouse input position
                    playerMovement.SetDestination(PlayerManager.Instance.mouseInput.mouseInputPosition);
                }
            }
        }

        private void Roll(InputAction.CallbackContext context)
        {
            if (PlayerManager.Instance.CurrentPlayerState == PlayerManager.State.Dead) return;

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
            if (PlayerManager.Instance.CurrentPlayerState == PlayerManager.State.Dead) return;

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
            if (PlayerManager.Instance.CurrentPlayerState == PlayerManager.State.Dead) return;
            TogglePopup(ref PlayerManager.Instance.inventory, PlayerManager.State.Inventory);
        }

        private void OpenSkillTree(InputAction.CallbackContext context)
        {
            if (PlayerManager.Instance.CurrentPlayerState == PlayerManager.State.Dead) return;
            TogglePopup(ref PlayerManager.Instance.skillTree, PlayerManager.State.Inventory);
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
                // Do NOT immediately set popupInstance to null
                // Let Popup.OnEnd() handle cleanup
            }
        }
    }
}
