using System.Collections;
using UnityEngine;

public class PlayerHealthBar : HealthBar
{
    private Vector3 originalScale;
    private Coroutine scaleCoroutine;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public override void SetHealth(int health)
    {
        if (health < slider.value) // Only trigger scaling when taking damage
        {
            if (scaleCoroutine != null)
                StopCoroutine(scaleCoroutine);

            scaleCoroutine = StartCoroutine(ScaleEffect());
        }
        base.SetHealth(health);
    }

    private IEnumerator ScaleEffect()
    {
        // Increase scale
        Vector3 targetScale = originalScale + new Vector3(0.3f, 0.3f, 0.3f);
        float durationToBig = 0.025f;
        float durationToSmall = 0.15f;
        float elapsed = 0f;

        while (elapsed < durationToBig)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / durationToBig);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;

        // Return to original scale
        elapsed = 0f;
        while (elapsed < durationToSmall)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / durationToSmall);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }
}
