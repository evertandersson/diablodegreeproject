using UnityEngine;

namespace Game
{
    public enum EquipmentType
    {
        Helmet,
        Armour, 
        Boots,
        FirstHandWeapon,
        SecondHandWeapon
    }

    public class EquipmentSlot : InventorySlot
    {
        public EquipmentType equipmentType;
    }

}
