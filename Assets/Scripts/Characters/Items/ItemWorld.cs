using UnityEngine;

namespace Game
{
    public class ItemWorld : MonoBehaviour, Interactable
    {
        [SerializeField]
        private ItemSO item;

        [SerializeField]
        private string message;

        public void Trigger()
        {
            // Try to get the Inventory component on the player or its child objects
            Inventory playerInventory = PlayerManager.Instance.GetComponentInChildren<Inventory>(true);

            if (playerInventory != null)
            {
                bool itemAdded = playerInventory.AddItemToInventory(item);

                if (itemAdded)
                {
                    Vector3 offset = new Vector3(0, 1, 0);
                    PopupText text = ObjectPooling.Instance.SpawnFromPool("PopupText", transform.position + offset, Quaternion.identity).GetComponent<PopupText>();
                    text.message = message;
                    text.StartCoroutine("Trigger");
                    Destroy(gameObject); // Destroy item after adding it to inventory
                }
            }
            else
            {
                Debug.LogError("Inventory component not found on player!");
            }
            
        }

        public Vector3 GetCenterPoint()
        {
            return transform.position;
        }
    }

}
