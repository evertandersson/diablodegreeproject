using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [System.Serializable]
    public class InventorySaveData
    {
        public List<ItemSaveData> inventoryItems;
        public List<ActionSlotSaveData> actionSlots;
    }

    [System.Serializable]
    public class ItemSaveData
    {
        public string itemName;
        public int quantity;
    }

    [System.Serializable]
    public class ActionSlotSaveData
    {
        public string itemName;
        public UpgradeSaveData upgradeData;
    }

    [System.Serializable]
    public class UpgradeSaveData
    {
        public float damageBonus;
        public float cooldownReduction;
        public int extraFireballs;
        public bool followEnemies;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Save the game (including both inventory and action slots)
    public void SaveGame(string saveFileName, List<InventorySlot> inventory, List<ActionSlot> actionSlots)
    {
        InventorySaveData saveData = new InventorySaveData
        {
            inventoryItems = new List<ItemSaveData>(),
            actionSlots = new List<ActionSlotSaveData>()
        };

        // Save inventory items
        foreach (var slot in inventory)
        {
            // Only save the item if it's not null
            if (slot.item != null)
            {
                saveData.inventoryItems.Add(new ItemSaveData
                {
                    itemName = slot.item.itemName,
                    quantity = slot.itemAmount
                });
            }
            else
            {
                saveData.inventoryItems.Add(null);
            }
        }

        // Save action slots
        foreach (var slot in actionSlots)
        {
            if (slot.item != null && slot.item is AttackTypeSO attackType)
            {
                // Ensure attackInstance is not null
                if (attackType.attackInstance != null)
                {
                    saveData.actionSlots.Add(new ActionSlotSaveData
                    {
                        itemName = attackType.itemName,
                        upgradeData = new UpgradeSaveData
                        {
                            damageBonus = attackType.attackInstance.currentDamage,
                            cooldownReduction = attackType.attackInstance.currentCooldown,
                            extraFireballs = attackType.attackInstance.currentProjectileCount,
                            followEnemies = attackType.attackInstance.currentFollowEnemies
                        }
                    });
                }
                
            }
            
        }


        // Serialize and save the data to a file
        string json = JsonUtility.ToJson(saveData, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/" + saveFileName, json);
    }

    // Load saved data
    public InventorySaveData LoadSavedData(string saveFileName)
    {
        string path = Application.persistentDataPath + "/" + saveFileName;
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            return JsonUtility.FromJson<InventorySaveData>(json);
        }
        return null;
    }
}
