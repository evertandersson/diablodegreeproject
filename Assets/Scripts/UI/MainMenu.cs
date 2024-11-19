using Events;
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
            // Create GameManager instance 
            var gameManager = GameManager.Instance;

            EventHandler.Main.RemoveEvent(this);


            SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
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
