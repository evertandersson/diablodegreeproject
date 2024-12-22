using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public interface IEvent
    {
        void OnBegin(bool firstTime);
        void OnUpdate();
        void OnEnd();
        bool IsDone();
    }

    public abstract class GameEventBehaviour : MonoBehaviour, IEvent
    {
        public virtual void OnBegin(bool firstTime) { }
        public virtual void OnUpdate() { }
        public virtual void OnEnd() { }
        public virtual bool IsDone() { return true; }
    }

    private static EventHandler main;

    private List<IEvent> eventStack = new List<IEvent>();
    private HashSet<IEvent> startedEvents = new HashSet<IEvent>();
    private IEvent currentEvent;

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

    private static List<EventHandler> eventHandlers = new List<EventHandler>();

    #region Properties

    public IEvent CurrentEvent => currentEvent;
    public List<IEvent> EventStack => eventStack;

    #endregion

    public static EventHandler CreateEventHandler()
    {
        GameObject go = new GameObject("EventHandler_" + eventHandlers.Count);
        EventHandler newHandler = go.AddComponent<EventHandler>();
        eventHandlers.Add(newHandler);
        return newHandler;
    }

    public void PushEvent(IEvent evt)
    {
        if (evt != null)
        {
            eventStack.RemoveAll(e => e == evt);
            eventStack.Insert(0, evt);
            if (currentEvent != null && currentEvent != evt)
            {
                currentEvent = null;
            }
        }
    }

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
        UpdateEvents();
    }

    private void UpdateEvents()
    {
        if (eventStack.Count == 0) return;

        if (currentEvent == null)
        {
            currentEvent = eventStack[0];
            bool firstTime = !startedEvents.Contains(currentEvent);
            startedEvents.Add(currentEvent);
            currentEvent.OnBegin(firstTime);
        }

        if (currentEvent != null)
        {
            currentEvent.OnUpdate();

            if (eventStack.Count > 0 && currentEvent == eventStack[0] && currentEvent.IsDone())
            {
                eventStack.RemoveAt(0);
                currentEvent.OnEnd();
                startedEvents.Remove(currentEvent);
                currentEvent = null;
            }
        }
    }

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
