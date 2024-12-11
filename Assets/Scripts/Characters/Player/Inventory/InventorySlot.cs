using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace Game
{
    public class InventorySlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public ItemSO item;  // The item this slot holds
        public int itemAmount;  // The amount of this item
        public TextMeshProUGUI amountText;  // Text showing the item amount
        public RawImage artwork;  // Image of the item

        private CanvasGroup canvasGroup;  // To manage raycast blocking during drag
        private Transform originalParent;  // Slot's original parent
        private RectTransform artworkRectTransform;  // To manipulate artwork's position
        private float originalZPosition;  // To store the original z position

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
            amountText.enabled = false;
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
                amountText.text = itemAmount.ToString();
                artwork.texture = item.itemIcon;
                amountText.enabled = itemAmount > 1;
            }
            else
            {
                HideItemUI();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (item != null)
            {
                originalParent = artwork.transform.parent;
                artwork.transform.SetParent(artworkCanvas.transform);
                canvasGroup.blocksRaycasts = false;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (item != null)
            {
                Vector3 newPosition = eventData.position;
                newPosition.z = originalZPosition;
                artworkRectTransform.position = newPosition;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (item != null)
            {
                artwork.transform.SetParent(originalParent);

                Vector3 finalLocalPosition = artworkRectTransform.localPosition;
                finalLocalPosition.z = originalZPosition;
                artworkRectTransform.localPosition = finalLocalPosition;

                // Initialize targetSlot as null
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
                        // Look for an InventorySlot or ActionSlot in the raycast results
                        targetSlot = result.gameObject.GetComponent<InventorySlot>();
                        if (targetSlot != null)
                        {
                            break;
                        }
                    }
                }

                if (targetSlot != null && targetSlot != this)
                {
                    // If the target is an ActionSlot and the item is an ActionItem
                    if (targetSlot is ActionSlot && item is ActionItemSO)
                    {
                        if (targetSlot.item != null)
                            SwapItems(targetSlot);
                        else
                            MoveItem(targetSlot);
                    }
                    // Handle non-ActionSlot cases
                    else
                    {
                        if (targetSlot.item != null)
                            SwapItems(targetSlot);
                        else
                            MoveItem(targetSlot);
                    }
                    PlayerManager.Instance.UpdateActionSlots();
                }

                canvasGroup.blocksRaycasts = true;
                artworkRectTransform.anchoredPosition = Vector3.zero;
                artworkRectTransform.rotation = Quaternion.identity;
            }

        }



        private void SwapItems(InventorySlot targetSlot)
        {
            if (this.item == targetSlot.item)
            {
                int totalAmount = this.itemAmount + targetSlot.itemAmount;
                targetSlot.itemAmount = totalAmount;
                targetSlot.UpdateItemAmountText();

                RemoveItem();
            }
            else
            {
                ItemSO tempItem = targetSlot.item;
                int tempAmount = targetSlot.itemAmount;

                targetSlot.item = this.item;
                targetSlot.itemAmount = this.itemAmount;
                targetSlot.UpdateItemAmountText();

                this.item = tempItem;
                this.itemAmount = tempAmount;
                this.UpdateItemAmountText();
            }
        }

        private void MoveItem(InventorySlot targetSlot)
        {
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
    }
}