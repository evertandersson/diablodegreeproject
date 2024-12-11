using System;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerCutscene : MonoBehaviour
{
    PlayableDirector director;
    private bool isTriggered = false;

    public static event Action StartCutsceneEvent;
    public static event Action StopCutsceneEvent;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
        director.extrapolationMode = DirectorWrapMode.None;
    }

    private void OnEnable()
    {
        // Subscribe to PlayableDirector's stopped event
        director.stopped += OnCutsceneStopped;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        director.stopped -= OnCutsceneStopped;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            StartCutsceneEvent?.Invoke();
            director.Play();
            isTriggered = true;
        }
    }

    private void OnCutsceneStopped(PlayableDirector obj)
    {
        Debug.Log("Cutscene has ended!");
        StopCutsceneEvent?.Invoke();
    }
}
