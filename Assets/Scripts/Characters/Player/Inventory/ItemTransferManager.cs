using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ItemTransferManager : MonoBehaviour
    {
        public static ItemTransferManager Instance { get; private set; }

        private List<IItemTransferRule> rules = new();

        private void Awake()
        {
            Instance = this;
            rules.Add(new InventoryToInventoryRule());
            rules.Add(new InventoryToActionRule());
            rules.Add(new InventoryToEquipmentRule());
            rules.Add(new EquipmentToInventoryRule());
            rules.Add(new ActionToActionRule());
            rules.Add(new ActionToInventoryRule());
        }

        public void TryTransfer(Slot from, Slot to)
        {
            foreach (var rule in rules)
            {
                if (rule.CanTransfer(from, to))
                {
                    rule.ExecuteTransfer(from, to);
                    return;
                }
            }

            Debug.Log($"No rule for {from.GetType().Name} → {to.GetType().Name}");
        }
    }
}
