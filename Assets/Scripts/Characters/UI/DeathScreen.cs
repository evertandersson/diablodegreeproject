using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathScreen : Popup
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        restartButton.interactable = false;
        mainMenuButton.interactable = false;
    }

    public override void OnBegin(bool firstTime)
    {
        base.OnBegin(firstTime);

        SaveManager.Instance.Save(); // Save the game before reloading

        StartCoroutine(EnableButtons()); // Wait a second before activating buttons
    }

    public void GoToMainMenu()
    {
        SaveManager.Instance.Save();
        ClearEvents(true);
        Time.timeScale = 1.0f;
        LevelTransition.Instance.Load("MainMenu", this);
    }

    IEnumerator EnableButtons()
    {
        yield return new WaitForSeconds(0.5f);

        restartButton.interactable = true;
        mainMenuButton.interactable = true;
    }
}
