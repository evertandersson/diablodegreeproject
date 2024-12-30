using Leguar.TotalJSON;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;

namespace Game
{
    public class InventorySaver : MonoBehaviour
    {
        private static InventorySaver _instance;

        [SerializeField] private ItemDataBaseSO itemDB;

        private SerializableListString inventoryList = new SerializableListString();
        private SerializableListString actionSlotList = new SerializableListString();
        private SerializableListString equipmentList = new SerializableListString();

        public static InventorySaver Instance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("InventorySaver instance is null.");
                }
                return _instance;
            }
        }

        private void Awake()
        {
            // Singleton setup
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Load()
        {
            inventoryList.serializableList.Clear();

            LoadScriptables();

            ImportSaveData();
        }

        private void Save()
        {
            inventoryList.serializableList.Clear();

            BuildSaveData();

            SaveScriptables();

            Debug.Log("OnDisable called: Saving data...");

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
                SaveReset();

            if (Input.GetKeyDown(KeyCode.S))
                Save();
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
            equipmentList.serializableList.Clear();

            SaveSlotsToSerializableList(PlayerManager.Instance.inventory.inventory, inventoryList);
            SaveSlotsToSerializableList(PlayerManager.Instance.slotManager.actionSlots, actionSlotList);
            SaveSlotsToSerializableList(EquipmentManager.Instance.equipmentSlots, equipmentList);
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
                    Debug.Log($"Saved item: {SI.name} with count: {SI.count}");
                }
            }
        }


        public void SaveScriptables()
        {
            Debug.Log("Saving to: " + Application.persistentDataPath);

            string filepath = Application.persistentDataPath + "/inventory_save.json";
            string actionFilePath = Application.persistentDataPath + "/action_slots_save.json";
            string equipmentFilePath = Application.persistentDataPath + "/equipment_slots_save.json";

            // Save inventory
            string inventoryJson = JSON.Serialize(inventoryList).CreatePrettyString();
            File.WriteAllText(filepath, inventoryJson);

            // Save action slots
            string actionSlotJson = JSON.Serialize(actionSlotList).CreatePrettyString();
            File.WriteAllText(actionFilePath, actionSlotJson);

            // Save equipment slots
            string equipmentSlotJson = JSON.Serialize(equipmentList).CreatePrettyString();
            File.WriteAllText(equipmentFilePath, equipmentSlotJson);

            Debug.Log("Save completed.");
        }

        private void ImportSaveData()
        {
            // Clear existing data for both inventories
            ClearSlots(PlayerManager.Instance.inventory.inventory);
            ClearSlots(PlayerManager.Instance.slotManager.actionSlots);
            ClearSlots(EquipmentManager.Instance.equipmentSlots);

            // Load data into inventory and action slots
            LoadSlots(PlayerManager.Instance.inventory.inventory, inventoryList);
            LoadSlots(PlayerManager.Instance.slotManager.actionSlots, actionSlotList);
            LoadSlots(EquipmentManager.Instance.equipmentSlots, equipmentList);
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

                Debug.Log($"Attempting to load item: {name} with count: {count} into slot {i}");

                ItemSO obj = itemDB.GetItem(name);
                if (obj == null)
                {
                    Debug.LogWarning($"Item '{name}' not found in the database. Skipping slot {i}.");
                    continue;
                }

                if (i >= slots.Count)
                {
                    Debug.LogWarning($"Not enough slots to load item '{name}' at index {i}. Skipping.");
                    continue;
                }

                slots[i].item = obj;
                slots[i].itemAmount = count;
                slots[i].CheckIfItemNull(); // Ensure UI updates correctly

                Debug.Log($"Loaded item '{obj.name}' with count {count} into slot {i}.");
            }
        }




        public void LoadScriptables()
        {
            Debug.Log("Loading from: " + Application.persistentDataPath);

            string filepath = Application.persistentDataPath + "/inventory_save.json";
            string actionFilePath = Application.persistentDataPath + "/action_slots_save.json";
            string equipmentFilePath = Application.persistentDataPath + "/equipment_slots_save.json";

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

            // Load equipment slots
            if (File.Exists(equipmentFilePath))
            {
                string equipmentSlotJson = File.ReadAllText(equipmentFilePath);
                equipmentList = JSON.ParseString(equipmentSlotJson).Deserialize<SerializableListString>();
            }

            Debug.Log("Load completed.");
        }

        public void SaveReset()
        {
            foreach (InventorySlot itemSlot in PlayerManager.Instance.inventory.inventory)
            {
                itemSlot.item = null;
                itemSlot.itemAmount = 0;
            }
            inventoryList.serializableList.Clear();
            BuildSaveData();
            SaveScriptables();
        }
    }
}