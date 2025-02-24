using Autodesk.Fbx;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class GameManager : EventHandler.GameEventBehaviour
    {
        private static GameManager _instance;

        private static Enemy[] allEnemies;

        public static Dictionary<string, Vector3> spawnPositionAtLevel = new Dictionary<string, Vector3>() { 
            { "TheDungeon", new Vector3(-38f, -5f, -30f) },
            { "TestScene", new Vector3(41f, 0f, -42f) } 
        };

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

        public void EnableGameManager()
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

        // Called when restarting a level and wanting to get the spawn pos at this level
        public static Vector3 GetSpawnPositionAtLevel(string level)
        {
            //string level = SceneManager.GetActiveScene().name;
            Vector3 spawnPos = spawnPositionAtLevel[level];
            return spawnPos;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (allEnemies == null)
            {
                allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
            }

            if (PlayerManager.Instance != null)
            {
                PlayerManager.Instance?.OnUpdate();
                PlayerManager.Instance?.OnFixedUpdate();
            }

            foreach(Enemy enemy in allEnemies)
            {
                enemy.EnemyEventHandler.CurrentEvent?.OnUpdate();
            }
        }

        public override bool IsDone()
        {
            return false;
        }
    }
}
