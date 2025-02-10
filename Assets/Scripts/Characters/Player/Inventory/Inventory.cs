using System.Collections.Generic;
using UnityEngine;
using Game;

public class Inventory : Popup
{
    [SerializeField]
    public List<InventorySlot> inventory = new List<InventorySlot>();

    [SerializeField] private Transform inventoryPanel;  
    [SerializeField] private Transform actionPanel;     

    private void Start()
    {
        SetUpInventory();
    }


    private void SetUpInventory()
    {
        inventory = new List<InventorySlot>();

        // Add only slots tagged as "InventorySlot"
        foreach (var slot in inventoryPanel.GetComponentsInChildren<InventorySlot>())
        {
            if (slot is not EquipmentSlot)
            {
                inventory.Add(slot);
            }
        }

        PlayerManager.Instance.slotManager.GetActionSlots();
        PlayerManager.Instance.slotManager.SetUpSlots();

        SaveManager.Instance.Load();
    }

    public bool AddItemToInventory(ItemSO item)
    {
        ActionSlot[] actionSlots = PlayerManager.Instance.slotManager.actionSlots;
        for (int i = 0; i < actionSlots.Length; i++)
        {
            if (actionSlots[i].item != null)
            {
                if (actionSlots[i].item.itemName == item.itemName && actionSlots[i].item.isStackable)
                {
                    actionSlots[i].itemAmount++;
                    actionSlots[i].UpdateItemAmountText();
                    Debug.Log("Item stacked succesfully");
                    return true; // Item stacked successfully
                }
            }
        }

        InventorySlot inventorySlot = null;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == null)
            {
                if (inventorySlot == null)
                    inventorySlot = inventory[i];
            }
            else if (inventory[i].item.isStackable && inventory[i].item.itemName == item.itemName)
            {
                inventory[i].itemAmount++;
                inventory[i].UpdateItemAmountText();
                Debug.Log("Item stacked succesfully");
                return true; // Item stacked successfully
            }
        }

        if (inventorySlot != null)
        {
            inventorySlot.item = item;
            inventorySlot.artwork.texture = item.itemIcon;
            inventorySlot.itemAmount++;
            inventorySlot.UpdateItemAmountText();
            Debug.Log("Item added succesfully");
            return true; // Item added successfully
        }

        Debug.Log("Inventory full");
        return false; // Inventory is full
    }
}
