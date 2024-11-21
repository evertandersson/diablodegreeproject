using Leguar.TotalJSON;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySaver : MonoBehaviour
{
    [SerializeField] private Inventory myInventory;
    [SerializeField] private SlotManager slotManager;
    [SerializeField] private ItemDataBaseSO itemDB;

    private SerializableListString inventoryList = new SerializableListString();
    private SerializableListString actionSlotList = new SerializableListString();

    private void Start()
    {
        inventoryList.serializableList.Clear();

        LoadScriptables();

        ImportSaveData();
    }

    private void OnDisable()
    {
        inventoryList.serializableList.Clear();

        BuildSaveData();

        SaveScriptables();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
            SaveReset();
    }

    public void ResetScriptables()
    {
        int i = 0;
        while (File.Exists(Application.persistentDataPath +
                string.Format("/{0}.inv", i)))
        {
            File.Delete(Application.persistentDataPath +
                string.Format("/{0}.inv", i));
            i++;
        }
        
    }

    private void BuildSaveData()
    {
        inventoryList.serializableList.Clear(); // Clear previous data
        actionSlotList.serializableList.Clear();

        SaveSlotsToSerializableList(myInventory.inventory, inventoryList);
        SaveSlotsToSerializableList(slotManager.actionSlots, actionSlotList);
    }

    private void SaveSlotsToSerializableList(IEnumerable<InventorySlot> slots, SerializableListString targetList)
    {
        foreach (var slot in slots)
        {
            if (slot != null && slot.item != null)
            {
                SerializableListString.SerialItem SI = new SerializableListString.SerialItem
                {
                    name = slot.item.name,
                    count = slot.itemAmount
                };
                targetList.serializableList.Add(SI);
            }
        }
    }


    public void SaveScriptables()
    {
        Debug.Log("Saving to: " + Application.persistentDataPath);

        string filepath = Application.persistentDataPath + "/inventory_save.json";
        string actionFilePath = Application.persistentDataPath + "/action_slots_save.json";

        // Save inventory
        string inventoryJson = JSON.Serialize(inventoryList).CreatePrettyString();
        File.WriteAllText(filepath, inventoryJson);

        // Save action slots
        string actionSlotJson = JSON.Serialize(actionSlotList).CreatePrettyString();
        File.WriteAllText(actionFilePath, actionSlotJson);

        Debug.Log("Save completed.");
    }

    private void ImportSaveData()
    {
        // Clear existing data for both inventories
        ClearSlots(myInventory.inventory);
        ClearSlots(slotManager.actionSlots);

        // Load data into inventory and action slots
        LoadSlots(myInventory.inventory, inventoryList);
        LoadSlots(slotManager.actionSlots, actionSlotList);
    }

    private void ClearSlots(IEnumerable<InventorySlot> slots)
    {
        foreach (var slot in slots)
        {
            slot.item = null;
            slot.itemAmount = 0;
        }
    }

    private void LoadSlots(IList<InventorySlot> slots, SerializableListString targetList)
    {
        for (int i = 0; i < targetList.serializableList.Count; i++)
        {
            string name = targetList.serializableList[i].name;
            int count = targetList.serializableList[i].count;

            ItemSO obj = itemDB.GetItem(name);
            if (obj && i < slots.Count)
            {
                slots[i].item = obj;
                slots[i].itemAmount = count;
            }
        }
    }



    public void LoadScriptables()
    {
        Debug.Log("Loading from: " + Application.persistentDataPath);

        string filepath = Application.persistentDataPath + "/inventory_save.json";
        string actionFilePath = Application.persistentDataPath + "/action_slots_save.json";

        // Load inventory
        if (File.Exists(filepath))
        {
            string inventoryJson = File.ReadAllText(filepath);
            inventoryList = JSON.ParseString(inventoryJson).Deserialize<SerializableListString>();
        }

        // Load action slots
        if (File.Exists(actionFilePath))
        {
            string actionSlotJson = File.ReadAllText(actionFilePath);
            actionSlotList = JSON.ParseString(actionSlotJson).Deserialize<SerializableListString>();
        }

        Debug.Log("Load completed.");
    }

    public void SaveReset()
    {
        foreach(InventorySlot itemSlot in myInventory.inventory)
        {
            itemSlot.item = null;
            itemSlot.itemAmount = 0;
        }
        inventoryList.serializableList.Clear();
        BuildSaveData();
        SaveScriptables();
    }
}
