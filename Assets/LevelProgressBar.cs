using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    private TextMeshProUGUI levelText;
    [SerializeField] private Slider experienceBar;
    [SerializeField] private Slider backgroundSlider;
    private LevelSystem levelSystem;

    private void Awake()
    {
        levelText = transform.Find("LevelText").GetComponent<TextMeshProUGUI>();
        experienceBar = GetComponent<Slider>();
        backgroundSlider = transform.Find("Background Fill Area").GetComponent<Slider>();
    }

    public void SetExperienceToNextLevel(int amount)
    {
        experienceBar.maxValue = amount;
        backgroundSlider.maxValue = amount;
    }

    private void SetExperience(int experience, int experienceToNextLevel)
    {
        if (gameObject.activeSelf)
        {
            backgroundSlider.value = experience;
            levelText.text = experience + " / " + experienceToNextLevel + " xp";
            StartCoroutine(SetBackgroundSlider());
        }
    }

    public void SetLevelSystem(LevelSystem levelSystem)
    {
        //Set the LevelSystem object
        this.levelSystem = levelSystem;

        //Set the experience to next level
        SetExperienceToNextLevel(levelSystem.GetExperienceToNextLevel());
        //Set current experience
        SetExperience(levelSystem.GetExperience(), levelSystem.GetExperienceToNextLevel());

        //Subcribe to events
        levelSystem.OnExperienceChanged += LevelSystem_OnExperienceChanged;
        levelSystem.OnLevelChanged += LevelSystem_OnLevelChanged;
    }

    private void LevelSystem_OnLevelChanged(object sender, System.EventArgs e)
    {
        //Level changed, set new max experience and set current experience
        experienceBar.value = levelSystem.GetExperience();
        SetExperienceToNextLevel(levelSystem.GetExperienceToNextLevel());
        SetExperience(levelSystem.GetExperience(), levelSystem.GetExperienceToNextLevel());
    }

    private void LevelSystem_OnExperienceChanged(object sender, System.EventArgs e)
    {
        //Experience changed, set new current experience
        SetExperience(levelSystem.GetExperience(), levelSystem.GetExperienceToNextLevel());
    }

    private IEnumerator SetBackgroundSlider()
    {
        yield return new WaitForSeconds(0.5f);

        while (experienceBar.value < backgroundSlider.value)
        {
            experienceBar.value = Mathf.MoveTowards(experienceBar.value, backgroundSlider.value, 10f * Time.deltaTime);
            yield return null;
        }

    }
}
