using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{

    public class Inventory : OkapiElement, IEnumerable<(int slot, Item item, int count)>
    {
        public delegate void OnChange(bool add, Item item, int slot);
        public event OnChange onChange;

        [SerializeField]
        private bool limited = false;
        [SerializeField, ShowIf(nameof(limited))]
        private int maxSlots = 9;
        [SerializeField] 
        private bool combatTextEnable;
        [SerializeField] 
        private float combatTextDuration = 1.0f;
        [SerializeField] 
        private Color combatTextReceivedItemColor = Color.yellow;
        [SerializeField] 
        private Color combatTextLostItemColor = Color.green;

        private class Items
        {
            public Item item;
            public int count;
        };

        private List<Items> items;

        public int Add(Item item, int quantity)
        {
            int count = 0;
            for (int i = 0; i < quantity; i++)
            {
                if (Add(item, false)) count++;
            }

            if (combatTextEnable)
            {
                if (quantity > 1)
                    CombatTextManager.SpawnText(gameObject, $"Received {quantity}x {item.displayName}", combatTextReceivedItemColor, combatTextReceivedItemColor, combatTextDuration);
                else
                    CombatTextManager.SpawnText(gameObject, $"Received {item.displayName}", combatTextReceivedItemColor, combatTextReceivedItemColor, combatTextDuration);
            }

            return count;
        }

        public bool Add(Item item, bool combatText = true)
        {
            int slotIndex = GetSlot(item);
            if (slotIndex == -1)
            {
                return false;
            }

            items[slotIndex].item = item;
            items[slotIndex].count++;

            onChange?.Invoke(true, item, slotIndex);

            if ((combatTextEnable) && (combatText))
                CombatTextManager.SpawnText(gameObject, $"Received {item.displayName}", combatTextReceivedItemColor, combatTextReceivedItemColor, combatTextDuration);

            return true;
        }

        public int Remove(Item item, int quantity)
        {
            int ret = 0;

            for (int i = 0; i < quantity; i++)
            {
                if (Remove(item, false)) ret++;
            }

            if (combatTextEnable)
            {
                if (quantity > 1)
                    CombatTextManager.SpawnText(gameObject, $"Lost {quantity}x {item.displayName}", combatTextLostItemColor, combatTextLostItemColor, combatTextDuration);
                else
                    CombatTextManager.SpawnText(gameObject, $"Lost {item.displayName}", combatTextLostItemColor, combatTextLostItemColor, combatTextDuration);
            }


            return ret;
        }

        public bool Remove(Item item, bool combatText = true)
        {
            int slotIndex = FindItem(item);
            if (slotIndex == -1)
            {
                return false;
            }

            items[slotIndex].count--;

            if (items[slotIndex].count <= 0)
            {
                items[slotIndex].item = null;
                items[slotIndex].count = 0;
            }

            onChange?.Invoke(false, item, slotIndex);

            if ((combatText) && (combatTextEnable))
                CombatTextManager.SpawnText(gameObject, $"Lost {item.displayName}", combatTextLostItemColor, combatTextLostItemColor, combatTextDuration);

            return true;
        }

        public (Item, int) GetSlotContent(int slot)
        {
            if ((items == null) || (items.Count <= slot)) return (null, 0);

            return (items[slot].item, items[slot].count);
        }

        int FindItem(Item item)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if ((items[i].item == item) && (items[i].count > 0))
                {
                    return i;
                }
            }

            return -1;
        }

        int GetSlot(Item item)
        {
            if (items == null) items = new();

            if (item.isStackable)
            {
                // Find a stack
                for (int i = 0; i < items.Count; i++)
                {
                    if ((items[i].item == item) && (items[i].count < item.maxStack))
                    {
                        return i;
                    }
                }
            }
            else
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if ((items[i].item == null) && (items[i].count == 0))
                    {
                        items[i].item = null;
                        items[i].count = 0;
                        return i;
                    }
                }
            }

            if ((limited) && (items.Count >= maxSlots)) return -1;

            items.Add(new Items { item = item, count = 0 });

            return items.Count - 1;
        }

        public bool HasItem(Item item)
        {
            if (items == null) return false;

            foreach (var i in items)
            {
                if ((i.item == item) && (i.count > 0)) return true;
            }

            return false;
        }

        public bool HasItems()
        {
            if (items == null) return false;

            foreach (var i in items)
            {
                if ((i.item != null) && (i.count > 0)) return true;
            }

            return false;
        }

        public int GetItemCount(Item item)
        {
            if (items == null) return 0;

            int count = 0;
            foreach (var i in items)
            {
                if (i.item == item) count += i.count;
            }

            return count;
        }

        public IEnumerator<(int slot, Item item, int count)> GetEnumerator()
        {
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if ((items[i].item != null) && (items[i].count > 0))
                    {
                        yield return (i, items[i].item, items[i].count);
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "Inventory system, use with actions such as Add Item and Remove Item, or conditions such as HasItem or GetItemCount";
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = GetRawDescription("", gameObject);

            return _explanation;
        }
    }

    [Serializable]
    public class TargetInventory : OkapiTarget<Inventory>
    {
    }
}