using Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour
{
    public static InfoWindow Instance;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] private Image[] lines;
    [SerializeField] private TextMeshProUGUI levelRequiredText;
    [SerializeField] private TextMeshProUGUI[] bonusStat;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void ShowEquipmentInfoWindow(Vector3 position,
        float width,
        float height,
        ItemSO item)
    {
        Vector3 offset = new Vector3(0, -height * 0.5f, 0);
        transform.position = position + offset;
        titleText.text = item.itemName;
        descriptionText.text = item.itemDescription;
        levelRequiredText.gameObject.SetActive(true);
        levelRequiredText.text = "Requires Level " + item.levelRequired;

        foreach(Image line in lines)
        {
            line.gameObject.SetActive(true);
        }

        transform.SetAsLastSibling();
        gameObject.SetActive(true);

        if (item is AttackTypeSO || item is ItemSO || item is PotionSO)
        {
            if (item is not AttackTypeSO && item is not PotionSO)
            {
                statText.gameObject.SetActive(false);
            }
            else
            {
                statText.gameObject.SetActive(true);
                statText.text = item.statText + " " + item.GetStatIncrease();
            }
            foreach (var stat in bonusStat)
            {
                stat.gameObject.SetActive(false);
            }
        }
        if (item is EquipmentSO equipment) 
        {
            statText.gameObject.SetActive(true);

            equipment.UpdateStatDisplay();
            statText.text = item.statText + " " + equipment.mainStat.statImprovement;

            if (equipment.bonusStats.Length <= 0)
                return;

            for (int i = 0; i < bonusStat.Length; i++)
            {
                if (i >= equipment.bonusStats.Length || bonusStat[i] == null)
                {
                    bonusStat[i].gameObject.SetActive(false);
                    continue;
                }

                bonusStat[i].gameObject.SetActive(true);
                bonusStat[i].text = equipment.bonusStats[i].statText + " " + equipment.bonusStats[i].statImprovement;
                
            }
        }
    }

    public void ShowSkillInfoWindow(Vector3 position,
        float width,
        float height,
        SkillSO skill)
    {
        Vector3 offset = new Vector3(0, -height * 0.5f, 0);
        transform.position = position + offset;
        titleText.text = skill.skillName;
        descriptionText.text = skill.skillDescription;
        levelRequiredText.gameObject.SetActive(false);

        foreach (Image line in lines)
        {
            line.gameObject.SetActive(false);
        }

        transform.SetAsLastSibling();
        gameObject.SetActive(true);

        statText.gameObject.SetActive(false);


        foreach (TextMeshProUGUI stat in bonusStat)
        {
            statText.gameObject.SetActive(false);
        }

        if (skill.statsImprovements.Length <= 0)
            return;

        for (int i = 0; i < bonusStat.Length; i++)
        {
            if (i >= skill.statsImprovements.Length || bonusStat[i] == null)
            {
                bonusStat[i].gameObject.SetActive(false);
                continue;
            }

            bonusStat[i].gameObject.SetActive(true);
            skill.GetStatIncrease();
            bonusStat[i].text = skill.statsImprovements[i].statText + " " + skill.statsImprovements[i].statImprovement;
        }
    }

    public void HideInfoWindow()
    {
        gameObject.SetActive(false);
    }
}
