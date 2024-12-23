using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SkillSO skill;
    public SkillButton[] previousSkillsNeeded;
    public bool isUnlocked;

    public Button button;
    public RawImage icon;
    public Text costText;
    private RectTransform rectTransform;

    private SkillTreeManager skillTreeManager;

    private void Start()
    {
        skillTreeManager = FindFirstObjectByType<SkillTreeManager>();
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        icon = GetComponentInChildren<RawImage>();
        icon.texture = skill.skillIcon;
        //costText.text = skill.skillCost.ToString();

        button.onClick.AddListener(() => UnlockSkill());
        UpdateButtonState(this);
    }

    public void UpdateButtonState(SkillButton skillButton)
    {
        if (EventHandler.Main.CurrentEvent is SkillTreeManager)
            button.interactable = skillTreeManager.CanUnlock(skillButton);
    }

    public void UnlockSkill()
    {
        skillTreeManager.UnlockSkill(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventHandler.Main.CurrentEvent is SkillTreeManager)
        {
            InfoWindow.Instance.ShowInfoWindow(transform.position, 
                rectTransform.rect.width, 
                rectTransform.rect.height, 
                skill.skillName, 
                skill.skillDescription);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InfoWindow.Instance.HideInfoWindow();
    }
}
