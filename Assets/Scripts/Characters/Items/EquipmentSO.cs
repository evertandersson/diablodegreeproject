using UnityEngine;

namespace Game
{
    public enum BonusStatType
    {
        Damage,
        Defense,
        Health,
        HealthRegen,
        Mana,
        ManaRegen,
        Movement,
        AttackSpeed
    }

    [System.Serializable]
    public class BonusStat
    {
        public BonusStatType type;
        [HideInInspector] public string statText;
        public int statImprovement;

        public void UpdateStatText()
        {
            switch (type)
            {
                case BonusStatType.Damage:
                    statText = "Additional Damage:";
                    break;
                case BonusStatType.Defense:
                    statText = "Additional Defense:";
                    break;
                case BonusStatType.Health:
                    statText = "Additional Health:";
                    break;
                case BonusStatType.HealthRegen:
                    statText = "Health Regeneration:";
                    break;
                case BonusStatType.Mana:
                    statText = "Additional Mana:";
                    break;
                case BonusStatType.ManaRegen:
                    statText = "Mana Regeneration:";
                    break;
                case BonusStatType.Movement:
                    statText = "Movement Speed:";
                    break;
                case BonusStatType.AttackSpeed:
                    statText = "Attack Speed:";
                    break;
            }
        }
    }

    [CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
    public class EquipmentSO : ItemSO
    {
        public EquipmentType equipmentType;

        public int damageIncrease;
        public int defenseIncrease;
        public int healthIncrease;
        public float movementSpeedIncrease;

        public BonusStat[] bonusStats;

        public override string GetStatIncrease()
        {
            foreach(BonusStat stat in bonusStats)
            {
                stat.UpdateStatText();
            }

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
