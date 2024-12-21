using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : Bar
{
    private TextMeshProUGUI experienceText;
    private TextMeshProUGUI levelText;
    private LevelSystem levelSystem;

    private void Awake()
    {
        experienceText = transform.Find("ExperienceText").GetComponent<TextMeshProUGUI>();
        levelText = transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
    }

    public void SetExperienceToNextLevel(int amount, int level)
    {
        slider.maxValue = amount;
        backgroundSlider.maxValue = amount;
        levelText.text = "Level " + level;
    }

    private void SetExperience(int experience, int experienceToNextLevel)
    {
        if (gameObject.activeSelf)
        {
            backgroundSlider.value = experience;
            experienceText.text = experience + " / " + experienceToNextLevel + " xp";
            StartCoroutine(SmoothFill(backgroundSlider, slider, 0.5f, 10));
        }
    }

    public void SetLevelSystem(LevelSystem levelSystem)
    {
        //Set the LevelSystem object
        this.levelSystem = levelSystem;

        //Set the experience to next level
        SetExperienceToNextLevel(levelSystem.GetExperienceToNextLevel(), levelSystem.GetCurrentLevel());
        //Set current experience
        SetExperience(levelSystem.GetExperience(), levelSystem.GetExperienceToNextLevel());

        //Subcribe to events
        levelSystem.OnExperienceChanged += LevelSystem_OnExperienceChanged;
        levelSystem.OnLevelChanged += LevelSystem_OnLevelChanged;
    }

    private void LevelSystem_OnLevelChanged(object sender, System.EventArgs e)
    {
        //Level changed, set new max experience and set current experience
        slider.value = levelSystem.GetExperience();
        SetExperienceToNextLevel(levelSystem.GetExperienceToNextLevel(), levelSystem.GetCurrentLevel());
        SetExperience(levelSystem.GetExperience(), levelSystem.GetExperienceToNextLevel());
    }

    private void LevelSystem_OnExperienceChanged(object sender, System.EventArgs e)
    {
        //Experience changed, set new current experience
        SetExperience(levelSystem.GetExperience(), levelSystem.GetExperienceToNextLevel());
    }
}
