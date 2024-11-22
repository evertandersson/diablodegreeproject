using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] protected Slider slider;
    [SerializeField] protected Slider backgroundSlider;

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
            StartCoroutine(SetBackgroundSlider());
        }
    }

    private IEnumerator SetBackgroundSlider()
    {
        yield return new WaitForSeconds(0.5f);

        while (backgroundSlider.value > slider.value)
        {
            backgroundSlider.value = Mathf.MoveTowards(backgroundSlider.value, slider.value, 10f * Time.deltaTime);
            yield return null;
        }

    }


}
