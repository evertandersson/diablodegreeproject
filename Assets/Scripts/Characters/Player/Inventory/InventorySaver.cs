using Leguar.TotalJSON;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game
{
    public class InventorySaver : MonoBehaviour
    {
        private static InventorySaver _instance;

        [SerializeField] private ItemDataBaseSO itemDB;

        private SerializableListString inventoryList = new SerializableListString();
        private SerializableListString actionSlotList = new SerializableListString();
        private SerializableListString equipmentList = new SerializableListString();

        private int playerLevel;
        private int playerExperience;

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

        private void Start()
        {
            
        }

        public void Load()
        {
            inventoryList.serializableList.Clear();
            actionSlotList.serializableList.Clear();
            equipmentList.serializableList.Clear();

            LoadScriptables();
            ImportSaveData();
        }

        private void Save()
        {
            inventoryList.serializableList.Clear();
            actionSlotList.serializableList.Clear();
            equipmentList.serializableList.Clear();

            BuildSaveData();
            SaveScriptables();

            Debug.Log("Data saved successfully.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
                SaveReset();

            if (Input.GetKeyDown(KeyCode.S))
                Save();
        }

        private void BuildSaveData()
        {
            inventoryList.serializableList.Clear();
            actionSlotList.serializableList.Clear();
            equipmentList.serializableList.Clear();

            SaveSlotsToSerializableList(PlayerManager.Instance.inventory.inventory, inventoryList);
            SaveSlotsToSerializableList(PlayerManager.Instance.slotManager.actionSlots, actionSlotList);
            SaveSlotsToSerializableList(EquipmentManager.Instance.equipmentSlots, equipmentList);

            //Save level and xp
            playerLevel = PlayerManager.Instance.levelSystem.GetCurrentLevel();
            playerExperience = PlayerManager.Instance.levelSystem.GetExperience();
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

            string filepath = Application.persistentDataPath + "/player_save.json";

            SaveData saveData = new SaveData
            {
                inventory = inventoryList,
                actionSlots = actionSlotList,
                equipmentSlots = equipmentList,

                level = PlayerManager.Instance.levelSystem.GetCurrentLevel(),
                experience = PlayerManager.Instance.levelSystem.GetExperience()
            };

            string saveJson = JSON.Serialize(saveData).CreatePrettyString();
            File.WriteAllText(filepath, saveJson);

            Debug.Log("Save completed.");
        }

        private void ImportSaveData()
        {
            ClearSlots(PlayerManager.Instance.inventory.inventory);
            ClearSlots(PlayerManager.Instance.slotManager.actionSlots);
            ClearSlots(EquipmentManager.Instance.equipmentSlots);

            LoadSlots(PlayerManager.Instance.inventory.inventory, inventoryList);
            LoadSlots(PlayerManager.Instance.slotManager.actionSlots, actionSlotList);
            LoadEquipmentSlots(EquipmentManager.Instance.equipmentSlots);

            // Ensure LevelSystem exists
            if (PlayerManager.Instance.levelSystem == null)
            {
                PlayerManager.Instance.levelSystem = new LevelSystem();
            }

            // Apply Level and XP from save data
            LevelSystem levelSystem = PlayerManager.Instance.levelSystem;

            levelSystem.SetLevel(playerLevel); // Set Level First
            levelSystem.AddExperience(playerExperience); // Add Experience Next

            // Debugging Loaded Values
            Debug.Log($"Applied Level: {levelSystem.GetCurrentLevel()}, XP: {levelSystem.GetExperience()}");

            // Update the UI
            PlayerManager.Instance.SetStats();
            PlayerManager.Instance.progressBar.SetLevelSystem(levelSystem);

            SkillTreeManager.Instance.Initialize();
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
                slots[i].CheckIfItemNull();

                Debug.Log($"Loaded item '{obj.name}' with count {count} into slot {i}.");
            }
        }

        private void LoadEquipmentSlots(IList<EquipmentSlot> slots)
        {
            for (int i = 0; i < equipmentList.serializableList.Count; i++)
            {
                string name = equipmentList.serializableList[i].name;
                int count = equipmentList.serializableList[i].count;

                EquipmentSO obj = itemDB.GetItem(name) as EquipmentSO;
                if (obj == null)
                {
                    Debug.LogWarning($"Item '{name}' not found in the database. Skipping slot {i}.");
                    continue;
                }

                foreach (EquipmentSlot slot in slots)
                {
                    if (slot.equipmentType == obj.equipmentType)
                    {
                        slot.item = obj;
                        slot.itemAmount = count;
                        slot.CheckIfItemNull();
                        break;
                    }
                }
            }
            EquipmentManager.Instance.GetStatsFromArmour();
        }

        public void LoadScriptables()
        {
            Debug.Log("Loading from: " + Application.persistentDataPath);

            string filepath = Application.persistentDataPath + "/player_save.json";

            if (File.Exists(filepath))
            {
                string saveJson = File.ReadAllText(filepath);
                SaveData saveData = JSON.ParseString(saveJson).Deserialize<SaveData>();

                inventoryList = saveData.inventory;
                actionSlotList = saveData.actionSlots;
                equipmentList = saveData.equipmentSlots;

                playerLevel = saveData.level;
                playerExperience = saveData.experience;

                Debug.Log($"Loaded Inventory: {inventoryList.serializableList.Count} items");
                Debug.Log($"Loaded Action Slots: {actionSlotList.serializableList.Count} items");
                Debug.Log($"Loaded Equipment Slots: {equipmentList.serializableList.Count} items");
                Debug.Log($"Loaded Level: {playerLevel}, XP: {playerExperience}");
            }
            else
            {
                Debug.LogWarning("Save file not found.");
            }

            Debug.Log("Load completed from player_save.json.");
        }


        public void SaveReset()
        {
            // Reset Inventory Slots
            foreach (InventorySlot itemSlot in PlayerManager.Instance.inventory.inventory)
            {
                itemSlot.item = null;
                itemSlot.itemAmount = 0;
            }

            // Reset Action Slots
            foreach (ActionSlot actionSlot in PlayerManager.Instance.slotManager.actionSlots)
            {
                actionSlot.item = null;
                actionSlot.itemAmount = 0;
            }

            // Reset Equipment Slots
            foreach (EquipmentSlot equipmentSlot in EquipmentManager.Instance.equipmentSlots)
            {
                equipmentSlot.item = null;
                equipmentSlot.itemAmount = 0;
            }

            // Clear Serializable Lists
            inventoryList.serializableList.Clear();
            actionSlotList.serializableList.Clear();
            equipmentList.serializableList.Clear();

            // Reset Level and Experience
            if (PlayerManager.Instance.levelSystem == null)
            {
                PlayerManager.Instance.levelSystem = new LevelSystem();
            }

            PlayerManager.Instance.levelSystem.SetLevel(1); // Reset to Level 1
            PlayerManager.Instance.levelSystem.AddExperience(-PlayerManager.Instance.levelSystem.GetExperience()); // Reset XP to 0

            playerLevel = 1; // Ensure save data reflects reset
            playerExperience = 0;

            // Update UI and other dependent systems
            PlayerManager.Instance.SetStats();
            PlayerManager.Instance.progressBar.SetLevelSystem(PlayerManager.Instance.levelSystem);

            // Clear any skill tree progress
            SkillTreeManager.Instance.Initialize();

            // Save Reset Data
            BuildSaveData();
            SaveScriptables();

            Debug.Log("Game state has been reset: Inventory, Action Slots, Equipment, Level, and Experience.");
        }

    }

    [System.Serializable]
    public class SaveData
    {
        public SerializableListString inventory;
        public SerializableListString actionSlots;
        public SerializableListString equipmentSlots;
        public int level;
        public int experience;
    }
}
