using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionSlot : InventorySlot, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public TextMeshProUGUI indexText;
    public Image cooldownImage;

    private void Start()
    {
        artwork.raycastTarget = true;
        amountText.raycastTarget = true;
        cooldownImage.raycastTarget = true;
        cooldownImage.fillAmount = 0;
    }

    public void RefillCooldown(float cooldown, float timerCooldown)
    {
        cooldownImage.fillAmount = 1 - (timerCooldown / cooldown);
    }
}
