using System.Diagnostics;

namespace Game
{
    public class EquipmentToInventoryRule : IItemTransferRule
    {
        public bool CanTransfer(Slot from, Slot to)
        {
            return from is EquipmentSlot eqSlot && to is InventorySlot;
        }

        public void ExecuteTransfer(Slot from, Slot to)
        {
            var eqSlot = (EquipmentSlot)from;
            var invSlot = (InventorySlot)to;

            // Store reference before clearing
            var oldEquipment = eqSlot.item as EquipmentSO;

            // If the target slot has an equipment item, ensure same type
            if (invSlot.item is EquipmentSO newEquipment)
            {
                if (newEquipment.equipmentType != eqSlot.equipmentType)
                {
                    return;
                }
            }

            // Unequip old stats before removing
            if (oldEquipment != null)
                PlayerManager.Instance.SetEquipment(oldEquipment, -1);

            // Perform swap or move
            if (invSlot.item != null)
            {
                (to.item, from.item) = (from.item, to.item);
                (to.itemAmount, from.itemAmount) = (from.itemAmount, to.itemAmount);
            }
            else
            {
                invSlot.item = eqSlot.item;
                invSlot.itemAmount = eqSlot.itemAmount;
                eqSlot.RemoveItem();
            }

            invSlot.UpdateUI();
            eqSlot.UpdateUI();
        }
    }
}
