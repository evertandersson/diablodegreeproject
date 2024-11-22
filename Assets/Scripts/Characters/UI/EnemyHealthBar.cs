using UnityEngine;

public class EnemyHealthBar : HealthBar
{
    private Transform cameraTransform; // Reference to the camera

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

    public override void SetHealth(int health)
    {
        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
        base.SetHealth(health);
    }
}
