using System;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{

    public class Equipment : OkapiElement
    {
        public delegate void OnChange(bool equip, Hypertag slot, Item item);
        public event OnChange onChange;

        [SerializeField]
        private TargetInventory linkedInventory;
        [SerializeField]
        private List<Hypertag>  availableSlots;
        [SerializeField]
        private bool combatTextEnable;
        [SerializeField]
        private float combatTextDuration = 1.0f;
        [SerializeField]
        private Color combatTextEquippedItemColor = Color.yellow;
        [SerializeField]
        private Color combatTextUnequippedItemColor = Color.green;

        private class EquipItem
        {
            public Item     item;
            public float    lastChange;
        }

        private Dictionary<Hypertag, EquipItem>  items = new();

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "Equipment system, use with actions such as Equip/Unequip/Toggle Item, or conditions such as IsEquipped.\nThe following slots are available:\n";
            foreach (var equip in availableSlots)
            {
                desc += $"{ident}   {equip.name}\n";
            }

            return desc;
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = GetRawDescription("", gameObject);

            return _explanation;
        }

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            linkedInventory.CheckErrors(_logs, "linked inventory", gameObject);
        }

        public bool HasSlot(Hypertag slot)
        {
            return (availableSlots.IndexOf(slot) != -1);
        }

        public Item GetItem(Hypertag slot)
        {
            if (items.TryGetValue(slot, out var item))
            {
                return item.item;
            }

            return null;
        }
        public float GetLastChange(Hypertag slot)
        {
            if (items.TryGetValue(slot, out var item))
            {
                return item.lastChange;
            }

            return 0.0f;
        }

        public void Equip(Hypertag slot, Item itemToEquip)
        {
            items[slot] = new()
            {
                item = itemToEquip,
                lastChange = Time.time
            };
            onChange?.Invoke(true, slot, itemToEquip);
            if (combatTextEnable)
                CombatTextManager.SpawnText(gameObject, $"Equipped {itemToEquip.displayName}", combatTextEquippedItemColor, combatTextEquippedItemColor, combatTextDuration);
        }

        public bool IsEquipped(Item item)
        {
            foreach (var equippedItem in items)
            {
                if (equippedItem.Value.item == item) return true;
            }

            return false;
        }

        public void Unequip(Hypertag slot)
        {
            var prevItem = GetItem(slot);
            items[slot] = new()
            {
                item = null,
                lastChange = Time.time
            };
            onChange?.Invoke(false, slot, null);
            if ((combatTextEnable) && (prevItem))
            {
                CombatTextManager.SpawnText(gameObject, $"Unequipped {prevItem.displayName}", combatTextUnequippedItemColor, combatTextUnequippedItemColor, combatTextDuration);
            }
        }

        public List<Hypertag> GetAvailableSlots()
        {
            return availableSlots;
        }

        private void Update()
        {
            var inventory = linkedInventory.GetTarget(gameObject);
            if (inventory != null)
            {
                var slots = new List<Hypertag>(items.Keys); 

                foreach (var slot in slots)
                {
                    if (items.TryGetValue(slot, out var equippedItem) && equippedItem != null)
                    {
                        if (!inventory.HasItem(equippedItem.item))
                        {
                            Unequip(slot);
                        }
                    }
                }
            }
        }
    }

    [Serializable]
    public class TargetEquipment : OkapiTarget<Equipment>
    {
    }
}