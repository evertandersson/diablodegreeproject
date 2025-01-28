using System.Collections;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    None,
    Item,
    Skill,
    Equipment
}

public class InfoWindow : MonoBehaviour
{
    public static InfoWindow Instance;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI levelRequiredText;
    [SerializeField] private TextMeshProUGUI[] bonusStat;

    [SerializeField] private RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
        rectTransform = GetComponent<RectTransform>();
    }

    public void ShowInfoWindow(Vector3 position,
        float width,
        float height,
        ItemType itemType,
        string title = "title",
        string description = "description")
    {
        Vector3 offset = new Vector3(0, -height * 0.5f, 0);
        transform.position = position + offset;
        titleText.text = title;
        descriptionText.text = description;

        if (itemType == ItemType.Skill || itemType == ItemType.Item)
        {
            foreach (var stat in bonusStat)
            {
                stat.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (var stat in bonusStat)
            {
                stat.gameObject.SetActive(true);
            }
        }

        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }

    public void HideInfoWindow()
    {
        gameObject.SetActive(false);
    }
}
