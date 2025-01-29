using Game;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour
{
    public static InfoWindow Instance;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI statText;
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
        levelRequiredText.text = "Requires Level " + item.levelRequired;

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

            statText.text = item.statText + " " + equipment.GetStatIncrease();

            if (equipment.bonusStats.Length <= 0)
                return;

            for (int i = 0; i < bonusStat.Length; i++)
            {
                if (i >= equipment.bonusStats.Length)
                    break;

                if (bonusStat[i] == null)
                    continue;

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
        levelRequiredText.text = "Cost: " + skill.skillCost;

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
            if (i >= skill.statsImprovements.Length)
                break;

            if (bonusStat[i] == null)
                continue;

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
