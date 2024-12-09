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
                    if (PlayerManager.Instance.mouseInput.hit.transform.CompareTag("Door"))
                    {
                        Door door = PlayerManager.Instance.mouseInput.hit.transform.GetComponent<Door>();
                        playerMovement.SetDestination(door.transform.position);

                        PlayerManager.Instance.SetCurrentDoor(door);
                    }
                    else
                    {
                        playerMovement.SetDestination(PlayerManager.Instance.mouseInput.mouseInputPosition);
                    }
                }
                else
                {
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
