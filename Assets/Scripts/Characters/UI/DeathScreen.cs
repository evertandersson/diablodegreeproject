using Game;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
        // Create a list to hold events to remove
        var eventsToRemove = new List<EventHandler.IEvent>();

        // Identify events to remove
        foreach (var ev in EventHandler.Main.EventStack)
        {
            if (ev is not GameManager)
            {
                eventsToRemove.Add(ev);
            }
        }

        // Remove the identified events from the stack
        foreach (var ev in eventsToRemove)
        {
            EventHandler.Main.EventStack.Remove(ev);
        }

        activePopups.Clear();

        // Trigger end logic
        OnEnd();

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
