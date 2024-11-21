using System.Collections.Generic;
using UnityEngine;
using Game;

public class Inventory : Popup
{
    [SerializeField]
    public List<InventorySlot> inventory = new List<InventorySlot>();

    [SerializeField]
    private SlotManager slotManager;

    [SerializeField] private Transform inventoryPanel;  
    [SerializeField] private Transform actionPanel;     

    private void Awake()
    {
        SetUpInventory();   
    }


    private void SetUpInventory()
    {
        inventory = new List<InventorySlot>();

        // Add all inventory slots from the inventory panel
        inventory.AddRange(inventoryPanel.GetComponentsInChildren<InventorySlot>());

        slotManager.GetActionSlots();

        InventorySaver.Instance.Load();
    }

    public bool AddItemToInventory(ItemSO item)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == null)
            {
                inventory[i].item = item;
                inventory[i].artwork.texture = item.itemIcon;
                inventory[i].itemAmount++;
                inventory[i].UpdateItemAmountText();
                Debug.Log("Item added succesfully");
                return true; // Item added successfully
            }
            else if (inventory[i].item.isStackable && inventory[i].item.itemName == item.itemName)
            {
                inventory[i].itemAmount++;
                inventory[i].UpdateItemAmountText();
                Debug.Log("Item stacked succesfully");
                return true; // Item stacked successfully
            }
        }
        Debug.Log("Inventory full");
        return false; // Inventory is full
    }
}
