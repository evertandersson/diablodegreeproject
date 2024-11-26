using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : Bar
{
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        backgroundSlider.maxValue = health;
        backgroundSlider.value = health;
    }

    public virtual void SetHealth(int health)
    {
        if (gameObject.activeSelf)
        {
            slider.value = health;
            StartCoroutine(SmoothFill(slider, backgroundSlider, 0.5f, 10f));
        }
    }
}
