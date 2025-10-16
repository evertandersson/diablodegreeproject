using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Xml;

namespace Game
{
    public abstract class Slot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public ItemSO item;
        public int itemAmount;
        public RawImage artwork;
        public TMPro.TextMeshProUGUI amountText;

        protected CanvasGroup canvasGroup;
        protected Canvas parentCanvas;
        protected Transform originalParent;
        protected RectTransform artworkRectTransform;

        protected RectTransform frameRectTransform;

        protected virtual void Awake()
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            parentCanvas = GetComponentInParent<Canvas>();
            artworkRectTransform = artwork.GetComponent<RectTransform>();
            frameRectTransform = GetComponent<RectTransform>();
        }

        public virtual void UpdateUI()
        {
            if (item == null)
            {
                artwork.enabled = false;
                if (amountText) amountText.enabled = false;
            }
            else
            {
                artwork.enabled = true;
                artwork.texture = item.itemIcon;
                if (amountText)
                {
                    amountText.text = itemAmount > 1 ? itemAmount.ToString() : "";
                    amountText.enabled = itemAmount > 1;
                }
            }
        }

        public virtual void SetItem(ItemSO newItem)
        {
            item = newItem;
            itemAmount = 1;
            if (artwork) artwork.texture = item?.itemIcon;
            UpdateUI();
            UpdateItemAmountText();
        }

        public void UpdateItemAmountText()
        {
            if (amountText != null)
            {
                amountText.text = itemAmount > 1 ? itemAmount.ToString() : "";
                if (itemAmount <= 0) RemoveItem();
            }
        }

        public virtual void CheckIfItemNull() 
        { 
            if (item == null)
                UpdateUI();
        }

        public virtual void RemoveItem()
        {
            item = null;
            itemAmount = 0;
            UpdateUI();
        }

        #region Drag and Drop

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (item == null) return;

            originalParent = artwork.transform.parent;
            artwork.transform.SetParent(parentCanvas.transform);
            canvasGroup.blocksRaycasts = false;
            EquipmentManager.Instance.ShowEquipmentSlots();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (item == null) return;
            artworkRectTransform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            artwork.transform.SetParent(originalParent);
            artworkRectTransform.localPosition = Vector3.zero;
            canvasGroup.blocksRaycasts = true;

            if (item == null) return;

            // Detect target slot
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);

            Slot target = null;
            foreach (var r in results)
            {
                target = r.gameObject.GetComponent<Slot>();
                if (target != null) break;
            }

            // Pass to ItemTransferManager
            if (target != null && target != this)
                ItemTransferManager.Instance.TryTransfer(this, target);

            EquipmentManager.Instance.HideEquipmentSlots();
        }

        #endregion 

        #region Handle Hovering

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (item == null) return;
            InfoWindow.Instance.ShowEquipmentInfoWindow(transform.position, frameRectTransform.rect.width, frameRectTransform.rect.height, this.item);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (item == null) return;
            InfoWindow.Instance.HideInfoWindow();
        }

        #endregion
    }
}
