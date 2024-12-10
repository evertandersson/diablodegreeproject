using UnityEngine;
using UnityEngine.Playables;

public class TriggerCutscene : MonoBehaviour
{
    PlayableDirector director;
    private bool isTriggered = false;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered)
        {
            director.Play();
            isTriggered = true;
        }

    }
}
