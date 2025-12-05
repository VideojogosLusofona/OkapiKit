using UnityEngine;

namespace OkapiKit
{
    public class ActionEquipItem : Action
    {
        public enum ItemSource { Explicit, InventoryItem, InventorySlot };

        [SerializeField] private TargetEquipment    equipment;
        [SerializeField] private ItemSource         itemSource;
        [SerializeField] private Item               item;
        [SerializeField] private TargetInventory    inventory;
        [SerializeField] private int                inventorySlot;
        [SerializeField] private bool               unequipIfEquipped;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            var equipManager = equipment.GetTarget(gameObject);
            if (equipManager == null)
            {
                Debug.LogWarning("Equip Item: Equip manager not found!");
                return;
            }

            var inv = inventory.GetTarget(gameObject);
            if ((inv == null) && ((itemSource == ItemSource.InventorySlot) || (itemSource == ItemSource.InventoryItem)))
            {
                Debug.LogWarning("Equip Item: Inventory not found!");
                return;
            }


            Item itemToEquip = null;
            switch (itemSource)
            {
                case ItemSource.Explicit:
                    itemToEquip = item;
                    break;
                case ItemSource.InventoryItem:
                    itemToEquip = item;
                    if (!inv.HasItem(itemToEquip))
                        itemToEquip = null;
                    break;
                case ItemSource.InventorySlot:
                    int count;
                    (itemToEquip, count) = inv.GetSlotContent(inventorySlot);
                    if (count <= 0) itemToEquip = null;
                    break;
                default:
                    break;
            }

            if (itemToEquip == null)
            {
                return;
            }
            if ((itemToEquip.inventorySlots == null) || (itemToEquip.inventorySlots.Length == 0)) return;

            // Check for all slots
            var slot = itemToEquip.GetEquipAutoSlot(equipManager, true);
            if (slot == null) return;

            if (equipManager.HasSlot(slot))
            {
                var prevItem = equipManager.GetItem(slot);
                if (prevItem == itemToEquip)
                {
                    if (unequipIfEquipped)
                    {
                        equipManager.Unequip(slot);
                    }
                }
                else
                {
                    equipManager.Equip(slot, itemToEquip);
                }
            }
            else
            {
                Debug.LogWarning($"Equipment manager doesn't have slot {slot.name}!");
            }
        }

        public override string GetActionTitle() { return "Equip Item"; }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string equipmentName = equipment.GetRawDescription("equipment", refObject);
            string inventoryName = inventory.GetRawDescription("inventory", refObject);
            string itemName = (item != null) ? item.displayName : "[UNDEFINED ITEM]";

            switch (itemSource)
            {
                case ItemSource.Explicit:
                    return $"equips '{itemName}' on {equipmentName}";

                case ItemSource.InventoryItem:
                    return $"equips '{itemName}' from {inventoryName} on {equipmentName}";

                case ItemSource.InventorySlot:
                    return $"equips item from slot {inventorySlot} of {inventoryName} on {equipmentName}";

                default:
                    return $"equips item on {equipmentName}";
            }
        }


        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            equipment.CheckErrors(_logs, "equipment", gameObject);

            if ((itemSource == ItemSource.Explicit) || (itemSource == ItemSource.InventoryItem))
            {
                if (item == null)
                {
                    logs.Add(new LogEntry(LogEntry.Type.Error, $"Need to specify item to equip", $"You need to specify a valid item to equip"));
                }
                else
                {
                    if ((item.inventorySlots == null) || (item.inventorySlots.Length == 0))
                    {
                        logs.Add(new LogEntry(LogEntry.Type.Error, $"Given item doesn't have an equipment slot setup!", $"To equip an item, it has to have defined at least a valid slot!"));
                    }
                    else
                    {
                        for (int i = 0; i < item.inventorySlots.Length; i++)
                        {
                            if (item.inventorySlots[i] == null)
                            {
                                logs.Add(new LogEntry(LogEntry.Type.Error, $"Invalid slot definition on given item (position {i + 1})!", $"To equip an item, the slot definitions have to be valid!"));
                            }
                        }
                    }
                }
            }

            if ((itemSource == ItemSource.InventorySlot) || (itemSource == ItemSource.InventoryItem))
            {
                inventory.CheckErrors(_logs, "inventory", gameObject);
            }
        }
    }
}
