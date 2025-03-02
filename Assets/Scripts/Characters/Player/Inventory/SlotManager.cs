using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class SlotManager : MonoBehaviour
    {
        public static SlotManager Instance;

        public ActionSlot[] actionSlots;
        [SerializeField] private RectTransform actionPanel;

        [SerializeField] private Transform[] hideableChildren;

        private void Awake()
        {
            Instance = this;

            SetFirstInLayer();

            DialougeManager.startDialouge += HideUI;
            DialougeManager.endDialouge += ShowUI;
        }


        private void OnDisable()
        {
            DialougeManager.startDialouge -= HideUI;
            DialougeManager.endDialouge -= ShowUI;
        }

        private void HideUI()
        {
            ToggleUI(false);
        }

        private void ShowUI()
        {
            ToggleUI(true);
        }

        private void ToggleUI(bool show)
        {
            foreach (Transform child in hideableChildren)
            {
                if (child != null)
                {
                    child.gameObject.SetActive(show);
                }
            }
        }

        public void GetActionSlots()
        {
            actionSlots = GetComponentsInChildren<ActionSlot>();
        }

        public void SetFirstInLayer()
        {
            actionPanel.SetAsLastSibling();
        }

        public void SetUpSlots()
        {
            for (int i = 0; i < actionSlots.Length; i++)
            {
                actionSlots[i].indexText.text = (i + 1).ToString();

                if (actionSlots[i].item != null)
                {
                    actionSlots[i].artwork.texture = actionSlots[i].item.itemIcon;
                    actionSlots[i].artwork.enabled = true;
                }
                else
                {
                    actionSlots[i].artwork.enabled = false; // Hide artwork if no item
                }
            }
        }

        public void HandleCooldowns()
        {
            foreach (var actionSlot in actionSlots)
            {
                // Skip empty slots
                if (actionSlot.item == null) continue;

                // Cast the item to ActionItemSO (if applicable)
                if (actionSlot.item is ActionItemSO actionItem)
                {
                    // Increment cooldown timer
                    if (actionItem.timerCooldown < actionItem.cooldown)
                    {
                        actionItem.timerCooldown += Time.deltaTime;

                        // Update the UI for the cooldown
                        actionSlot.RefillCooldown(StatsCalculator.CalculateAttackSpeed(actionItem.cooldown), actionItem.timerCooldown);
                    }
                    else
                    {
                        // Cooldown complete, reset UI
                        actionSlot.cooldownImage.fillAmount = 0;
                    }
                }
            }
        }
    }
}