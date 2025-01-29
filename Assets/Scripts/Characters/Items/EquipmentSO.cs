using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
    public class EquipmentSO : ItemSO
    {
        public EquipmentType equipmentType;

        public int damageIncrease;
        public int defenseIncrease;
        public int healthIncrease;
        public float movementSpeedIncrease;

        public override string GetStatIncrease()
        {
            switch (equipmentType)
            {
                case EquipmentType.FirstHandWeapon:
                    return damageIncrease.ToString();
                default:
                    return defenseIncrease.ToString();
            }
        }
    }
}
