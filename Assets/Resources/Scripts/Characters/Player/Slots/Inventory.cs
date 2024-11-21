using Game;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Popup
{
    [SerializeField] private List<InventorySlot> inventory = new List<InventorySlot>();
    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private Transform actionPanel;
    [SerializeField] private SlotManager slotManager;

    private void Awake()
    {
        PreloadItems();  // Preload all items at the start of the game
        SetUpInventory();  // Ensure inventory is initialized before loading data
        LoadGame("savefile.json");  // Load saved game data after initialization
    }

    public void PreloadItems()
    {
        // Assume you have a list of all items in your game
        var allItems = Resources.LoadAll<ItemSO>("Scripts/Characters/Items/Attacks/AttackTypesSO/");

        foreach (var item in allItems)
        {
            Debug.Log("Preloaded item: " + item.itemName);  // Log each item to ensure they're loaded
        }
    }
    private void SetUpInventory()
    {
        inventory.Clear();  // Clear the existing list to avoid duplication
        inventory.AddRange(inventoryPanel.GetComponentsInChildren<InventorySlot>());  // Add all inventory slots

        // Log the number of inventory slots and check each one for existing items
        Debug.Log("Setting up inventory with the following slots:");
        foreach (var slot in inventory)
        {
            if (slot != null && slot.item != null)
                Debug.Log("Inventory contains: " + slot.item.itemName);
            else
                Debug.Log("Inventory slot is empty.");
        }
    }

    public bool AddItemToInventory(ItemSO item, int quantity)
    {
        // Log the item being added
        Debug.Log("Attempting to add item: " + item.itemName);

        // Loop through the inventory slots to find an empty slot or an existing stackable item
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].item == null)  // If the slot is empty
            {
                inventory[i].item = item;
                inventory[i].artwork.texture = item.itemIcon;
                inventory[i].itemAmount = quantity;
                inventory[i].UpdateItemAmountText();
                Debug.Log("Item added successfully: " + item.itemName);
                return true;
            }
            else if (inventory[i].item.isStackable && inventory[i].item.itemName == item.itemName)  // If the item is stackable
            {
                inventory[i].itemAmount += quantity;
                inventory[i].UpdateItemAmountText();
                Debug.Log("Item stacked successfully: " + item.itemName);
                return true;
            }
        }

        // If no suitable slot is found
        Debug.Log("Inventory full. Could not add: " + item.itemName);
        return false;
    }

    // Call to save the game data (both inventory and action slots)
    public void SaveGame(string saveFileName)
    {
        SaveManager.Instance.SaveGame(saveFileName, inventory, slotManager.actionSlots);
    }

    public void LoadGame(string saveFileName)
    {
        var savedData = SaveManager.Instance.LoadSavedData(saveFileName);

        // Check if saved data is valid
        if (savedData != null)
        {
            Debug.Log("Loading game data...");
            // Load inventory items from saved data
            for (int i = 0; i < savedData.inventoryItems.Count; i++)
            {
                var itemData = savedData.inventoryItems[i];
                Debug.Log("Looking for item: " + itemData.itemName);  // Log item name for debugging
                ItemSO item = FindItemSOByName(itemData.itemName);  // Try to find the item by its name
                if (item != null)
                {
                    AddItemToInventory(item, itemData.quantity);  // Add item with its quantity
                }
                else
                {
                    Debug.LogWarning("Item not found in inventory: " + itemData.itemName);
                }
            }
        }
        else
        {
            Debug.LogError("Failed to load saved data.");
        }
    }

    public ItemSO FindItemSOByName(string name)
    {
        // Search through the inventory list for an ItemSO with the matching name
        foreach (var slot in inventory)
        {
            // Check if the slot is not null and if the item is not null
            if (slot != null && slot.item != null)
            {
                if (slot.item.itemName == name)
                {
                    return slot.item;  // Return the item if found
                }
            }
        }

        Debug.LogWarning("Item not found: " + name);  // Log a warning if item isn't found
        return null;
    }

}
