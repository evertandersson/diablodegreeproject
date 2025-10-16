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

    public class EquipmentSlot : Slot
    {
        public EquipmentType equipmentType;
    }

}
