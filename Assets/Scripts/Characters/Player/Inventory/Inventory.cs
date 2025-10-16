using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Inventory : Popup
    {
        [SerializeField] private Transform inventoryPanel;
        [SerializeField] private Transform actionPanel;

        public List<Slot> inventorySlots { get; private set; } = new();

        private void Start()
        {
            SetUpInventory();
        }

        private void SetUpInventory()
        {
            inventorySlots.Clear();

            foreach (var slot in inventoryPanel.GetComponentsInChildren<InventorySlot>())
            {
                inventorySlots.Add(slot);
            }
            foreach (var slot in actionPanel.GetComponentsInChildren<ActionSlot>())
            {
                inventorySlots.Add(slot);
            }

            foreach (var slot in inventorySlots)
            {
                slot.UpdateUI();
            }

  

            // Setup the other slot systems
            PlayerManager.Instance.actionSlotManager.GetActionSlots();
            PlayerManager.Instance.actionSlotManager.SetUpSlots();

            SaveManager.Instance.Load();
        }

        /// <summary>
        /// Tries to add an item to an empty slot or stack it if possible.
        /// </summary>
        public bool AddItemToInventory(ItemSO item)
        {
            // Try to stack with existing items first
            foreach (var slot in inventorySlots)
            {
                if (slot.item != null && slot.item.itemName == item.itemName && slot.item.isStackable)
                {
                    slot.itemAmount++;
                    slot.UpdateUI();
                    Debug.Log($"Stacked {item.itemName}");
                    return true;
                }
            }

            foreach (var slot in inventorySlots)
            {
                if (slot.item == null)
                {
                    slot.SetItem(item);
                    Debug.Log($"Added new item: {item.itemName}");
                    return true;
                }
            }

            Debug.LogWarning("Inventory full!");
            return false;
        }
    }
}
