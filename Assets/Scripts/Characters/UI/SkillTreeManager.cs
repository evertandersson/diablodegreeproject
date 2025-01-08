using Game;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SkillTreeManager : Popup
{
    public static SkillTreeManager Instance;

    public List<SkillButton> allSkills;
    public List<SkillButton> unlockedSkills;

    public int skillPoints = 0;
    [SerializeField] private TextMeshProUGUI skillPointsText;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        allSkills = new List<SkillButton>(GetComponentsInChildren<SkillButton>(true));
        PlayerManager.Instance.levelSystem.OnLevelChanged += AddSkillPoint;

        foreach (SkillButton skillButton in allSkills)
        {
            if (skillButton.isUnlocked)
            {
                UnlockSkill(skillButton);
            }
        }

        UpdateSkillPointsText();
    }

    private void AddSkillPoint(object sender, System.EventArgs e)
    {
        skillPoints++;
        UpdateSkillPointsText();
    }

    private void RemoveSkillPoint()
    {
        skillPoints--;
        UpdateSkillPointsText();
    }

    private void UpdateSkillPointsText()
    {
        skillPointsText.text = "Skill points: " + skillPoints;
    }

    public bool CanUnlock(SkillButton skill)
    {
        if (skill.isUnlocked) return false;

        if (skill.previousSkillsNeeded.Length > 0)
        {
            foreach (SkillButton previousSkill in skill.previousSkillsNeeded)
            {
                if (!previousSkill.isUnlocked)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void UnlockSkill(SkillButton skill)
    {
        if (CanUnlock(skill) && skillPoints > 0)
        {
            skill.isUnlocked = true;
            RemoveSkillPoint();
            unlockedSkills.Add(skill); // Add to unlockedSkills, not allSkills
            foreach (SkillButton skillButton in allSkills)
            {
                skillButton.UpdateButtonState(skillButton);
            }
            PlayerManager.Instance.ApplySkillPoint(skill.skill);
        }
    }
}
