using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class MainMenu : EventHandler.GameEventBehaviour
    {
        [SerializeField] private Texture2D cursorTexture;
        private Vector2 hotspot = Vector2.zero;

        private void OnEnable()
        {
            Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
            EventHandler.Main.PushEvent(this);
        }

        public override bool IsDone()
        {
            return false;
        }

        public void OnNewGame()
        {
            SaveManager.Instance.NewGame();
            
            // Create GameManager instance 
            var gameManager = GameManager.Instance;

            EventHandler.Main.RemoveEvent(this);
            gameManager.EnableGameManager();

            LevelTransition.Instance.Load("TheDungeon", this);
        }

        public void OnLoadGame()
        {
            // Create GameManager instance 
            var gameManager = GameManager.Instance;

            EventHandler.Main.RemoveEvent(this);
            gameManager.EnableGameManager();

            LevelTransition.Instance.Load(SaveManager.Instance.GetLatestSavedScene(), this);
        }
        
        public void OnOptions()
        {
            Popup.Create<OptionsMenu>();
        }

        public void OnQuit()
        {
            
        }
    }
}
