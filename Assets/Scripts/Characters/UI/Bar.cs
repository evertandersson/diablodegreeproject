using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    [SerializeField] protected Slider slider;
    [SerializeField] protected Slider backgroundSlider;

    protected IEnumerator SmoothFill(Slider target, Slider moving, float delay, float speed)
    {
        yield return new WaitForSeconds(delay);

        while (moving.value != target.value)
        {
            moving.value = Mathf.MoveTowards(moving.value, target.value, speed * Time.deltaTime);
            yield return null;
        }
    }

}
