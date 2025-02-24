using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Popup : EventHandler.GameEventBehaviour
    {
        protected bool isDone = false;
        protected CanvasGroup group = null;

        private float fadeSpeed = 5.0f;

        public static Stack<Popup> activePopups = new Stack<Popup>(); // A stack that keeps track of which popups are currently active

        public static event Action Pause;
        public static event Action UnPause;

        private void OnEnable()
        {
            group = GetComponent<CanvasGroup>();
            group.alpha = 0.0f;
            group.interactable = true;
            group.blocksRaycasts = true;
        }

        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);

            Pause?.Invoke();
            GameManager.IsPaused = true;
            PlayerManager.Instance.CharacterAnimator.speed = 0;

            isDone = false;
            gameObject.SetActive(true);
            group.interactable = true;
            group.blocksRaycasts = true;

            // Moves this popup to the bottom of the hierarchy to be in front of all other popups
            transform.SetAsLastSibling();

            if (PlayerManager.Instance != null)
            {
                PlayerManager.Instance.Agent.isStopped = true;
            }

            GameManager.StopAllEnemies(true);

            if (!activePopups.Contains(this))
            {
                activePopups.Push(this); // Push the popup onto the stack
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (group) group.alpha = Mathf.MoveTowards(group.alpha, isDone ? 0.0f : 1.0f, fadeSpeed * Time.deltaTime);
        }

        protected void ClearEvents(bool exitToMenu = false)
        {
            // Create a list to hold events to remove
            var eventsToRemove = new List<EventHandler.IEvent>();

            // Identify events to remove
            foreach (var ev in EventHandler.Main.EventStack)
            {
                if (!exitToMenu)
                {
                    if (ev is GameManager)
                    {
                        continue;
                    }
                }
                eventsToRemove.Add(ev);
            }

            // Remove the identified events from the stack
            foreach (var ev in eventsToRemove)
            {
                EventHandler.Main.EventStack.Remove(ev);
            }

            // Clear all popups
            activePopups.Clear();

            EventHandler.Main.ResetCurrentEvent();
        }

        public virtual void Restart()
        {
            PlayerManager.Instance.CurrentPlayerState = PlayerManager.State.Dead;
            SaveManager.Instance.Save();

            ClearEvents();

            // Reload the current scene
            LevelTransition.Instance.Load(SceneManager.GetActiveScene().name, this);
        }

        public virtual void OnOkay()
        {
            isDone = true;
            group.interactable = false;
        }
        public virtual void OnCancel()
        {
            isDone = true;
            group.interactable = false;
        }

        public override void OnEnd()
        {
            base.OnEnd();
            group.interactable = false;
            group.blocksRaycasts = false;
            group.alpha = 0.0f; 

            if (activePopups.Count > 0 && activePopups.Peek() == this)
            {
                activePopups.Pop(); // Remove the popup from the stack if it's the top one
            }

            if (activePopups.Count == 0)
            {
                UnPause?.Invoke();
                GameManager.IsPaused = false;
                PlayerManager.Instance.CharacterAnimator.speed = 1;
            }

            if (PlayerManager.Instance == null)
                return;

            GameManager.StopAllEnemies(false);

            PlayerManager.Instance.Agent.isStopped = false;

            if (PlayerManager.Instance.CurrentPlayerState == PlayerManager.State.Idle) SlotManager.Instance.SetFirstInLayer();            
        }   

        public override bool IsDone()
        {
            return isDone && group.alpha < 0.001f;
        }

        /* This function is used to check if the popup doesnt exist in the scene already,
         * if it does, it simply activates it. But if it doesn't, it creates a new instance of the object */
        public static T Create<T>() where T : Popup, EventHandler.IEvent
        {
            // Look for an existing object of type T in the scene
            T existingObject = FindFirstObjectByType<T>();

            if (existingObject != null)
            {
                // If the object exists, activate its GameObject
                existingObject.gameObject.SetActive(true);

                // Push the event to the EventHandler if it's not already in the stack
                if (!EventHandler.Main.EventStack.Contains(existingObject))
                {
                    EventHandler.Main.PushEvent(existingObject);
                }

                return existingObject; // Return the found object
            }
            else
            {
                // If no existing object, load the prefab
                GameObject prefab = Resources.Load<GameObject>("Prefabs/UI/" + typeof(T).Name);
                if (prefab == null)
                {
                    Debug.LogError($"Prefab not found: Prefabs/UI/{typeof(T).Name}");
                    return null;
                }

                // Instantiate the prefab
                GameObject go = Instantiate(prefab);

                // Get the T component
                T newInstance = go.GetComponent<T>();
                if (newInstance == null)
                {
                    Debug.LogError($"Component of type {typeof(T).Name} not found on prefab.");
                    return null;
                }

                // Push the new event to the EventHandler
                EventHandler.Main.PushEvent(newInstance);

                return newInstance; 
            }
        }

    }

}
