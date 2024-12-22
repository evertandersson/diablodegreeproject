using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Popup : EventHandler.GameEventBehaviour
    {
        protected bool isDone = false;
        private CanvasGroup group = null;

        private float fadeSpeed = 5.0f;

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

            isDone = false;
            gameObject.SetActive(true);
            group.interactable = true;
            group.blocksRaycasts = true;

            // Move this popup to the top of the hierarchy
            transform.SetAsLastSibling();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            group.alpha = Mathf.MoveTowards(group.alpha, isDone ? 0.0f : 1.0f, fadeSpeed * Time.deltaTime);
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
            group.alpha = 0.0f; // Fully fade out when done

            SlotManager.Instance.SetFirstInLayer();            
        }   

        public override bool IsDone()
        {
            return isDone && group.alpha < 0.001f;
        }

        public static T Create<T>() where T : MonoBehaviour, EventHandler.IEvent
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

                return existingObject; // Return the found instance
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
