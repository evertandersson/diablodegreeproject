using UnityEngine;


namespace Game
{
    public class ActionToInventoryRule : IItemTransferRule
    {
        public bool CanTransfer(Slot from, Slot to)
        {
            return from is ActionSlot && to is InventorySlot;
        }

        public void ExecuteTransfer(Slot from, Slot to)
        {
            var fromSlot = (ActionSlot)from;
            var toSlot = (InventorySlot)to;

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
