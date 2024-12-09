using UnityEngine;

namespace Game
{
    public class ItemWorld : MonoBehaviour, Interactable
    {
        [SerializeField]
        private ItemSO item;

        public void Trigger()
        {
            // Try to get the Inventory component on the player or its child objects
            Inventory playerInventory = PlayerManager.Instance.GetComponentInChildren<Inventory>(true);

            if (playerInventory != null)
            {
                bool itemAdded = playerInventory.AddItemToInventory(item);

                if (itemAdded)
                {
                    Destroy(gameObject); // Destroy item after adding it to inventory
                }
            }
            else
            {
                Debug.LogError("Inventory component not found on player!");
            }
            
        }
    }

}
