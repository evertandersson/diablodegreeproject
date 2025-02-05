using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Game
{
    public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public ItemSO item;  // The item this slot holds
        public int itemAmount;  // The amount of this item
        public TextMeshProUGUI amountText;  // Text showing the item amount
        public RawImage artwork;  // Image of the item

        private CanvasGroup canvasGroup;  // To manage raycast blocking during drag
        private Transform originalParent;  // Slot's original parent
        private RectTransform artworkRectTransform;  // To manipulate artwork's position
        private float originalZPosition;  // To store the original z position
        private RectTransform rectTransform;

        private Canvas artworkCanvas;  // Canvas for the artwork

        private void Awake()
        {
            canvasGroup = GetComponentInChildren<CanvasGroup>();
            artworkRectTransform = artwork.GetComponent<RectTransform>();

            // Store the original Z position of the artwork
            originalZPosition = artworkRectTransform.localPosition.z;

            // Ensure artwork does not block raycasts during drag
            artwork.raycastTarget = false;

            artworkCanvas = artwork.GetComponentInParent<Canvas>();

            rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            CheckIfItemNull();
        }

        public virtual void CheckIfItemNull()
        {
            if (item == null)
            {
                HideItemUI();
            }
            else
            {
                UpdateItemAmountText();
            }
        }

        private void HideItemUI()
        {
            if (amountText) amountText.enabled = false;
            artwork.enabled = false;
        }

        public void UpdateItemAmountText()
        {
            if (itemAmount <= 0)
            {
                item = null;
                HideItemUI();
            }
            if (item != null)
            {
                artwork.enabled = true;
                if (amountText) amountText.text = itemAmount.ToString();
                artwork.texture = item.itemIcon;
                if (amountText) amountText.enabled = itemAmount > 1;
            }
            else
            {
                HideItemUI();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (IsInteractable())
            {
                originalParent = artwork.transform.parent;
                artwork.transform.SetParent(artworkCanvas.transform);
                canvasGroup.blocksRaycasts = false;
                artwork.raycastTarget = false; // Ensure it doesn't block raycasts
                EquipmentManager.Instance.ShowEquipmentSlots();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (IsInteractable())
            {
                Vector3 newPosition = eventData.position;
                newPosition.z = originalZPosition;
                artworkRectTransform.position = newPosition;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (IsInteractable())
            {
                artwork.transform.SetParent(originalParent);

                Vector3 finalLocalPosition = artworkRectTransform.localPosition;
                finalLocalPosition.z = originalZPosition;
                artworkRectTransform.localPosition = finalLocalPosition;

                InventorySlot targetSlot = null;

                // Try to get the slot from the pointerEnter first
                if (eventData.pointerEnter != null)
                {
                    targetSlot = eventData.pointerEnter.GetComponent<InventorySlot>();
                }

                // If pointerEnter is null, perform a raycast to find the target
                if (targetSlot == null)
                {
                    List<RaycastResult> results = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(eventData, results);

                    foreach (RaycastResult result in results)
                    {
                        targetSlot = result.gameObject.GetComponent<InventorySlot>();
                        if (targetSlot != null)
                        {
                            break;
                        }
                    }
                }

                if (targetSlot != null && targetSlot != this)
                {
                    // Prevent non-Action items from being placed into ActionSlot
                    if (targetSlot is ActionSlot && !(item is ActionItemSO))
                    {
                        ReturnToOriginalSlot(); // Prevent move to ActionSlot if it's not an ActionItemSO
                        return;
                    }

                    // Handle ActionSlot logic
                    if (targetSlot is ActionSlot)
                    {
                        if (item is ActionItemSO)
                        {
                            if (targetSlot.item != null)
                                SwapItems(targetSlot);
                            else
                                MoveItem(targetSlot);
                        }
                        else
                        {
                            ReturnToOriginalSlot(); // If item is not an ActionItemSO, return to original slot
                        }
                    }
                    else if (targetSlot is EquipmentSlot equipmentSlot)
                    {
                        // Check if both items are of the same equipment type
                        if (item is EquipmentSO equipment && equipment.equipmentType == equipmentSlot.equipmentType)
                        {
                            if (targetSlot.item != null)
                            {
                                EquipmentSO targetEquip = targetSlot.item as EquipmentSO;
                                // Ensure both items are of the same type before swapping
                                if (equipment.equipmentType == targetEquip.equipmentType)
                                {
                                    // Unequip the current item and target item before swapping
                                    if (this.item is EquipmentSO thisEquipment)
                                    {
                                        PlayerManager.Instance.SetEquipment(thisEquipment, -1); // Unequip current item
                                    }

                                    if (targetSlot.item is EquipmentSO targetEquipment)
                                    {
                                        PlayerManager.Instance.SetEquipment(targetEquipment, -1); // Unequip target item
                                    }

                                    // Swap items
                                    SwapItems(targetSlot);
                                }
                                else
                                {
                                    ReturnToOriginalSlot(); // Return to original slot if the types don't match
                                }
                            }
                            else
                            {
                                MoveItem(targetSlot); // If the target is empty, move the item
                            }

                            // Reapply stats for the newly equipped item (both slots)
                            if (this.item is EquipmentSO newItem)
                            {
                                PlayerManager.Instance.SetEquipment(newItem, 1); // Equip the new item
                            }

                            if (targetSlot.item is EquipmentSO targetNewItem)
                            {
                                PlayerManager.Instance.SetEquipment(targetNewItem, 1); // Equip swapped item stats
                            }
                        }
                        else
                        {
                            ReturnToOriginalSlot(); // Return to original slot if the equipment types don't match
                        }
                    }
                    else
                    {
                        // Handle other inventory slots (ActionSlot or general)
                        if (targetSlot.item != null)
                        {
                            if (this is EquipmentSlot && (item as EquipmentSO)?.equipmentType != (targetSlot.item as EquipmentSO)?.equipmentType)
                            {
                                ReturnToOriginalSlot();
                                return;
                            }

                            if ((this is ActionSlot && !(targetSlot.item is ActionItemSO)) ||
                                (this is EquipmentSlot && !(targetSlot.item is EquipmentSO)))
                            {
                                ReturnToOriginalSlot();
                            }
                            else
                            {
                                SwapItems(targetSlot);
                            }
                        }
                        else
                        {
                            MoveItem(targetSlot);
                        }
                    }

                    PlayerManager.Instance.UpdateActionSlots();
                }
                else
                {
                    // If no valid target slot, return to original slot
                    ReturnToOriginalSlot();
                }

                canvasGroup.blocksRaycasts = true;
                artworkRectTransform.anchoredPosition = Vector3.zero;
                artworkRectTransform.rotation = Quaternion.identity;
            }
        }




        // Return item to its original slot
        private void ReturnToOriginalSlot()
        {
            artwork.transform.SetParent(originalParent);
            artworkRectTransform.localPosition = Vector3.zero;
            artworkRectTransform.rotation = Quaternion.identity;
        }


        private void SwapItems(InventorySlot targetSlot)
        {
            if (this.item == targetSlot.item && item.isStackable)
            {
                int totalAmount = this.itemAmount + targetSlot.itemAmount;
                targetSlot.itemAmount = totalAmount;
                targetSlot.UpdateItemAmountText();

                RemoveItem();
                return;
            }

            // Unequip both the current item and the target item
            if (this is EquipmentSlot && this.item is EquipmentSO thisEquipment)
            {
                PlayerManager.Instance.SetEquipment(thisEquipment, -1); // Unequip current item stats
            }
            if (targetSlot is EquipmentSlot && targetSlot.item is EquipmentSO targetEquipment)
            {
                PlayerManager.Instance.SetEquipment(targetEquipment, -1); // Unequip target item stats
            }

            // Swap the items in the slots
            ItemSO tempItem = targetSlot.item;
            int tempAmount = targetSlot.itemAmount;

            targetSlot.item = this.item;
            targetSlot.itemAmount = this.itemAmount;
            targetSlot.UpdateItemAmountText();

            this.item = tempItem;
            this.itemAmount = tempAmount;
            this.UpdateItemAmountText();

            // Reapply stats for the newly equipped items
            if (this is EquipmentSlot && this.item is EquipmentSO thisNewEquipment)
            {
                PlayerManager.Instance.SetEquipment(thisNewEquipment, 1); // Equip new item stats
            }
            if (targetSlot is EquipmentSlot && targetSlot.item is EquipmentSO targetNewEquipment)
            {
                PlayerManager.Instance.SetEquipment(targetNewEquipment, 1); // Equip swapped item stats
            }
        }

        private void MoveItem(InventorySlot targetSlot)
        {
            // If the current slot is an EquipmentSlot and has an equipment item, unequip it
            if (this is EquipmentSlot && this.item is EquipmentSO equipment)
            {
                PlayerManager.Instance.SetEquipment(equipment, -1); // Unequip stats
            }

            // Move the item to the target slot
            targetSlot.item = this.item;
            targetSlot.itemAmount = this.itemAmount;
            targetSlot.UpdateItemAmountText();

            RemoveItem();
        }

        public void RemoveItem()
        {
            this.item = null;
            this.itemAmount = 0;
            this.UpdateItemAmountText();
        }

        //Handle hovering mouse over item
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsInteractable())
            {
                InfoWindow.Instance.ShowEquipmentInfoWindow(transform.position, 
                    rectTransform.rect.width, 
                    rectTransform.rect.height,
                    item);
            }
        }

        //Handle exiting hover mouse over item
        public void OnPointerExit(PointerEventData eventData)
        {
            InfoWindow.Instance.HideInfoWindow();   
        }

        private bool IsInteractable()
        {
            return item != null && EventHandler.Main.CurrentEvent is Inventory;
        }
    }
}