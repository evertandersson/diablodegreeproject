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
    public class Stat
    {
        public BonusStatType type;
        [HideInInspector] public string statText;
        public float statImprovement;

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
