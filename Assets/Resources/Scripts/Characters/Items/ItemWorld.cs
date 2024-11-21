using UnityEngine;

public class ItemWorld : MonoBehaviour
{
    [SerializeField]
    private ItemSO item;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerManager playerManager))
        {
            // Try to get the Inventory component on the player or its child objects
            Inventory playerInventory = playerManager.GetComponentInChildren<Inventory>();

            if (playerInventory != null)
            {
                bool itemAdded = playerInventory.AddItemToInventory(item, 1);

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
