using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarFill; // Reference to the UI image representing the health bar
    [SerializeField] private float animationDuration = 0.2f; // Duration of the scaling animation

    private float originalScale; // Store the original scale of the health bar
    private Coroutine scaleCoroutine; // Reference to the currently running coroutine (if any)
    private int maxHealth; // Store the maximum health

    private void Awake()
    {
        originalScale = healthBarFill.fillAmount; // Initialize the original scale
    }

    public void SetMaxHealth(int health)
    {
        maxHealth = health; // Set the maximum health
        SetHealth(health); // Initialize the health bar to full
    }

    public void SetHealth(int health)
    {
        // Clamp health to ensure it's between 0 and maxHealth
        health = Mathf.Clamp(health, 0, maxHealth);

        // Calculate the target scale as a percentage of max health
        float targetFill = (float)health / maxHealth;

        // Stop any currently running scaling animation
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        // Start a new scaling animation
        scaleCoroutine = StartCoroutine(ScaleHealthBar(targetFill));
    }

    private IEnumerator ScaleHealthBar(float targetFill)
    {
        float currentFill = healthBarFill.fillAmount; // Get the current scale

        float elapsedTime = 0f;

        // Smoothly interpolate the scale over time
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            healthBarFill.fillAmount = Mathf.Lerp(currentFill, targetFill, elapsedTime / animationDuration);
            yield return null;
        }

        // Ensure the scale is set to the exact target value at the end
        healthBarFill.fillAmount = targetFill;
    }
}
