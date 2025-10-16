using UnityEngine;

namespace Game
{
    public class EquipmentToInventoryRule : IItemTransferRule
    {
        public bool CanTransfer(Slot from, Slot to)
        {
            if (from is not EquipmentSlot eqSlot || to is not InventorySlot)
                return false;

            if (eqSlot.item is not EquipmentSO)
                return false;

            return true;
        }

        public void ExecuteTransfer(Slot from, Slot to)
        {
            var eqSlot = (EquipmentSlot)from;
            var invSlot = (InventorySlot)to;
            var oldEquipment = eqSlot.item as EquipmentSO;

            if (oldEquipment == null)
                return;

            PlayerManager.Instance.SetEquipment(oldEquipment, -1);

            // Handle swapping
            if (invSlot.item != null)
            {
                if (invSlot.item is EquipmentSO newEquipment &&
                    newEquipment.equipmentType == eqSlot.equipmentType)
                {
                    // Swap items
                    (invSlot.item, eqSlot.item) = (eqSlot.item, invSlot.item);
                    (invSlot.itemAmount, eqSlot.itemAmount) = (eqSlot.itemAmount, invSlot.itemAmount);

                    // Equip new stats after swap
                    PlayerManager.Instance.SetEquipment((EquipmentSO)eqSlot.item, 1);
                }
                else
                {
                    PlayerManager.Instance.SetEquipment(oldEquipment, 1);
                    Debug.Log("Cannot swap equipment with non-equipment item or mismatched type.");
                    return;
                }
            }
            else
            {
                // Move equipment to inventory if target is empty
                invSlot.item = eqSlot.item;
                invSlot.itemAmount = eqSlot.itemAmount;
                eqSlot.RemoveItem();
            }

            invSlot.UpdateUI();
            eqSlot.UpdateUI();

            Debug.Log($"Unequipped {oldEquipment.name} into inventory.");
        }
    }
}
