using TMPro;
using UnityEngine;

namespace Game
{
    public class StatsDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI damageText;
        [SerializeField] private TextMeshProUGUI defenseText;

        public void UpdateStatsText()
        {
            levelText.text = PlayerManager.Instance.Level.ToString();
            healthText.text = PlayerManager.Instance.MaxHealth.ToString();
            damageText.text = PlayerManager.Instance.Damage.ToString();
            defenseText.text = PlayerManager.Instance.Defense.ToString();
        }
    }
}