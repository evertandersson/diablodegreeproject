using UnityEngine;

namespace Game
{
    public class InventoryToActionRule : IItemTransferRule
    {
        public bool CanTransfer(Slot from, Slot to)
        {
            return from is InventorySlot 
                && to is ActionSlot
                && from.item is ActionItemSO;
        }

        public void ExecuteTransfer(Slot from, Slot to)
        {
            var fromSlot = (InventorySlot)from;
            var toSlot = (ActionSlot)to;

            // Stack if same item & stackable
            if (fromSlot.item == toSlot.item && fromSlot.item.isStackable)
            {
                toSlot.itemAmount += fromSlot.itemAmount;
                toSlot.UpdateUI();
                fromSlot.RemoveItem();
                return;
            }

            // Swap
            (toSlot.item, fromSlot.item) = (fromSlot.item, toSlot.item);
            (toSlot.itemAmount, fromSlot.itemAmount) = (fromSlot.itemAmount, toSlot.itemAmount);

            toSlot.UpdateUI();
            fromSlot.UpdateUI();
        }
    }

}
