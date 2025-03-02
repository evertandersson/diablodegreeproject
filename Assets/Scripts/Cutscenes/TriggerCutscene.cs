using System;
using UnityEngine;
using UnityEngine.Playables;

public abstract class TriggerCutscene : MonoBehaviour
{
    protected PlayableDirector director;
    private bool isTriggered = false;

    protected abstract Action StartCutsceneAction { get; }
    protected abstract Action StopCutsceneAction { get; }

    public static bool IsCutScenePlaying = false;

    protected virtual void Awake()
    {
        director = GetComponent<PlayableDirector>();
        director.extrapolationMode = DirectorWrapMode.None;
    }

    private void OnEnable()
    {
        director.stopped += OnCutsceneStopped;
    }

    private void OnDisable()
    {
        director.stopped -= OnCutsceneStopped;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            StartCutsceneAction?.Invoke();
            IsCutScenePlaying = true;
            director.Play();
            isTriggered = true;
        }
    }

    private void OnCutsceneStopped(PlayableDirector obj)
    {
        Debug.Log("Cutscene has ended!");
        IsCutScenePlaying = false;
        StopCutsceneAction?.Invoke();
    }
}
