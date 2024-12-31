using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class EquipmentManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static EquipmentManager Instance;

        private CanvasGroup canvasGroup;
        public EquipmentSlot[] equipmentSlots;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            equipmentSlots = GetComponentsInChildren<EquipmentSlot>(true);

            Instance = this;
        }

        public void ShowEquipmentSlots()
        {
            canvasGroup.alpha = 1.0f;
        }

        public void HideEquipmentSlots()
        {
            canvasGroup.alpha = 0.0f;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            ShowEquipmentSlots();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            HideEquipmentSlots();
        }

        public void GetStatsFromArmour()
        {
            foreach(EquipmentSlot slot in equipmentSlots)
            {
                EquipmentSO equipmentSO = slot.item as EquipmentSO;
                if (equipmentSO != null)
                {
                    PlayerManager.Instance.SetEquipment(equipmentSO);
                }
            }
        }
    }

}
