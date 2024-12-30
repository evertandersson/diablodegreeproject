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
    }
}
