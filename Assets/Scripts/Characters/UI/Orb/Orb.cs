using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Orb : MonoBehaviour
{
    [SerializeField] protected Image barFill; // Reference to the UI image representing the health bar
    [SerializeField] private float animationDuration = 0.2f; // Duration of the scaling animation

    private Coroutine scaleCoroutine; // Reference to the currently running coroutine (if any)
    private int maxValue; // Store the maximum health

    public void SetMaxValue(int value)
    {
        maxValue = value; // Set the maximum health

        SetValue(Game.PlayerManager.Instance.Health);
    }

    public void SetValue(float value)
    {
        // Clamp health to ensure it's between 0 and maxHealth
        value = Mathf.Clamp(value, 0, maxValue);

        // Calculate the target scale as a percentage of max health
        float targetFill = (float)value / maxValue;

        // Stop any currently running scaling animation
        if (scaleCoroutine != null)
        {
            StopCoroutine(scaleCoroutine);
        }

        // Start a new scaling animation
        scaleCoroutine = StartCoroutine(ScaleBar(targetFill));
    }

    private IEnumerator ScaleBar(float targetFill)
    {
        float currentFill = barFill.fillAmount; // Get the current scale

        float elapsedTime = 0f;

        // Smoothly interpolate the scale over time
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            barFill.fillAmount = Mathf.Lerp(currentFill, targetFill, elapsedTime / animationDuration);
            yield return null;
        }

        // Ensure the scale is set to the exact target value at the end
        barFill.fillAmount = targetFill;
    }
}
