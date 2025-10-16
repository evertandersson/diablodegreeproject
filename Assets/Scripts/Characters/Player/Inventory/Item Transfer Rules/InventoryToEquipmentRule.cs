using UnityEngine;

namespace Game
{
    public class InventoryToEquipmentRule : IItemTransferRule
    {
        public bool CanTransfer(Slot from, Slot to)
        {
            if (from is not InventorySlot || to is not EquipmentSlot) return false;
            if (from.item is not EquipmentSO eqItem) return false;

            return eqItem.equipmentType == ((EquipmentSlot)to).equipmentType;
        }

        public void ExecuteTransfer(Slot from, Slot to)
        {
            var eqSlot = (EquipmentSlot)to;
            var invSlot = (InventorySlot)from;
            var eqItem = (EquipmentSO)invSlot.item;

            // Unequip old item
            if (eqSlot.item is EquipmentSO oldItem)
                PlayerManager.Instance.SetEquipment(oldItem, -1);

            // Equip new item
            eqSlot.item = eqItem;
            eqSlot.itemAmount = invSlot.itemAmount;
            eqSlot.UpdateUI();

            PlayerManager.Instance.SetEquipment(eqItem, 1);
            invSlot.RemoveItem();
        }
    }
}