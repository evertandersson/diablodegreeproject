using TMPro;
using UnityEngine;

public class InfoWindow : MonoBehaviour
{
    public static InfoWindow Instance;

    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void ShowInfoWindow(Vector3 position,
        float width,
        float height,
        string title = "title",
        string description = "description")
    {
        Vector3 offset = new Vector3(0, -height * 0.5f, 0);
        transform.position = position + offset;
        titleText.text = title;
        descriptionText.text = description;
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }

    public void HideInfoWindow()
    {
        gameObject.SetActive(false);
    }
}
