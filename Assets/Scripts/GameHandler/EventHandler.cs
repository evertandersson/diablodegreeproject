using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    // Interface for events
    public interface IEvent
    {
        void OnBegin(bool firstTime); // Called on start
        void OnUpdate(); // Called every frame
        void OnEnd(); // Calls at the end of event
        bool IsDone(); // Checks if event is finished
    }

    // Abstract class that is used as base for event behaviors
    public abstract class GameEventBehaviour : MonoBehaviour, IEvent
    {
        public virtual void OnBegin(bool firstTime) { }
        public virtual void OnUpdate() { }
        public virtual void OnEnd() { }
        public virtual bool IsDone() { return true; }
    }

    private static EventHandler main; // Singleton instance of the main event handler

    private List<IEvent> eventStack = new List<IEvent>(); // Stack to store active events
    private HashSet<IEvent> startedEvents = new HashSet<IEvent>(); // Tracks started events
    private IEvent currentEvent; // Currently active event

    public static EventHandler Main
    {
        get
        {
            if (main == null && Application.isPlaying)
            {
                GameObject go = new GameObject("MainEventHandler");
                DontDestroyOnLoad(go);
                main = go.AddComponent<EventHandler>();
            }
            return main;
        }
    }

    private static List<EventHandler> eventHandlers = new List<EventHandler>(); // List of all event handlers created, uses eventhandlers for enemy as well

    #region Properties

    public IEvent CurrentEvent => currentEvent;  // Get the currently active event
    public List<IEvent> EventStack => eventStack; // Get the event stack

    #endregion

    // Creates a new event handler instance and adds it to the list
    public static EventHandler CreateEventHandler()
    {
        GameObject go = new GameObject("EventHandler_" + eventHandlers.Count);
        EventHandler newHandler = go.AddComponent<EventHandler>();
        eventHandlers.Add(newHandler);
        return newHandler;
    }

    public void ResetCurrentEvent()
    {
        currentEvent = eventStack.Count > 0 ? currentEvent = eventStack[0] : null;
    }

    // Pushes an event onto the stack, ensuring no duplicates
    public void PushEvent(IEvent evt)
    {
        if (evt != null)
        {
            eventStack.RemoveAll(e => e == evt);
            eventStack.Insert(0, evt); // Add the event to the front of the stack
            if (currentEvent != null && currentEvent != evt)
            {
                currentEvent = null; // Reset the current event if it's different
            }
        }
    }

    // Removes an event from the stack, calling OnEnd() if necessary
    public void RemoveEvent(IEvent evt)
    {
        if (evt == null || !eventStack.Contains(evt)) return;

        if (evt == currentEvent || startedEvents.Contains(evt))
        {
            evt.OnEnd();
        }

        eventStack.Remove(evt);
    }

    private void Update()
    {
        UpdateEvents(); // Updates events every frame
    }

    // Handles the logic of managing events in the stack
    private void UpdateEvents()
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
            currentEvent.OnUpdate();

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

    // Debug UI to display event stack in the Unity Editor
    private void OnGUI()
    {
        if (this != Main) return;
 
        #if UNITY_EDITOR
        const float LINE_HEIGHT = 32.0f;
        GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.7f);
        Rect r = new Rect(0, 0, 250.0f, LINE_HEIGHT * eventStack.Count);
        GUI.DrawTexture(r, Texture2D.whiteTexture);
 
        Rect line = new Rect(10, 0, r.width - 20, LINE_HEIGHT);
        for (int i = 0; i < eventStack.Count; i++)
        {
            GUI.color = eventStack[i] == currentEvent ? Color.green : Color.white;
            GUI.Label(line, "#" + i + ": " + eventStack[i].ToString(), i == 0 ? UnityEditor.EditorStyles.boldLabel : UnityEditor.EditorStyles.label);
            line.y += line.height;
        }
        #endif
    }
}
