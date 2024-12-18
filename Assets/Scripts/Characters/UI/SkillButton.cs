using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public SkillSO skill;
    public SkillButton[] previousSkillsNeeded;
    public bool isUnlocked;

    public Button button;
    public RawImage icon;
    public Text costText;

    private SkillTreeManager skillTreeManager;

    private void Start()
    {
        skillTreeManager = FindFirstObjectByType<SkillTreeManager>();
        button = GetComponent<Button>();
        icon = GetComponentInChildren<RawImage>();
        icon.texture = skill.skillIcon;
        //costText.text = skill.skillCost.ToString();

        button.onClick.AddListener(() => UnlockSkill());
        UpdateButtonState(this);
    }

    public void UpdateButtonState(SkillButton skillButton)
    {
        button.interactable = skillTreeManager.CanUnlock(skillButton);
    }

    public void UnlockSkill()
    {
        skillTreeManager.UnlockSkill(this);
    }
}
