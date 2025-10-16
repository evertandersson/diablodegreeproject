using UnityEngine;

namespace Game
{
    public interface IItemTransferRule
    {
        bool CanTransfer(Slot from, Slot to);
        void ExecuteTransfer(Slot from, Slot to);
    }

}
