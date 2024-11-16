using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    List<InventorySlot> inventory = new List<InventorySlot>();

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

        // Add all action slots from the action panel
        //inventory.AddRange(actionPanel.GetComponentsInChildren<InventorySlot>());
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
