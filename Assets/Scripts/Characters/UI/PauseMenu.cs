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
            //Time.timeScale = 0.0f;
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
            //Time.timeScale = 1.0f;
        }

        public override bool IsDone()
        {
            return isDone;
        }

        public void Resume()
        {
            isDone = true;
        }

        public override void Restart()
        {
            Time.timeScale = 1.0f;
            base.Restart();
        }

        public void GoToMainMenu()
        {
            SaveManager.Instance.Save();
            ClearEvents(true);
            //Time.timeScale = 1.0f;
            LevelTransition.Instance?.Load("MainMenu", this);
        }
    }

}
