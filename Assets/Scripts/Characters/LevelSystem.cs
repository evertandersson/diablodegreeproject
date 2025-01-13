using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelSystem
{
    public event System.EventHandler OnExperienceChanged;
    public event System.EventHandler OnLevelChanged;

    private int level;
    private int experience;
    private int experienceToNextLevel;
    private float experienceMultiplier = 2.5f;

    public LevelSystem() 
    {
        level = 1;
        experience = 0;
        experienceToNextLevel = 500;
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        if (experience >= experienceToNextLevel)
        {
            level++;
            experience -= experienceToNextLevel;
            experienceToNextLevel = (int)(experienceToNextLevel * experienceMultiplier);
            if (OnLevelChanged != null) OnLevelChanged(this, EventArgs.Empty);
        }
        if (OnExperienceChanged != null) OnExperienceChanged(this, EventArgs.Empty);
    }

    public void SetLevel(int lvl)
    {
        level = lvl;
        for (int i = 0; i < level - 1; i++)
        {
            experienceToNextLevel = (int)(experienceToNextLevel * experienceMultiplier);
        }
        if (OnLevelChanged != null) OnLevelChanged(this, EventArgs.Empty);
    }

    public int GetCurrentLevel()
    {
        return level;
    }

    public int GetExperience()
    {
        return experience;
    }

    public int GetExperienceToNextLevel()
    {
        return experienceToNextLevel;
    }
}
