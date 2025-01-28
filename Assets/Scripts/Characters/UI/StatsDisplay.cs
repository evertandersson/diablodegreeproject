using TMPro;
using UnityEngine;

namespace Game
{
    public class StatsDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI healthRegenText;
        [SerializeField] private TextMeshProUGUI manaText;
        [SerializeField] private TextMeshProUGUI manaRegenText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI attackSpeedText;
        [SerializeField] private TextMeshProUGUI defenseText;

        public void UpdateStatsText()
        {
            levelText.text = PlayerManager.Instance.Level.ToString();
            healthText.text = PlayerManager.Instance.MaxHealth.ToString();
            healthRegenText.text = "0"; // TODO: Add health regen variable in playermanager
            manaText.text = PlayerManager.Instance.MaxMana.ToString();
            manaRegenText.text = PlayerManager.Instance.ManaRegen.ToString();
            damageText.text = PlayerManager.Instance.Damage.ToString();
            attackSpeedText.text = PlayerManager.Instance.AttackSpeed.ToString();
            defenseText.text = PlayerManager.Instance.Defense.ToString();
        }
    }
}