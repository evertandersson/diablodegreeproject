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

    public void Restart()
    {
        ClearEvents();

        // Trigger on end
        OnEnd();

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator EnableButtons()
    {
        yield return new WaitForSeconds(0.5f);

        restartButton.interactable = true;
        mainMenuButton.interactable = true;
    }
}
