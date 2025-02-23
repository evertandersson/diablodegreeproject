using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class MainMenu : EventHandler.GameEventBehaviour
    {
        private void OnEnable()
        {
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

            LevelTransition.Instance.Load("TheDungeon");
        }

        public void OnLoadGame()
        {
            // Create GameManager instance 
            var gameManager = GameManager.Instance;

            EventHandler.Main.RemoveEvent(this);
            gameManager.EnableGameManager();

            LevelTransition.Instance.Load("TheDungeon");
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
