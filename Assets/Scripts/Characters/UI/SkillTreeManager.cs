using Game;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeManager : Popup
{
    public static SkillTreeManager Instance;

    public List<SkillButton> allSkills;
    public List<SkillButton> unlockedSkills;

    public int skillPoints = 20;
    [SerializeField] private TextMeshProUGUI skillPointsText;

    public Transform lineParent;
    public Sprite lineSprite;
    public Transform startPoint;

    private HashSet<string> drawnLines = new HashSet<string>();

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        allSkills = new List<SkillButton>(GetComponentsInChildren<SkillButton>(true));
        PlayerManager.Instance.levelSystem.OnLevelChanged += AddSkillPoint;

        foreach (var unlockedSkill in SaveManager.Instance.skillsUnlockedList.serializableList)
        {
            // Find the matching skill in the allSkills list by id
            SkillButton skill = allSkills.Find(s => s.id == unlockedSkill.name);

            if (skill != null)
            {
                LoadSkillAtStart(skill);
            }
        }

        foreach (SkillButton skillButton in allSkills)
        {
            skillButton.UpdateButtonState(skillButton);
        }

        RedrawLines();
        UpdateSkillPointsText();
    }

    [ExecuteAlways]
    public void ViewTreeInEditor()
    {
        allSkills = new List<SkillButton>(GetComponentsInChildren<SkillButton>(true));
        RedrawLines();
    }

    private void DrawLines()
    {
        foreach (SkillButton skill in allSkills)
        {
            if (skill.neighbouringSkills.Length < 1)
            {
                DrawUniqueLine(skill, null);
            }

            foreach (SkillButton neighbourSkill in skill.neighbouringSkills)
            {
                DrawUniqueLine(skill, neighbourSkill);
            }
        }
    }

    private void DrawUniqueLine(SkillButton skill, SkillButton neighbourSkill)
    {
        if (neighbourSkill == null)
        {
            // Handle case where a skill is connected to startPoint
            string key = GetLineKey(skill.transform, startPoint);
            if (drawnLines.Contains(key)) return;

            DrawUILine(skill.transform, startPoint, skill.isUnlocked);
            drawnLines.Add(key);
            return;
        }

        string key2 = GetLineKey(skill.transform, neighbourSkill.transform);
        if (drawnLines.Contains(key2)) return;

        bool bothUnlocked = skill.isUnlocked && neighbourSkill.isUnlocked;
        DrawUILine(skill.transform, neighbourSkill.transform, bothUnlocked);
        drawnLines.Add(key2);
    }



    private string GetLineKey(Transform a, Transform b)
    {
        // Ensure key is the same regardless of order (A->B and B->A should be the same)
        return a.GetInstanceID() < b.GetInstanceID() ? $"{a.GetInstanceID()}_{b.GetInstanceID()}" : $"{b.GetInstanceID()}_{a.GetInstanceID()}";
    }

    private void DrawUILine(Transform start, Transform end, bool isUnlocked)
    {
        GameObject lineObj = new GameObject("SkillLine", typeof(Image));
        lineObj.transform.SetParent(lineParent, false);
        lineObj.transform.SetAsFirstSibling();

        Image lineImage = lineObj.GetComponent<Image>();
        lineImage.sprite = lineSprite;
        lineImage.color = isUnlocked ? Color.white : Color.grey;

        RectTransform rectTransform = lineObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;

        Vector3 startPos = start.position;
        Vector3 endPos = end.position;
        rectTransform.position = (startPos + endPos) / 2;

        float distance = Vector3.Distance(startPos, endPos);
        rectTransform.sizeDelta = new Vector2(distance * 0.6f, 5f);
        Vector3 dir = (endPos - startPos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void RedrawLines()
    {
        foreach (Transform child in lineParent)
        {
            if (child.name == "SkillLine")
            {
                Destroy(child.gameObject);
            }
        }

        drawnLines.Clear();
        DrawLines(); // Redraw with updated colors
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

        if (skill.neighbouringSkills.Length == 0)
            return true;

        foreach (SkillButton previousSkill in skill.neighbouringSkills)
        {
            if (previousSkill.isUnlocked)
            {
                return true;
            }
        }

        return false;
    }

    public void UnlockSkill(SkillButton skill, string id, bool saveToList)
    {
        if (CanUnlock(skill) && skillPoints > 0)
        {
            skill.isUnlocked = true;
            RemoveSkillPoint();
            unlockedSkills.Add(skill); // Add to unlockedSkills, not allSkills
            if (saveToList) SaveManager.Instance.AddObjectToList(id, SaveManager.Instance.skillsUnlockedList);
            foreach (SkillButton skillButton in allSkills)
            {
                skillButton.UpdateButtonState(skillButton);
            }
            PlayerManager.Instance.ApplySkillPoint(skill.skill);

            // Redraw lines to update colors
            RedrawLines();
        }
    }

    private void LoadSkillAtStart(SkillButton skill)
    {
        skill.isUnlocked = true;
        unlockedSkills.Add(skill); // Add to unlockedSkills, not allSkills
        PlayerManager.Instance.ApplySkillPoint(skill.skill);
    }
}
