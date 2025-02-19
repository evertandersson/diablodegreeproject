using TMPro;
using UnityEngine;

public class EnemyHealthBar : HealthBar
{
    private Transform cameraTransform; // Reference to the camera
    [SerializeField] private TextMeshProUGUI levelText;


    void Start()
    {
        cameraTransform = Camera.main.transform;

        if (cameraTransform == null)
        {
            Debug.Log("No camera found in the scene!");
        }
    }

    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            transform.LookAt(transform.position + cameraTransform.forward);
        }
    }

    public override void SetHealth(float health)
    {
        if (health <= 0)
        {
            DisplayHealthBar(false);
        }
        base.SetHealth(health);
    }

    public void DisplayHealthBar(bool show)
    {
        gameObject.SetActive(show);
    }

    public void SetLevelText(int level)
    {
        levelText.text = level.ToString();
    }
}
