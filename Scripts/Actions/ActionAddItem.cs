using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class ActionAddItem : Action
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
                Debug.LogWarning("Add Item: Target not found!");
                return;
            }

            target.Add(item, quantity);
        }

        public override string GetActionTitle() { return "Add Item"; }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string itemName = (item) ? (item.displayName) : ("[UNDEFINED]");
            string inventoryName = inventory.GetRawDescription("inventory", refObject);

            if (quantity > 1)
                return $"adds {quantity}x {itemName} to {inventoryName}.";

            return $"adds item {itemName} to {inventoryName}.";
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            inventory.CheckErrors(_logs, "inventory", gameObject);

            if (item)
            {
                if (item.isCategory)
                {
                    logs.Add(new LogEntry(LogEntry.Type.Error, $"Can't add categories to inventory!", $"You can only add items to the inventory, not whole categories!"));
                }
            }
            else
            {
                logs.Add(new LogEntry(LogEntry.Type.Error, "Need to define an item to remove from inventory!", "Need to define an item to remove from inventory!"));
            }
        }
    }
}
