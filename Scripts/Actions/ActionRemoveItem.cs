using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class ActionRemoveItem : Action
    {
        [SerializeField] private Item               item;
        [SerializeField] private int                quantity = 1;
        [SerializeField] private TargetInventory    inventory;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            var target = inventory.GetTarget(gameObject);
            if (target == null)
            {
                Debug.LogWarning("Remove Item: Target not found!");
                return;
            }

            target.Remove(item, quantity);
        }

        public override string GetActionTitle() { return "Remove Item"; }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string itemName = (item) ? (item.displayName) : ("[UNDEFINED]");
            string inventoryName = inventory.GetRawDescription("inventory", refObject);

            if (quantity > 1)
                return $"removes {quantity}x {itemName} to {inventoryName}.";

            return $"removes item {itemName} to {inventoryName}.";
        }

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            inventory.CheckErrors(_logs, "inventory", gameObject);

            if (item)
            {
                if (item.isCategory)
                {
                    logs.Add(new LogEntry(LogEntry.Type.Error, $"Can't remove categories from inventory!", $"You can only remove items from the inventory, not whole categories!"));
                }
            }
            else
            {
                logs.Add(new LogEntry(LogEntry.Type.Error, "Need to define an item to remove from inventory!", "Need to define an item to remove from inventory!"));
            }
        }
    }
}
