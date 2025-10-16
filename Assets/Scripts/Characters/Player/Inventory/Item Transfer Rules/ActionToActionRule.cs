using UnityEngine;

namespace Game
{
    public class ActionToActionRule : IItemTransferRule
    {
        public bool CanTransfer(Slot from, Slot to)
        {
            return from is ActionSlot && to is ActionSlot;
        }

        public void ExecuteTransfer(Slot from, Slot to)
        {
            var fromSlot = (ActionSlot)from;
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
