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
        InputAction[] attacks;  // Array to store all attack actions
        InputAction openInventory;

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

            // Array for all attacks
            attacks = new InputAction[] {
            playerInputSystem.Player.Attack1,
            playerInputSystem.Player.Attack2
        };

            // Initialize the open inventory action
            openInventory = playerInputSystem.UI.OpenInventory;
            openInventory.Enable();
            openInventory.performed += OpenInventory;

            // Log bindings to confirm
            for (int i = 0; i < attacks.Length; i++)
            {
                Debug.Log($"Attack {i + 1} binding: " + attacks[i].bindings[0].ToString());
            }

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
            for (int i = 0; i < attacks.Length; i++)
            {
                attacks[i].Disable();
            }
        }

        private void Move(InputAction.CallbackContext context)
        {
            // Move the player based on mouse input position
            if (PlayerManager.Instance.CurrentPlayerState != PlayerManager.State.Inventory)
            {
                if (PlayerManager.Instance.mouseInput.hit.transform != null)
                {
                    // Try to get the Interactable interface from the hit object
                    var interactable = PlayerManager.Instance.mouseInput.hit.transform.GetComponent<Interactable>();

                    if (interactable != null)
                    {
                        // Set destination to the interactable object's position
                        Transform location = PlayerManager.Instance.mouseInput.hit.transform;
                        playerMovement.SetDestination(location.position);

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


        private void Attack(InputAction.CallbackContext context, int attackIndex)
        {
            // Call the PlayerManager's attack method with the correct index
            PlayerManager.Instance.Attack(attackIndex);
        }


        private void OpenInventory(InputAction.CallbackContext context)
        {
            if (PlayerManager.Instance.CurrentPlayerState != PlayerManager.State.Inventory)
            {
                // Show inventory
                PlayerManager.Instance.inventory = Popup.Create<Inventory>();
                PlayerManager.Instance.CurrentPlayerState = PlayerManager.State.Inventory;
            }
            else
            {
                // Hide inventory
                PlayerManager.Instance.inventory.OnCancel();
                PlayerManager.Instance.CurrentPlayerState = PlayerManager.State.Idle;
            }
        }
    }


}
