using Leguar.TotalJSON;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;

public class InventorySaver : MonoBehaviour
{
    [SerializeField] private Inventory myInventory;
    [SerializeField] private ItemDataBaseSO itemDB;
    private SerializableListString SL = new SerializableListString();

    private void Start()
    {
        SL.serializableList.Clear();

        LoadScriptables();

        ImportSaveData();
    }

    private void OnDisable()
    {
        SL.serializableList.Clear();

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
        for (int i = 0; i < myInventory.inventory.Count; i++)
        {
            if (myInventory.inventory[i] != null)
            {
                if (myInventory.inventory[i].item != null)
                {
                    SerializableListString.SerialItem SI = new SerializableListString.SerialItem();
                    SI.name = myInventory.inventory[i].item.name;
                    SI.count = myInventory.inventory[i].itemAmount;

                    SL.serializableList.Add(SI);
                }

            }
        }
    }

    public void SaveScriptables()
    {
        Debug.Log("IS: Saving to: " + Application.persistentDataPath);

        string filepath = Application.persistentDataPath + "/newsave.json";

        StreamWriter sw = new StreamWriter(filepath);

        JSON jsonObject = JSON.Serialize(SL);

        string json = jsonObject.CreatePrettyString();

        sw.WriteLine(json);

        sw.Close();
    }

    private void ImportSaveData()
    {
        for (int i = 0; i < SL.serializableList.Count; i++)
        {
            string name = SL.serializableList[i].name;
            int count = SL.serializableList[i].count;

            ItemSO obj = itemDB.GetItem(name);
            if (obj)
            {
                myInventory.AddItemToInventory(obj);

                myInventory.inventory[i].itemAmount = count;
            }
        }
    }

    public void LoadScriptables()
    {
        Debug.Log("IS: Loading from: " + Application.persistentDataPath);

        string filepath = Application.persistentDataPath + "/newsave.json";

        if (File.Exists(filepath))
        {
            string json = File.ReadAllText(filepath);

            JSON jsonObject = JSON.ParseString(json);

            SL = jsonObject.Deserialize<SerializableListString>();
        }
    }

    public void SaveReset()
    {
        foreach(InventorySlot itemSlot in myInventory.inventory)
        {
            itemSlot.item = null;
            itemSlot.itemAmount = 0;
        }
        SL.serializableList.Clear();
        BuildSaveData();
        SaveScriptables();
    }
}
