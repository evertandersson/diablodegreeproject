using UnityEngine;

namespace Game
{
    public class ItemWorld : MonoBehaviour, Interactable
    {
        [SerializeField]
        private ItemSO item;

        [SerializeField] private string id;

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid()
        {
            id = System.Guid.NewGuid().ToString();
        }

        [SerializeField]
        private string message;

        private void Awake()
        {
            SaveManager.LoadPickedUpItems += Load;
        }

        private void OnDisable()
        {
            SaveManager.LoadPickedUpItems -= Load;
        }

        private void Load()
        {
            if (SaveManager.Instance.pickedUpItemsList.serializableList.Exists(item => item.name == id))
            {
                gameObject.SetActive(false); // Hide the item
                Debug.Log($"Hiding item with ID {id} as it has already been picked up.");
            }
        }

        public void Trigger()
        {
            // Try to get the Inventory component on the player or its child objects
            Inventory playerInventory = PlayerManager.Instance.GetComponentInChildren<Inventory>(true);

            if (playerInventory != null)
            {
                bool itemAdded = playerInventory.AddItemToInventory(item);

                if (itemAdded)
                {
                    SaveManager.Instance.AddPickedUpItem(id);

                    Vector3 offset = new Vector3(0, 1, 0);
                    PopupText text = ObjectPooling.Instance.SpawnFromPool("PopupText", transform.position + offset, Quaternion.identity).GetComponent<PopupText>();
                    text.message = message;
                    text.StartCoroutine("Trigger");
                    SoundManager.PlaySound(SoundType.PICKUP);
                    gameObject.SetActive(false); // Destroy item after adding it to inventory
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
