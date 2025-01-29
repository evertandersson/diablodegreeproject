using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemSO : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public string statText;
    public int levelRequired;
    public Texture2D itemIcon;
    public GameObject prefab;

    public bool isStackable;

    public virtual string GetStatIncrease()
    {
        return "";
    }
}
