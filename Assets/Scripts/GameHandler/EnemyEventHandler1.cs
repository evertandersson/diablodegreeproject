using Game;
using System.Diagnostics.Tracing;
using UnityEngine;

public class EnemyEventHandler : EventHandler
{
    private void OnEnable()
    {
        GameManager.enemyUpdate += UpdateEnemyEvent;
    }
    private void OnDisable()
    {
        GameManager.enemyUpdate -= UpdateEnemyEvent;
    }

    private void UpdateEnemyEvent()
    {
        if (eventStack.Count == 0) return;

        currentEvent?.OnUpdate();
    }

    protected override void UpdateEvents()
    {
        if (eventStack.Count == 0) return;

        // If no current event, set the next event as the current event
        if (currentEvent == null)
        {
            currentEvent = eventStack[0];
            bool firstTime = !startedEvents.Contains(currentEvent);
            startedEvents.Add(currentEvent);
            currentEvent.OnBegin(firstTime);
        }

        // Update the current event
        if (currentEvent != null)
        {
            // If the event is done, remove it and move to the next one
            if (eventStack.Count > 0 && currentEvent == eventStack[0] && currentEvent.IsDone())
            {
                eventStack.RemoveAt(0);
                currentEvent.OnEnd();
                startedEvents.Remove(currentEvent);
                currentEvent = null;
            }
        }
    }


}
