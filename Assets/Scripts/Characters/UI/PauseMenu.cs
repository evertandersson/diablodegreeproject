using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class PauseMenu : Popup
{
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        private void Awake()
        {
            resumeButton.interactable = false;
            restartButton.interactable = false;
            mainMenuButton.interactable = false;
        }

        public override void OnBegin(bool firstTime)
        {
            base.OnBegin(firstTime);
            Time.timeScale = 0.0f;
            resumeButton.interactable = true;
            restartButton.interactable = true;
            mainMenuButton.interactable = true;
            group.alpha = 1.0f;

        }

        public override void OnEnd()
        {
            base.OnEnd();
            resumeButton.interactable = false;
            restartButton.interactable = false;
            mainMenuButton.interactable = false;
            Time.timeScale = 1.0f;
        }

        public override bool IsDone()
        {
            return isDone;
        }

        public void Resume()
        {
            isDone = true;
        }

        public void Restart()
        {
            ClearEvents();

            OnEnd();

            // Reload the current scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        public void GoToMainMenu()
        {
            ClearEvents();
            Time.timeScale = 1.0f;
            SceneManager.LoadScene("MainMenu");
        }
    }

}
