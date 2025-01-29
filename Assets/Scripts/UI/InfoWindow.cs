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

    public void ShowInfoWindow(Vector3 position,
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
                if (equipment.bonusStats.Length < i)
                    break;

                bonusStat[i].gameObject.SetActive(true);
                bonusStat[i].text = equipment.bonusStats[i].statText + " " + equipment.bonusStats[i].statImprovement;
                
            }
        }
    }

    public void HideInfoWindow()
    {
        gameObject.SetActive(false);
    }
}
