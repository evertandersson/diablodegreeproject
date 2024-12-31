using Game;
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
        restartButton.interactable = true;
        mainMenuButton.interactable = true;
    }

    public void Restart()
    {
        EventHandler.Main.EventStack.Remove(this);
        OnEnd();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
