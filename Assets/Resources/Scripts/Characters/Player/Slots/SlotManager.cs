using System.Collections.Generic;
using UnityEngine;

public class SlotManager : MonoBehaviour
{
    public List<ActionSlot> actionSlots;
    public Inventory inventory;

    private void Awake()
    {
        actionSlots = new List<ActionSlot>(GetComponentsInChildren<ActionSlot>());
        SetUpSlots(); // Set up slots for UI and initial configuration
        var savedActionSlots = SaveManager.Instance.LoadSavedData("savefile.json").actionSlots;
        SetUpSavedSlots(savedActionSlots); // Pass saved data to properly populate slots
    }

    public void SetUpSlots()
    {
        // Loop through and set up each action slot
        for (int i = 0; i < actionSlots.Count; i++)
        {
            actionSlots[i].indexText.text = (i + 1).ToString();

            if (actionSlots[i].item != null)
            {
                if (actionSlots[i].item is AttackTypeSO attackType)
                {
                    // Initialize AttackInstance for this slot
                    if (actionSlots[i].attackInstance == null)
                    {
                        actionSlots[i].attackInstance = new AttackInstance(attackType);
                    }
                }

                actionSlots[i].artwork.texture = actionSlots[i].item.itemIcon;
                actionSlots[i].artwork.enabled = true;
            }
            else
            {
                actionSlots[i].artwork.enabled = false; // Hide artwork if no item
            }
        }
    }

    // Set up action slots based on saved data
    public void SetUpSavedSlots(List<SaveManager.ActionSlotSaveData> savedActionSlots)
    {
        // Ensure you're modifying the actionSlots list, not clearing it
        for (int i = 0; i < savedActionSlots.Count && i < actionSlots.Count; i++)
        {
            var savedData = savedActionSlots[i];
            var item = FindItemSOByName(savedData.itemName);

            if (item != null)
            {
                actionSlots[i].item = item; // Assign item to the action slot
                ApplyUpgradesToSlot(actionSlots[i], savedData.upgradeData);
            }
            else
            {
                Debug.LogWarning("Item not found for action slot: " + savedData.itemName);
            }
        }
    }



    private ItemSO FindItemSOByName(string name)
    {
        // Lookup logic (to be implemented as required)
        return inventory.FindItemSOByName(name);
    }

    private void ApplyUpgradesToSlot(ActionSlot slot, SaveManager.UpgradeSaveData upgradeData)
    {
        if (slot.item is AttackTypeSO attackType)
        {
            // Apply the upgrades to the attack type instance
            attackType.attackInstance.currentDamage = upgradeData.damageBonus;
            attackType.attackInstance.currentCooldown = upgradeData.cooldownReduction;
            attackType.attackInstance.currentProjectileCount = upgradeData.extraFireballs;
            attackType.attackInstance.currentFollowEnemies = upgradeData.followEnemies;

            // Update slot artwork based on the item
            slot.artwork.texture = slot.item.itemIcon;
            slot.artwork.enabled = true;  // Ensure the artwork is visible
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
                    actionSlot.RefillCooldown(actionItem.cooldown, actionItem.timerCooldown);
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
