using Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButton : Loadable, IPointerEnterHandler, IPointerExitHandler
{
    public SkillSO skill;
    public SkillButton[] previousSkillsNeeded;
    public bool isUnlocked;

    public Button button;
    public RawImage icon;
    public Text costText;
    private RectTransform rectTransform;

    private SkillTreeManager skillTreeManager;

    protected override void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>(); // Try to assign it dynamically if not assigned
        }
        skillTreeManager = FindFirstObjectByType<SkillTreeManager>();
        rectTransform = GetComponent<RectTransform>();
        icon = GetComponentInChildren<RawImage>();
        icon.texture = skill.skillIcon;
        //costText.text = skill.skillCost.ToString();
        button.onClick.AddListener(() => UnlockSkill(true));
    }

    protected override void Load()
    {
        //Do nothing
    }

    public void UpdateButtonState(SkillButton skillButton)
    {
        //if (EventHandler.Main.CurrentEvent is SkillTreeManager)
        button.interactable = skillTreeManager.CanUnlock(skillButton);
    }

    public void UnlockSkill(bool saveToList)
    {
        skillTreeManager.UnlockSkill(this);
        if (saveToList) SaveManager.Instance.AddObjectToList(id, SaveManager.Instance.skillsUnlockedList);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventHandler.Main.CurrentEvent is SkillTreeManager)
        {
            InfoWindow.Instance.ShowSkillInfoWindow(transform.position, 
                rectTransform.rect.width, 
                rectTransform.rect.height, 
                skill);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        InfoWindow.Instance.HideInfoWindow();
    }

}
