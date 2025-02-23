using Leguar.TotalJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game
{
    public class SaveManager : MonoBehaviour
    {
        private static SaveManager _instance; // Singleton instance

        [SerializeField] private ItemDataBaseSO itemDB; // Reference to item database

        // Lists to store saved intentory data
        private SerializableList inventoryList = new SerializableList();
        private SerializableList actionSlotList = new SerializableList();
        private SerializableList equipmentList = new SerializableList();

        private int playerLevel; 
        private int playerExperience;

        public SerializableList pickedUpItemsList = new SerializableList(); // List of picked up world items
        public SerializableList doorsOpenedList = new SerializableList(); // List of which doors player has opened

        private Vector3 currentSpawnPoint;
        private Vector3 spawnOffset = new Vector3(0, 1f, 0);

        public SerializableList skillsUnlockedList = new SerializableList(); // Lists of skills unlocked
        
        public static event Action LoadWorldObjects; // Event triggered when world objects are loaded

        private bool useEncryption = true; // Bool to enable/disable encryption
        private readonly string encryptionCodeWord = "word"; // Simple encryption key

        // Singleton setup
        public static SaveManager Instance
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
            // Clear existing data before loading
            inventoryList.serializableList.Clear();
            actionSlotList.serializableList.Clear();
            equipmentList.serializableList.Clear();

            LoadDataFromFile(); // Load data from file
            ImportSaveData(); // Apply loaded data to game objects
        }

        public void Save()
        {
            BuildSaveData(); // Collect data from game objects
            SaveDataToFile(); // Write data to file

            Debug.Log("Data saved successfully.");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X)) // Press X to reset save
                SaveReset();

            if (Input.GetKeyDown(KeyCode.S)) // Press S to save
                Save();
        }

        private void BuildSaveData()
        {
            // Clear lists to avoid duplicates
            inventoryList.serializableList.Clear();
            actionSlotList.serializableList.Clear();
            equipmentList.serializableList.Clear();

            // Store inventory, action slots, and equipment in save lists
            SaveSlotsToSerializableList(PlayerManager.Instance.inventory.inventory, inventoryList);
            SaveSlotsToSerializableList(PlayerManager.Instance.slotManager.actionSlots, actionSlotList);
            SaveSlotsToSerializableList(EquipmentManager.Instance.equipmentSlots, equipmentList);

            // Save level and xp
            playerLevel = PlayerManager.Instance.levelSystem.GetCurrentLevel();
            playerExperience = PlayerManager.Instance.levelSystem.GetExperience();

            // Save player position
            
            currentSpawnPoint = PlayerManager.Instance.CurrentPlayerState == PlayerManager.State.Dead 
                ? currentSpawnPoint = GameManager.GetSpawnPositionAtLevel() 
                : PlayerManager.Instance.transform.position;
        }

        private void SaveSlotsToSerializableList(IEnumerable<InventorySlot> slots, SerializableList targetList)
        {
            foreach (var slot in slots)
            {
                if (slot != null && slot.item != null)
                {
                    SerializableList.SerialItem SI = new SerializableList.SerialItem
                    {
                        name = slot.item.name,
                        count = slot.itemAmount
                    };
                    targetList.serializableList.Add(SI);
                    Debug.Log($"Saved item: {SI.name} with count: {SI.count}");
                }
            }
        }

        public void SaveDataToFile()
        {
            Debug.Log("Saving to: " + Application.persistentDataPath);

            string filepath = Application.persistentDataPath + "/player_save.json";

            // Create save data object
            SaveData saveData = new SaveData
            {
                inventory = inventoryList,
                actionSlots = actionSlotList,
                equipmentSlots = equipmentList,

                level = playerLevel,
                experience = playerExperience,

                itemsPickedUp = pickedUpItemsList,
                doorsOpened = doorsOpenedList,

                skillsUnlocked = skillsUnlockedList,

                spawnPosition = currentSpawnPoint
            };

            string saveJson = JSON.Serialize(saveData).CreatePrettyString(); // Convert to JSON

            if (useEncryption)
            {
                saveJson = EncryptDecrypt(saveJson); // Encrypt JSON 
            }

            File.WriteAllText(filepath, saveJson); // Save to file

            Debug.Log("Save completed.");
        }

        public void AddObjectToList(string objectId, SerializableList targetList)
        {
            if (!targetList.serializableList.Exists(obj => obj.name == objectId))
            {
                SerializableList.SerialItem addedObj = new SerializableList.SerialItem
                {
                    name = objectId,
                    count = 1
                };

                targetList.serializableList.Add(addedObj);
                Save(); // Save the updated list
                Debug.Log($"Item with ID {objectId} added to picked-up list.");
            }
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

            if (playerLevel != 0)
            {
                levelSystem.SetLevel(playerLevel); // Set Level 
                levelSystem.AddExperience(playerExperience); // Add Experience 
            }

            // Debugging Loaded Values
            Debug.Log($"Applied Level: {levelSystem.GetCurrentLevel()}, XP: {levelSystem.GetExperience()}");

            // Update the UI
            PlayerManager.Instance.SetStats();
            PlayerManager.Instance.progressBar.SetLevelSystem(levelSystem);
            
            Debug.Log(PlayerManager.Instance.Level);

            levelSystem.OnLevelChanged += PlayerManager.Instance.UpgradeLevel;

            SkillTreeManager.Instance.Initialize();

            if (currentSpawnPoint != Vector3.zero)
                StartCoroutine(SetCurrentSpawnPoint());

            LoadWorldObjects?.Invoke();
        }

        private IEnumerator SetCurrentSpawnPoint()
        {
            yield return new WaitForEndOfFrame();
            PlayerManager player = PlayerManager.Instance;
            player.transform.position = currentSpawnPoint + spawnOffset;
            player.Agent.Warp(player.transform.position);
        }


        private void ClearSlots(IEnumerable<InventorySlot> slots)
        {
            foreach (var slot in slots)
            {
                slot.item = null;
                slot.itemAmount = 0;
            }
        }

        private void LoadSlots(IList<InventorySlot> slots, SerializableList targetList)
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

        public void LoadDataFromFile()
        {
            Debug.Log("Loading from: " + Application.persistentDataPath);

            string filepath = Application.persistentDataPath + "/player_save.json";

            if (File.Exists(filepath))
            {
                string saveJson = File.ReadAllText(filepath);

                if (useEncryption)
                {
                    saveJson = EncryptDecrypt(saveJson);
                }

                SaveData saveData = JSON.ParseString(saveJson).Deserialize<SaveData>();

                inventoryList = saveData.inventory;
                actionSlotList = saveData.actionSlots;
                equipmentList = saveData.equipmentSlots;

                playerLevel = saveData.level;
                playerExperience = saveData.experience;

                pickedUpItemsList = saveData.itemsPickedUp ?? new SerializableList();
                doorsOpenedList = saveData.doorsOpened ?? new SerializableList();

                skillsUnlockedList = saveData.skillsUnlocked ?? new SerializableList();

                currentSpawnPoint = saveData.spawnPosition;

                //Debug.Log($"Loaded Inventory: {inventoryList.serializableList?.Count} items");
                //Debug.Log($"Loaded Action Slots: {actionSlotList.serializableList?.Count} items");
                //Debug.Log($"Loaded Equipment Slots: {equipmentList.serializableList?.Count} items");
                //Debug.Log($"Loaded Level: {playerLevel}, XP: {playerExperience}");
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
            pickedUpItemsList.serializableList.Clear();
            doorsOpenedList.serializableList.Clear();
            skillsUnlockedList.serializableList.Clear();

            // Reset Level and Experience
            if (PlayerManager.Instance.levelSystem == null)
            {
                PlayerManager.Instance.levelSystem = new LevelSystem();
            }

            PlayerManager.Instance.levelSystem.SetLevel(1); // Reset to Level 1
            PlayerManager.Instance.levelSystem.AddExperience(-PlayerManager.Instance.levelSystem.GetExperience()); // Reset XP to 0

            playerLevel = 1;
            playerExperience = 0;

            // Update UI and other dependent systems
            PlayerManager.Instance.SetStats();
            PlayerManager.Instance.progressBar.SetLevelSystem(PlayerManager.Instance.levelSystem);

            // Clear any skill tree progress
            SkillTreeManager.Instance.Initialize();

            // Save Reset Data
            BuildSaveData();
            SaveDataToFile();

            Debug.Log("Game state has been reset: Inventory, Action Slots, Equipment, Level, and Experience.");
        }

        public void NewGame()
        {
            // Clear Serializable Lists
            inventoryList.serializableList.Clear();
            actionSlotList.serializableList.Clear();
            equipmentList.serializableList.Clear();
            pickedUpItemsList.serializableList.Clear();
            doorsOpenedList.serializableList.Clear();
            skillsUnlockedList.serializableList.Clear();
            playerLevel = 1;
            playerExperience = 0;
            currentSpawnPoint = Vector3.zero;


            SaveDataToFile();
        }

        private string EncryptDecrypt(string data)
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
            }
            return modifiedData;
        }

    }

    [System.Serializable]
    public class SaveData
    {
        public SerializableList inventory;
        public SerializableList actionSlots;
        public SerializableList equipmentSlots;
        public int level;
        public int experience;
        public SerializableList itemsPickedUp;
        public SerializableList doorsOpened;
        public SerializableList skillsUnlocked;
        public Vector3 spawnPosition;
    }
}
