using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SerializableList
{
    public struct SerialItem
    {
        public string name;
        public int count;
    }

    public List<SerialItem> serializableList = new List<SerialItem>();
}
