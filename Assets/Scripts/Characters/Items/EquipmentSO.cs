using UnityEngine;

namespace Game
{
    public enum StatType
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
    public class Stat
    {
        public StatType type;
        [HideInInspector] public string statText;
        public float statImprovement;

        public void UpdateStatText()
        {
            switch (type)
            {
                case StatType.Damage:
                    statText = "Additional Damage:";
                    break;
                case StatType.Defense:
                    statText = "Additional Defense:";
                    break;
                case StatType.Health:
                    statText = "Additional Health:";
                    break;
                case StatType.HealthRegen:
                    statText = "Health Regeneration:";
                    break;
                case StatType.Mana:
                    statText = "Additional Mana:";
                    break;
                case StatType.ManaRegen:
                    statText = "Mana Regeneration:";
                    break;
                case StatType.Movement:
                    statText = "Movement Speed:";
                    break;
                case StatType.AttackSpeed:
                    statText = "Attack Speed:";
                    break;
            }
        }
    }

    [CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
    public class EquipmentSO : ItemSO, IHasInfo
    {
        public EquipmentType equipmentType;

        public Stat mainStat;
        public Stat[] bonusStats;

        public Stat[] Stats { get => bonusStats; set => Stats = bonusStats; }

        public void UpdateStatDisplay()
        {
            mainStat.UpdateStatText();

            foreach(Stat stat in bonusStats)
            {
                stat.UpdateStatText();
            }
        }
    }
}
