using UnityEngine;

namespace Game
{
    public class GameManager : EventHandler.GameEventBehaviour
    {
        private static GameManager _instance;

        #region Properties

        public static GameManager Instance
        {
            get
            {
                // Return the existing instance if it exists
                if (_instance != null) return _instance;

                // Create a new instance if none exists
                if (Application.isPlaying)
                {
                    GameObject go = new GameObject("GameManager");
                    DontDestroyOnLoad(go);
                    _instance = go.AddComponent<GameManager>();
                }

                return _instance;
            }
        }

        #endregion

        private void OnEnable()
        { 
            // Ensure there is only one instance of GameManager
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Register to event system after confirming singleton setup
            EventHandler.Main.PushEvent(this);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        public override bool IsDone()
        {
            return false;
        }
    }
}
